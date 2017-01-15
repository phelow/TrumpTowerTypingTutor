using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private string m_currentText = "";

    [SerializeField]
    private TextMesh m_selectionText;

    void TryToVerify()
    {
        //TODO: ReAddWord 
        if (TrumpTower.ms_instance.HasSelection(m_currentText))
        {
            m_currentText = "";
            m_selectionText.text = m_currentText;
        }
    }

    void TryToAdd(string nextLetter)
    {
        string potentialNewWord = m_currentText + nextLetter;        
        if (!TrumpTower.ms_instance.ValidWord(potentialNewWord))
        {
            
            return;
        }
        m_currentText += nextLetter;

        m_selectionText.text = m_currentText;


    }

    void TryToBackSpace()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            TryToVerify();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TryToBackSpace();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            TryToAdd("a");

        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            TryToAdd("b");

        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            TryToAdd("c");

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TryToAdd("d");

        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            TryToAdd("e");

        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            TryToAdd("f");

        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TryToAdd("g");

        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            TryToAdd("h");

        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            TryToAdd("i");

        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            TryToAdd("j");

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            TryToAdd("k");

        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            TryToAdd("l");

        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            TryToAdd("m");

        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            TryToAdd("n");

        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            TryToAdd("o");

        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            TryToAdd("p");

        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            TryToAdd("q");

        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            TryToAdd("r");

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TryToAdd("s");

        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            TryToAdd("t");

        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            TryToAdd("u");

        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            TryToAdd("v");

        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            TryToAdd("w");

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            TryToAdd("x");

        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            TryToAdd("y");

        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            TryToAdd("z");

        }

    }
}
