using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    string m_name;


    public string Name { get { return m_name; } set { m_name = value; } }

    [SerializeField]
    private GameObject mp_itineraryBackdrop;

    [SerializeField]
    private GameObject mp_chatBubble;

    private ItineraryBubble m_itineraryBubble;

    private float m_angerSeconds = 0.0f;

    private float m_maxAnger = 30.0f;

    [SerializeField]
    private GameObject mp_icon;

    private List<Character> m_agenda;

    public bool InMeeting;

    private IEnumerator m_angerRoutine;

    public void FulfillAppointmnet(Character character)
    {
        m_agenda.Remove(character);
    }

    public GameObject GetIcon()
    {
        return mp_icon;
    }

    public List<Character> GetAppointments()
    {
        return m_agenda;
    }

    public float GetHappinessPoints()
    {
        return m_maxAnger - m_angerSeconds;
    }

    void Start()
    {
        m_angerRoutine = GetAngry();
        m_agenda = new List<Character>();
        m_itineraryBubble = GameObject.Instantiate(mp_itineraryBackdrop, this.transform.position, this.transform.rotation, this.transform).GetComponent<ItineraryBubble>();
        StartCoroutine(GetAngry());
    }

    public void ResetAnger()
    {
        m_angerSeconds = 0.0f;
    }

    private IEnumerator GetAngry()
    {
        while (TrumpTower.ms_instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetAnger();
        while (m_angerSeconds < m_maxAnger && m_agenda.Count > 0)
        {
            if (InMeeting == false && !TrumpTower.ms_instance.IsCharacterInElevator(this))
            {

                m_angerSeconds += 1.0f;

            }
            else
            {
                m_angerSeconds -= 1.0f;
                if (m_angerSeconds < 0.0f)
                {
                    m_angerSeconds = 0.0f;
                }
            }
            m_itineraryBubble.SetAnger(m_angerSeconds / m_maxAnger);
            yield return new WaitForSeconds(1.0f);
        }

        if (m_agenda.Count == 0)
        {
            float t = 1.0f;
            while (t > 0.0f)
            {
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
                m_itineraryBubble.SetFade(t);
            }
            yield break;

        }

        float fadeTime = .2f;
        while (fadeTime > .01f)
        {
            fadeTime *= .9f;
            GameObject.Instantiate(TrumpRoom.GetGameObjectChatBubble(), transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), transform.rotation, null);
            yield return new WaitForSeconds(fadeTime);
        }

        yield return TrumpTower.ms_instance.GameOverRoutine();
    }

    public bool HasAppointment(List<Character> characterPool = null)
    {
        if (characterPool == null)
        {

            return m_agenda.Count > 0;

        }

        foreach(Character c in characterPool)
        {
            if (c.GetAppointments().Contains(this))
            {
                return true;
            }
        }

        return m_agenda.Count > 0;
    }

    public void MeetWith(Character character)
    {
        if (m_agenda.Contains(character))
        {
            m_agenda.Remove(character);
            m_itineraryBubble.SetItinerary(m_agenda);
        }
    }

    public GameObject HeadIcon()
    {
        return mp_icon;
    }

    private IEnumerator FadeInBubble()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            m_itineraryBubble.SetFade(t);
            yield return new WaitForEndOfFrame();
        }
    }

    public void GenerateAgenda(List<Character> m_characterBase)
    {
        StartCoroutine(FadeInBubble());
        m_agenda = new List<Character>();
        int totalAgenda = TrumpTower.ms_instance.GetDifficulty();
        for (int i = 0; i < Mathf.Max(3, totalAgenda); i++)
        {
            m_agenda.Add(m_characterBase[Random.Range(0, m_characterBase.Count)]);
        }

        m_itineraryBubble.SetItinerary(m_agenda);
        StartCoroutine(m_angerRoutine);
    }
}
