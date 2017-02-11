using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TrumpTower : MonoBehaviour
{
    public static TrumpTower ms_instance;

    private int m_numRooms = 1;
    private int m_numCharacters = 1;
    private int m_level = 0;
    private float m_score = 0.0f;

    private int m_combo = 1;

    private int m_difficulty = 0;

    [SerializeField]
    private GameObject mp_room;

    [SerializeField]
    private TrumpRoom m_lastRoomAdded;

    private Stack<Character> m_newCharacterQueue;

    public List<TrumpRoom> m_rooms;

    private List<Character> m_activeCharacters;

    private Character m_elevatorCharacter = null;

    [SerializeField]
    private List<Character> m_characterPool;

    [SerializeField]
    private GameObject m_elevator;

    [SerializeField]
    private TextMesh m_elevatorText;

    [SerializeField]
    private CanvasGroup m_intermissionCanvas;

    [SerializeField]
    private Text m_intermissionYourScore;
    [SerializeField]
    private Text m_intermissionHighScore;
    [SerializeField]
    private Text m_levelText;

    [SerializeField]
    private Text m_doAnythingToContinue;

    [SerializeField]
    private TextMesh m_scoreText;

    [SerializeField]
    private Sprite m_topShaft;

    [SerializeField]
    private Sprite m_middleShaft;

    [SerializeField]
    private Sprite m_bottomShaft;

    [SerializeField]
    private Text m_message;
    bool selectionMade = false;
    public static bool m_waitForSpace = false;
    bool m_gameOverStarted = false;

    public IEnumerator GameOverRoutine()
    {
        if (m_gameOverStarted == false)
        {
            m_gameOverStarted = true;
            m_message.text = "Game Over";
            yield return EndLevel();
            Fader.Instance.FadeIn(1.0f).LoadLevel(1);
        }
    }

    public bool GetInSameRoom(Character a, Character b)
    {
        foreach(TrumpRoom room in m_rooms)
        {
            if((room.CurrentVisitor == a && room.CurrentResident == b) || (room.CurrentResident == a && room.CurrentVisitor == b))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCharacterInElevator(Character character)
    {
        return character == m_elevatorCharacter;
    }

    private void SetElevatorCharacter(Character elevatorCharacter)
    {
        if (elevatorCharacter == null)
        {
            m_elevatorText.text = "empty";

            m_elevatorCharacter = elevatorCharacter;
        }
        else
        {
            elevatorCharacter.transform.position = m_elevator.transform.position;
            elevatorCharacter.transform.SetParent(m_elevator.transform);

            m_elevatorCharacter = elevatorCharacter;
            m_elevatorText.text = elevatorCharacter.Name;

        }

    }

    private void PopulateCharacterDatabase()
    {
        m_rooms = new List<TrumpRoom>();
        m_activeCharacters = new List<Character>();
        m_newCharacterQueue = new Stack<Character>();

        m_characterPool.Reverse();

        foreach (Character character in m_characterPool)
        {
            m_newCharacterQueue.Push(character);
        }


        m_activeCharacters.Add(m_newCharacterQueue.Pop());
        m_activeCharacters.Add(m_newCharacterQueue.Pop());

        m_activeCharacters[0].StartCoroutine(m_activeCharacters[0].FadeInFadeOut());
        m_activeCharacters[1].StartCoroutine(m_activeCharacters[1].FadeInFadeOut());

        m_lastRoomAdded.SetResident(m_activeCharacters[0]);
        m_lastRoomAdded.SetVisitor(m_activeCharacters[1]);

        m_rooms.Add(m_lastRoomAdded);
    }

    void Awake()
    {
        Character.StartGame();
        ms_instance = this;
    }

    // Use this for initialization
    void Start()
    {
        PopulateCharacterDatabase();

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        AddARoom();
        while (true)
        {
            MakeHarder();
            m_difficulty++;
            yield return new WaitForEndOfFrame();
            yield return PlayLevel();
            yield return EndLevel();
        }
    }

    private IEnumerator EndLevel()
    {
        m_waitForSpace = true;
        m_intermissionHighScore.text = "" + PlayerPrefs.GetFloat("HighScore", 0.0f);

        foreach (Character c in m_activeCharacters)
        {
            c.ResetAnger();
        }

        yield return ClearAllRooms();

        m_levelText.text = "Level " + m_level++;
        m_intermissionYourScore.text = "Your score: " + m_score;

        if (m_score > PlayerPrefs.GetFloat("HighScore", 0.0f))
        {
            PlayerPrefs.SetFloat("HighScore", m_score);
        }

        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime;
            m_intermissionCanvas.alpha = t;

            yield return new WaitForEndOfFrame();
        }


        while (m_waitForSpace)
        {
            yield return new WaitForEndOfFrame();
        }

        t = 1.0f;

        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            m_intermissionCanvas.alpha = t;

            yield return new WaitForEndOfFrame();
        }

        foreach (Character c in m_activeCharacters)
        {
            c.ResetAnger();
        }

    }

    public bool AnyCharacterHasAppointmentWith(Character character)
    {
        foreach(Character c in m_activeCharacters)
        {
            if (c.GetAppointments().Contains(character))
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator Select(TrumpRoom room)
    {
        m_elevator.transform.position = room.GetElevatorPosition();

        ResetOverTextOnAllRooms(room);
        yield return room.Select();

        if (m_elevatorCharacter != null)
        {
            if (room.CurrentResident != null &&(room.CurrentResident.GetAppointments().Contains(m_elevatorCharacter) || m_elevatorCharacter.GetAppointments().Contains(room.CurrentResident)))
            {
                Character temp = room.CurrentVisitor;
                room.SetVisitor(m_elevatorCharacter);
                SetElevatorCharacter(temp);
            }
            else if (room.CurrentVisitor != null && (room.CurrentVisitor.GetAppointments().Contains(m_elevatorCharacter) || m_elevatorCharacter.GetAppointments().Contains(room.CurrentVisitor)))
            {
                Character temp = room.CurrentResident;
                room.SetResident(m_elevatorCharacter);
                SetElevatorCharacter(temp);

            }
            else if (room.CurrentVisitor != null && room.CurrentVisitor.HasAppointment() || AnyCharacterHasAppointmentWith(room.CurrentVisitor))
            {

                Character temp = room.CurrentVisitor;
                room.SetVisitor(m_elevatorCharacter);
                SetElevatorCharacter(temp);

            }
            else if (room.CurrentResident !=null && room.CurrentResident.HasAppointment() || AnyCharacterHasAppointmentWith(room.CurrentResident))
            {
                Character temp = room.CurrentResident;
                room.SetResident(m_elevatorCharacter);
                SetElevatorCharacter(temp);

            }


            CheckForAnyMeetings();

            //TODO: fill the hotel with words to select rooms to pick people up from
            yield return DropoffAccessRooms();
        }
        else
        {
            if (room.CurrentVisitor != null && room.CurrentVisitor.InMeeting == false)
            {
                SetElevatorCharacter(room.CurrentVisitor);
                room.SetVisitor(null);
            }
            else if (room.CurrentResident != null && room.CurrentResident.InMeeting == false)
            {
                SetElevatorCharacter(room.CurrentResident);
                room.SetResident(null);
            }

            //TODO: fill the hotel with words to select rooms on his agenda or empty rooms to move him to
            yield return DropoffAccessRooms();
        }
    }

    public bool HasSelection(string testWord)
    {
        foreach (TrumpRoom room in m_rooms)
        {
            if (room.GetWord() == testWord && testWord != "")
            {
                room.StartCoroutine(Select(room));
                return true;
            }
        }
        return false;
    }

    private bool LevelInProgress()
    {
        bool inProgress = false;

        foreach (Character character in m_activeCharacters)
        {
            if (character.HasAppointment()) //TODO: don't interrupt this character during a meeting
            {
                inProgress = true;
            }
        }
        return inProgress;
    }

    public void ResetCombo()
    {
        m_combo = 1;
        m_scoreText.text = "Score:" + m_score + " Combo:" + m_combo;

    }
    
    public void IncrementCombo()
    {
        m_combo++;
    }

    private void CheckForAnyMeetings()
    {
        foreach (TrumpRoom room in m_rooms)
        {
            m_score += room.CheckForMeeting();
            m_scoreText.text = "Score:" + m_score + " Combo:" + m_combo * m_combo;
        }
    }

    private IEnumerator PlayLevel()
    {
        //TODO: give each character a agenda of who to talk to.
        foreach (Character character in m_activeCharacters)
        {
            character.GenerateAgenda(m_activeCharacters.Where(x => x != character).ToList());
        }
        
        yield return DropoffAccessRooms();
        while (LevelInProgress())
        {
            CheckForAnyMeetings();

            yield return DropoffAccessRooms();
            yield return new WaitForEndOfFrame();
        }

    }

    private void ResetOverTextOnAllRooms(TrumpRoom exception)
    {
        foreach (TrumpRoom room in m_rooms)
        {
            if (room == exception)
            {
                continue;
            }
            room.ClearOverText();
        }

    }

    private IEnumerator ClearAllRooms()
    {
        foreach (TrumpRoom room in m_rooms)
        {
            yield return room.MakeUnselectable();
        }
    }

    public IEnumerator DropoffAccessRooms()
    {
        foreach (TrumpRoom room in m_rooms)
        {
            if (room.InAMeeting())
            {
                yield return room.MakeUnselectable();
            }
            else if ((room.CurrentResident != null && room.CurrentResident.HasAppointment(m_activeCharacters)) ||
               (room.CurrentVisitor != null && room.CurrentVisitor.HasAppointment(m_activeCharacters)))
            {
                yield return room.MakeSelectable();
            }
            else
            {
                yield return room.MakeUnselectable();
            }
        }
    }

    public void UpdateOverTextOnAll(string testWord)
    {
        foreach (TrumpRoom room in m_rooms)
        {
            room.CheckForSubstring(testWord);
        }
    }

    public bool ValidWord(string testWord)
    {
        foreach (TrumpRoom room in m_rooms)
        {
            room.CheckForSubstring(testWord);
            if (room.GetWord().Length < testWord.Length)
            {
                continue;
            }

            if (room.GetWord().Substring(0, testWord.Length) == testWord)
            {
                return true;
            }
        }
        return false;
    }

    public int GetDifficulty()
    {
        return m_difficulty;
    }

    private void AddARoom()
    {
        m_numRooms++;
        m_lastRoomAdded.SetElevatorShaftSprite(m_middleShaft);
        m_lastRoomAdded = GameObject.Instantiate(mp_room, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.position, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.rotation, transform).GetComponent<TrumpRoom>();
        if (m_numRooms == 1)
        {
            m_lastRoomAdded.SetElevatorShaftSprite(m_bottomShaft);
        }
        else
        {
            m_lastRoomAdded.SetElevatorShaftSprite(m_topShaft);

        }


        m_rooms.Add(m_lastRoomAdded);
    }

    private void MakeHarder()
    {
        //TODO: only add a room when over capacity

        if (m_numRooms < 9 && m_numRooms*2 - 3 < m_numCharacters)
        {
            AddARoom();
        }
        //TODO: pick a unused character at random.


        if (m_numCharacters < 14)
        {
            //Add a new room
            Character newCharacter = m_newCharacterQueue.Pop();
            newCharacter.StartCoroutine(newCharacter.FadeInFadeOut());
            m_numCharacters++;
            m_activeCharacters.Add(newCharacter);

            foreach (TrumpRoom room in m_rooms)
            {
                if (room.CurrentResident == null)
                {
                    room.SetResident(newCharacter);
                    return;
                }

                if (room.CurrentVisitor == null)
                {
                    room.SetVisitor(newCharacter);
                    return;
                }

            }
        }

    }
}
