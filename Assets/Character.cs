using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {
    public string Name { get; set; }

    private List<Character> m_agenda;

    public bool InMeeting;
    
    public void FulfillAppointmnet(Character character)
    {
        m_agenda.Remove(character);
    }

    public List<Character> GetAppointments()
    {
        return m_agenda;
    }

    public Character(string name)
    {
        m_agenda = new List<Character>();
        Name = name;
    }

    public bool HasAppointment()
    {
        return m_agenda.Count > 0;
    }

    public void MeetWith(Character character)
    {
        if (m_agenda.Contains(character))
        {
            m_agenda.Remove(character);
        }
    }

    public void GenerateAgenda(List<Character> m_characterBase)
    {
        m_agenda = new List<Character>();
        int totalAgenda = TrumpTower.ms_instance.GetDifficulty();
        for (int i = 0; i < Mathf.Max(3,totalAgenda); i++)
        {
            m_agenda.Add(m_characterBase[Random.Range(0, m_characterBase.Count)]);
        }
    }
}
