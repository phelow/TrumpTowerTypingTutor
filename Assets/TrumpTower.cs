using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrumpTower : MonoBehaviour
{
    public static TrumpTower ms_instance;

    private int m_numRooms = 1;
    private int m_numCharacters = 1;

    private int m_difficulty = 0;

    [SerializeField]
    private GameObject mp_room;

    [SerializeField]
    private TrumpRoom m_lastRoomAdded;

    private Stack<Character> m_characterBase;

    public List<TrumpRoom> m_rooms;

    private List<Character> m_activeCharacters;

    private Character m_elevatorCharacter = null;

    [SerializeField]
    private TextMesh m_elevatorText;
    private bool m_waitingForSelection = false;

    private void SetElevatorCharacter(Character elevatorCharacter)
    {
        m_elevatorCharacter = elevatorCharacter;

        if (m_elevatorCharacter == null)
        {
            m_elevatorText.text = "empty";
        }
        else
        {
            m_elevatorText.text = elevatorCharacter.Name;

        }
    }

    bool selectionMade = false;
    private void PopulateCharacterDatabase()
    {
        m_rooms = new List<TrumpRoom>();
        m_activeCharacters = new List<Character>();
        m_characterBase = new Stack<Character>();
        m_characterBase.Push(new Character("Trump"));
        m_characterBase.Push(new Character("Putin"));
        m_characterBase.Push(new Character("Pence"));
        m_characterBase.Push(new Character("Carson"));
        m_characterBase.Push(new Character("Bannon"));
        m_characterBase.Push(new Character("Perry"));
        m_characterBase.Push(new Character("Price"));
        m_characterBase.Push(new Character("Mattis"));
        m_characterBase.Push(new Character("Zinke"));
        m_characterBase.Push(new Character("Tillerson"));
        m_characterBase.Push(new Character("Mnuchin"));
        m_characterBase.Push(new Character("Sessions"));
        m_characterBase.Push(new Character("Ross"));
        m_characterBase.Push(new Character("Puzder"));
        m_characterBase.Push(new Character("Price"));
        m_activeCharacters.Add(m_characterBase.Pop());
        m_activeCharacters.Add(m_characterBase.Pop());
        m_lastRoomAdded.SetResident(m_activeCharacters[0]);
        m_lastRoomAdded.SetVisitor(m_activeCharacters[1]);

        m_rooms.Add(m_lastRoomAdded);
        AddARoom();
    }


    // Use this for initialization
    void Start()
    {
        ms_instance = this;
        PopulateCharacterDatabase();

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {

            AddARoom();
            m_difficulty++;
            yield return new WaitForEndOfFrame();
            yield return PlayLevel();
        }
    }

    public void Select(TrumpRoom room)
    {
        if (m_elevatorCharacter != null)
        {
            if (room.CurrentResident == null)
            {
                room.SetResident(m_elevatorCharacter);
                SetElevatorCharacter(null);
            }
            else if (room.CurrentVisitor == null)
            {
                room.SetVisitor(m_elevatorCharacter);
                SetElevatorCharacter(null);
            }
            else
            {
                Debug.LogError("Hotel overcrowding error");
            }

            CheckForAnyMeetings();

            //TODO: fill the hotel with words to select rooms to pick people up from
            PickupAccessRooms();
        }
        else
        {
            if(m_elevatorCharacter != null)
            {
                Debug.LogError("No one is in the elevator");
            }

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
            if (room.GetWord() == testWord)
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
            room.CheckForMeeting();
        }
    }

    private IEnumerator PlayLevel()
    {
        //TODO: give each character a agenda of who to talk to.
        foreach (Character character in m_activeCharacters)
        {
            character.GenerateAgenda(m_activeCharacters.Where(x => x != character).ToList());
        }

        CheckForAnyMeetings();

        if (m_elevatorCharacter == null)
        {
            PickupAccessRooms();
        }
        else
        {
            DropoffAccessRooms();
        }

        while (LevelInProgress())
        {
            yield return WaitForSelection();
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
        ClearAllRooms();

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
        m_waitingForSelection = true;
    }    

    private void PickupAccessRooms()
    {
        ClearAllRooms();

        foreach (TrumpRoom room in m_rooms)
        {
            //TODO: don't pick people up if they're in a meeting

            if (room.CurrentVisitor != null && room.CurrentVisitor.InMeeting == false)
            {
                room.MakeSelectable();
            }
            else if (room.CurrentResident != null && room.CurrentResident.HasAppointment() && room.CurrentResident.InMeeting == false)
            {
                room.MakeSelectable();
            }
        }
        m_waitingForSelection = false;
    }

    private IEnumerator WaitForSelection()
    {
        selectionMade = false;
        while (selectionMade == false)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public bool ValidWord(string testWord)
    {
        foreach(TrumpRoom room in m_rooms)
        {
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

    public void ExitMeeting(TrumpRoom room)
    {
        if (m_waitingForSelection)
        {
            room.MakeSelectable();
        }
    }

    public int GetDifficulty()
    {
        return m_difficulty;
    }

    private void AddARoom()
    {
        //TODO: only add a room when over capacity

        if (m_numRooms < 9)
        {
            m_numRooms++;
            m_lastRoomAdded = GameObject.Instantiate(mp_room, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.position, m_lastRoomAdded.GetNextRoomSpawnPoint().transform.rotation, transform).GetComponent<TrumpRoom>();
            m_rooms.Add(m_lastRoomAdded);
        }
        //TODO: pick a unused character at random.


        if (m_numCharacters < 14)
        {
            //Add a new room
            Character newCharacter = m_characterBase.Pop();
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
