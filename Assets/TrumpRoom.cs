using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpRoom : MonoBehaviour
{
    private string m_selectionText;

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

    public void MakeSelectable()
    {
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
            m_residentText.text = resident.Name;

        }
    }

    public void SetVisitor(Character visitor)
    {
        m_currentVisitor = visitor;
        if (visitor == null)
        {
            m_visitorText.text = "vacant";
        }
        else {
            m_visitorText.text = visitor.Name;
        }
    }

    public string GetWord()
    {
        return m_selectionText;
    }

    private IEnumerator MeetingRoutine()
    {
        m_currentVisitor.InMeeting = true;
        m_currentResident.InMeeting = true;

        yield return new WaitForSeconds(m_meetingTime);

        m_currentVisitor.InMeeting = false;
        m_currentResident.InMeeting = false;

        TrumpTower.ms_instance.ExitMeeting(this);

    }

    public void CheckForMeeting()
    {
        if ((m_currentResident != null && m_currentVisitor != null) && ((m_currentResident.GetAppointments().Contains(m_currentVisitor) || m_currentVisitor.GetAppointments().Contains(m_currentResident))))
        {
            m_currentResident.MeetWith(m_currentResident);
            m_currentResident.MeetWith(m_currentVisitor);
            StartCoroutine(MeetingRoutine());
        }
    }

    public void Update()
    {
    }
}
