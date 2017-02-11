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

    private static float ms_maxAnger = 40.0f;

    [SerializeField]
    private GameObject mp_icon;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;


    private List<Character> m_agenda;

    public bool InMeeting;

    private IEnumerator m_angerRoutine;

    [SerializeField]
    private Rigidbody2D m_rigidbody;


    public static void StartGame()
    {
        ms_maxAnger = 10.0f;
    }

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
        return ms_maxAnger - m_angerSeconds;
    }

    void Start()
    {
        m_angerRoutine = GetAngry();
        m_agenda = new List<Character>();
        m_itineraryBubble = GameObject.Instantiate(mp_itineraryBackdrop, this.transform.position, this.transform.rotation, this.transform).GetComponent<ItineraryBubble>();

        m_itineraryBubble.GetComponent<ItineraryBubble>().SetSpringStart(m_rigidbody);
        StartCoroutine(GetAngry());
    }

    public void ResetAnger()
    {
        m_angerSeconds = 0.0f;
        ms_maxAnger += 4.0f;
        m_itineraryBubble.SetAnger(m_angerSeconds / ms_maxAnger);
    }

    public IEnumerator FadeInFadeOut()
    {
        float t = 0.0f;
        Color original = m_spriteRenderer.color;
        t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            m_spriteRenderer.color = Color.Lerp(Color.clear, original, t);
            yield return new WaitForEndOfFrame();
        }
    }

    public bool CanMeet()
    {
        foreach (Character character in m_agenda)
        {
            if (character.InMeeting == false)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator GetAngry()
    {
        while (TrumpTower.ms_instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetAnger();
        while (m_angerSeconds < ms_maxAnger)
        {
            while (m_agenda.Count == 0)
            {
                m_angerSeconds = 0.0f;
                m_itineraryBubble.SetFade(0.0f);
                yield return new WaitForEndOfFrame();
            }
            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime;
                m_itineraryBubble.SetFade(t);
                yield return new WaitForEndOfFrame();
            }
            while (m_agenda.Count > 0 && m_angerSeconds < ms_maxAnger)
            {
                if (InMeeting == false && !TrumpTower.ms_instance.IsCharacterInElevator(this) && CanMeet())
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

                if (m_agenda.Count > 0)
                {
                    m_itineraryBubble.SetAnger(m_angerSeconds / ms_maxAnger);
                }
                yield return new WaitForSeconds(1.0f);
            }
            t = 1.0f;
            while (t > 0.0f)
            {
                t -= Time.deltaTime;
                m_itineraryBubble.SetFade(t);
                yield return new WaitForEndOfFrame();
            }
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

    private IEnumerator CompleteMeeting(Character character)
    {

        GameObject go = GameObject.Instantiate(character.GetIcon(), transform.position,transform.rotation,null);

        float i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime;
            go.transform.localScale = Vector3.one * Mathf.Lerp(0.0f, 3.0f, i);
            yield return new WaitForEndOfFrame();
        }

        i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime;
            go.transform.localScale = Vector3.one * Mathf.Lerp(3.0f, 0.0f,i);
            yield return new WaitForEndOfFrame();
        }

        Destroy(go); 
    }

    public void MeetWith(Character character)
    {
        if (m_agenda.Contains(character))
        {
            m_agenda.Remove(character);

            StartCoroutine(CompleteMeeting(character));

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
        int totalAgenda = Random.Range(2, Mathf.Max(2, TrumpTower.ms_instance.GetDifficulty()));

        List<Character> workableCharacters = new List<Character>();

        foreach (Character character in m_characterBase)
        {
            if (character == this)
            {
                continue;
            }

            if (TrumpTower.ms_instance.GetInSameRoom(character, this))
            {
                continue;
            }

            workableCharacters.Add(character);
        }

        for (int i = 0; i < Mathf.Min(workableCharacters.Count, totalAgenda); i++)
        {
            Character addedCharacter = workableCharacters[Random.Range(0, workableCharacters.Count)];

            m_agenda.Add(addedCharacter);
            workableCharacters.Remove(addedCharacter);
        }

        m_itineraryBubble.SetItinerary(m_agenda);
        StartCoroutine(m_angerRoutine);
    }
}
