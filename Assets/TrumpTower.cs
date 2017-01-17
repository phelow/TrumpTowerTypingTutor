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
    private Text m_message;
    bool selectionMade = false;

    public IEnumerator GameOverRoutine()
    {
        m_message.text = "Game Over";
        yield return EndLevel();
        Fader.Instance.FadeIn(1.0f).LoadLevel(1);
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

        foreach(Character character in m_characterPool)
        {
            m_newCharacterQueue.Push(character);
        }


        m_activeCharacters.Add(m_newCharacterQueue.Pop());
        m_activeCharacters.Add(m_newCharacterQueue.Pop());
        m_lastRoomAdded.SetResident(m_activeCharacters[0]);
        m_lastRoomAdded.SetVisitor(m_activeCharacters[1]);

        m_rooms.Add(m_lastRoomAdded);
    }

    void Awake()
    {
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
        m_intermissionHighScore.text = "" + PlayerPrefs.GetFloat("HighScore", 0.0f);

        foreach (Character c in m_activeCharacters)
        {
            c.ResetAnger();
        }

        ClearAllRooms();

        m_levelText.text = "Level " + m_level++ + " completed!";
        m_intermissionYourScore.text = "Your score: " + m_score;

        if (m_score > PlayerPrefs.GetFloat("HighScore", 0.0f))
        {
            PlayerPrefs.SetFloat("HighScore", m_score);
        }

        float t = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime;
            m_intermissionCanvas.alpha = t;

            yield return new WaitForEndOfFrame();
        }

        while (!Input.anyKey)
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

    public void Select(TrumpRoom room)
    {
        m_elevator.transform.position = room.GetElevatorPosition();

        room.Select();
        ClearAllRooms();

        if (m_elevatorCharacter != null)
        {
            if (room.CurrentResident == null)
            {
                room.SetResident(m_elevatorCharacter);
            }
            else if (room.CurrentVisitor == null)
            {
                room.SetVisitor(m_elevatorCharacter);
            }
            else
            {
                Debug.LogError("Hotel overcrowding error");
            }
            SetElevatorCharacter(null);


            CheckForAnyMeetings();

            //TODO: fill the hotel with words to select rooms to pick people up from
            PickupAccessRooms();
        }
        else
        {
            if(room.CurrentVisitor != null)
            {
                SetElevatorCharacter(room.CurrentVisitor);
                room.SetVisitor(null);
            }else if(room.CurrentResident != null)
            {
                SetElevatorCharacter(room.CurrentResident);
                room.SetResident(null);
            }

            //TODO: fill the hotel with words to select rooms on his agenda or empty rooms to move him to
            DropoffAccessRooms();
        }
    }

    public bool HasSelection(string testWord)
    {
        foreach (TrumpRoom room in m_rooms)
        {
            room.ClearOverText();
            if (room.GetWord() == testWord && testWord != "")
            {
                Select(room);
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

    private void CheckForAnyMeetings()
    {
        foreach (TrumpRoom room in m_rooms)
        {
            m_score += room.CheckForMeeting();
            m_scoreText.text = "Score:" + m_score;
        }
    }

    private IEnumerator PlayLevel()
    {
        //TODO: give each character a agenda of who to talk to.
        foreach (Character character in m_activeCharacters)
        {
            character.GenerateAgenda(m_activeCharacters.Where(x => x != character).ToList());
        }

        while (LevelInProgress())
        {
            CheckForAnyMeetings();

            if (m_elevatorCharacter == null)
            {
                PickupAccessRooms();
            }
            else
            {
                DropoffAccessRooms();
            }
            yield return new WaitForEndOfFrame();
        }

    }

    private void ClearAllRooms()
    {
        foreach(TrumpRoom room in m_rooms)
        {
            room.MakeUnselectable();
        }
    }

    private void DropoffAccessRooms()
    {
        foreach (TrumpRoom room in m_rooms.Where(x => x.CurrentVisitor == null && x.CurrentResident == null || (m_elevatorCharacter.GetAppointments().Contains(x.CurrentResident) && x.CurrentVisitor == null)))
        {
            if (room.CurrentResident != null)
            {
                room.MakeSelectable();
            }
            else if (room.CurrentResident == null)
            {
                room.MakeSelectable();
            }
        }
    }    

    private void PickupAccessRooms()
    {
        foreach (TrumpRoom room in m_rooms)
        {
            //TODO: don't pick people up if they're in a meeting

            if (room.CurrentVisitor != null && room.CurrentVisitor.InMeeting == false)
            {
                room.MakeSelectable();
            }
            else if (room.CurrentResident != null && room.CurrentResident.HasAppointment(m_activeCharacters) && room.CurrentResident.InMeeting == false)
            {
                room.MakeSelectable();
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
        foreach(TrumpRoom room in m_rooms)
        {
            room.CheckForSubstring(testWord);
            if(room.GetWord().Length < testWord.Length)
            {
                continue;
            }

            if(room.GetWord().Substring(0,testWord.Length) == testWord)
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
        m_lastRoomAdded = GameObject.Instantiate(mp_room, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.position, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.rotation, transform).GetComponent<TrumpRoom>();
        m_rooms.Add(m_lastRoomAdded);
    }

    private void MakeHarder()
    {
        //TODO: only add a room when over capacity

        if (m_numRooms < 9)
        {
            AddARoom();
        }
        //TODO: pick a unused character at random.


        if (m_numCharacters < 12)
        {
            //Add a new room
            Character newCharacter = m_newCharacterQueue.Pop();
            m_numCharacters++;
            m_activeCharacters.Add(newCharacter);

            foreach (TrumpRoom room in m_rooms)
            {
                if(room.CurrentResident == null)
                {
                    room.SetResident(newCharacter);
                    return;
                }

                if(room.CurrentVisitor == null)
                {
                    room.SetVisitor(newCharacter);
                    return;
                }

            }
        }

    }
}
