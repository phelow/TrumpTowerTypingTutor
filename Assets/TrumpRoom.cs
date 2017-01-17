using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpRoom : MonoBehaviour
{
    private string m_selectionText = "";

    [SerializeField]
    private GameObject m_nextRoomSlot;

    [SerializeField]
    private TextMesh m_text;

    Character m_currentResident;

    private float m_meetingTime = 3.0f;

    Character m_currentVisitor;

    [SerializeField]
    private TextMesh m_residentText;

    [SerializeField]
    private TextMesh m_visitorText;

    [SerializeField]
    private GameObject m_visitorSlot;

    [SerializeField]
    private GameObject m_residentSlot;

    [SerializeField]
    private GameObject m_elevatorSlot;

    [SerializeField]
    public GameObject mp_chatBubble;

    [SerializeField]
    public TextMesh m_overText;

    static TrumpRoom ms_instance;


    public Character CurrentResident
    {
        get
        {
            return m_currentResident;
        }
    }

    public Character CurrentVisitor
    {
        get
        {
            return m_currentVisitor;
        }
    }

    public Vector3 GetElevatorPosition()
    {
        return m_elevatorSlot.transform.position;
    }

    public void Select()
    {
        m_selectionText = "";
        m_text.text = m_selectionText;
    }

    public void Awake()
    {
        ms_instance = this;
    }

    public static GameObject GetGameObjectChatBubble()
    {
        return ms_instance.mp_chatBubble;
    }

    public void MakeSelectable()
    {
        if (m_selectionText != "")
        {
            return;
        }

        m_selectionText = Dictionary.ms_instance.PickWord(TrumpTower.ms_instance.GetDifficulty());
        m_text.text = m_selectionText;
    }

    public void MakeUnselectable()
    {
        m_selectionText = "";
        m_text.text = m_selectionText;
    }

    public GameObject GetNextRoomSpawnPoint()
    {
        return m_nextRoomSlot;
    }

    public void SetResident(Character resident)
    {
        m_currentResident = resident;


        if (resident == null)
        {
            m_residentText.text = "vacant";
        }
        else
        {
            resident.transform.position = m_residentSlot.transform.position;
            resident.transform.SetParent(this.transform);
            m_residentText.text = resident.Name;

        }
    }

    public void SetVisitor(Character visitor)
    {
        if (visitor == null)
        {
            m_visitorText.text = "vacant";
        }
        else
        {
            visitor.transform.SetParent(this.transform);
            visitor.transform.position = m_visitorSlot.transform.position;
            m_visitorText.text = visitor.Name;
        }

        m_currentVisitor = visitor;
    }

    public string GetWord()
    {
        return m_selectionText;
    }

    public void ClearOverText()
    {
        m_overText.text = "";
    }

    private float m_minChatBubbleSpawnTime = 1.0f;
    private float m_maxChatBubbleSpawnTime = 3.0f;

    private IEnumerator MeetingRoutine()
    {
        m_currentVisitor.InMeeting = true;
        m_currentResident.InMeeting = true;

        float t = 0.0f;

        while (t < m_meetingTime)
        {
            float m_waitTime = Random.Range(m_minChatBubbleSpawnTime, m_maxChatBubbleSpawnTime);

            if (Random.Range(0, 10) <= 5)
            {
                GameObject.Instantiate(this.mp_chatBubble, m_residentSlot.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), transform.rotation, m_residentSlot.transform);

            }
            else
            {
                GameObject.Instantiate(this.mp_chatBubble, m_visitorSlot.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), transform.rotation, m_visitorSlot.transform);
            }
            yield return new WaitForSeconds(m_waitTime);
            t += m_waitTime;
        }

        m_currentVisitor.InMeeting = false;
        m_currentResident.InMeeting = false;
    }

    public void CheckForSubstring(string currentText)
    {
        m_overText.text = "";
        if (currentText.Length <= m_selectionText.Length && m_selectionText.Substring(0, currentText.Length) == currentText)
        {
            m_overText.text = currentText;
        }

    }

    public float CheckForMeeting()
    {
        if (m_currentVisitor == null || m_currentResident == null)
        {
            return 0;
        }

        if (m_currentResident.InMeeting || m_currentVisitor.InMeeting)
        {
            return 0;
        }

        if (m_currentResident.GetAppointments().Contains(m_currentVisitor) || m_currentVisitor.GetAppointments().Contains(m_currentResident))
        {


            m_currentResident.MeetWith(m_currentVisitor);
            m_currentVisitor.MeetWith(m_currentResident);
            StartCoroutine(MeetingRoutine());
            return m_currentVisitor.GetHappinessPoints() + m_currentResident.GetHappinessPoints();
        }

        return 0.0f;
    }

    public void Update()
    {
    }
}
