﻿using System.Collections;
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

    [SerializeField]
    private GameObject mp_textBlock;

    [SerializeField]
    private GameObject m_firstLetterSlot;

    [SerializeField]
    private GameObject m_nextLetterSlot;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private List<SeekPosition> m_letterBlocks;

    static TrumpRoom ms_instance;

    private string m_lastText = "";

    [SerializeField]
    private SpriteRenderer m_elevatorShaftSprite;

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

    public IEnumerator Select()
    {
        yield return MakeUnselectable();
    }

    public void Awake()
    {
        ms_instance = this;
        m_nextLetterSlot = m_firstLetterSlot;
        m_letterBlocks = new List<SeekPosition>();
        StartCoroutine(CreateText(""));
        

        StartCoroutine(TellToPressSpace());
    }

    public static GameObject GetGameObjectChatBubble()
    {
        return ms_instance.mp_chatBubble;
    }

    public IEnumerator MakeSelectable()
    {
        if (m_selectionText != "")
        {
            yield break;
        }

        yield return CreateText(Dictionary.ms_instance.PickWord(TrumpTower.ms_instance.GetDifficulty()));
    }

    public bool InAMeeting()
    {
        return ((m_currentVisitor != null && m_currentVisitor.InMeeting) || (m_currentResident !=null && m_currentResident.InMeeting));
    }

    public void ResetOverText()
    {
        
    }

    public IEnumerator MakeUnselectable()
    {
        yield return CreateText("");
    }

    private IEnumerator CreateText(string text)
    {
        m_selectionText = text;
        m_nextLetterSlot = m_firstLetterSlot;
        foreach (SeekPosition letter in m_letterBlocks)
        {
            letter.StartCoroutine(letter.End());
        }

        m_letterBlocks = new List<SeekPosition>();

        foreach (char character in text)
        {
            yield return MakeLetter(character);
        }
    }

    public void SetElevatorShaftSprite(Sprite room)
    {
        m_elevatorShaftSprite.sprite = room;
    }

    private IEnumerator MakeLetter(char character)
    {
        //TODO: spawn block off screen

        SeekPosition letter = GameObject.Instantiate(mp_textBlock, m_nextLetterSlot.transform.position, m_firstLetterSlot.transform.rotation, null).GetComponent<SeekPosition>();
        m_letterBlocks.Add(letter);
        //set it's target
        letter.SetLetter(character);
        letter.SetTarget(m_nextLetterSlot);
        yield return StartCoroutine(letter.Unselect());
        m_nextLetterSlot = letter.GetNextSlot();
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
        CheckForSubstring("");
    }

    private float m_minChatBubbleSpawnTime = 1.0f;
    private float m_maxChatBubbleSpawnTime = 3.0f;

    private IEnumerator MeetingRoutine()
    {
        m_currentVisitor.InMeeting = true;
        m_currentResident.InMeeting = true;

        float t = 0.0f;

        if (Random.Range(0, 10) <= 5)
        {
            yield return GameObject.Instantiate(this.mp_chatBubble, m_residentSlot.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), 0), transform.rotation, m_residentSlot.transform).GetComponent<CommunicationBubble>().FadeOutBubble();

        }
        else
        {
            yield return GameObject.Instantiate(this.mp_chatBubble, m_visitorSlot.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), 0), transform.rotation, m_visitorSlot.transform).GetComponent<CommunicationBubble>().FadeOutBubble();
        }

        m_currentVisitor.InMeeting = false;
        m_currentResident.InMeeting = false;

        yield return TrumpTower.ms_instance.DropoffAccessRooms();
    }

    private IEnumerator TellToPressSpace()
    {
        float spaceWaitingTime = 0.0f;
        while (true)
        {

            if (m_overText.text.Length == m_selectionText.Length && m_selectionText != "")
            {
                spaceWaitingTime += Time.deltaTime;
            }
            else
            {
                spaceWaitingTime = 0.0f;
            }

            if (spaceWaitingTime > 3.0f)
            {
                if (m_selectionText.Length <= 4)
                {

                    m_overText.text += " [PRESS SPACE]";
                }
                else if (m_selectionText.Length <= 9)
                {


                    m_overText.text += " [SPACE]";
                }
                else
                {
                    m_overText.text += " [ ]";
                }
            }

            yield return new WaitForEndOfFrame();

        }
    }

    public void CheckForSubstring(string currentText)
    {
        m_overText.text = ""; //TODO: remove
        if (currentText.Length <= m_selectionText.Length && m_selectionText.Substring(0, currentText.Length) == currentText)
        {
            for (int i = 0; i <= currentText.Length - 1; i++)
            {
                m_letterBlocks[i].StartCoroutine(m_letterBlocks[i].Select());
            }

            if (currentText.Length < m_lastText.Length)
            {
                for (int i = currentText.Length; i < m_letterBlocks.Count; i++)
                {
                    m_letterBlocks[i].StartCoroutine(m_letterBlocks[i].Unselect());
                }
            }

            m_lastText = currentText;
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
            TrumpTower.ms_instance.IncrementCombo();
            m_currentResident.MeetWith(m_currentVisitor);
            m_currentVisitor.MeetWith(m_currentResident);
            StartCoroutine(MeetingRoutine());
            return m_currentVisitor.GetHappinessPoints() + m_currentResident.GetHappinessPoints();
        }

        return 0.0f;
    }
}
