using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour {
    [SerializeField]
    private Text m_text;
	// Use this for initialization
	void Start () {
        m_text.text = "Huge Score:" + PlayerPrefs.GetFloat("HighScore", 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
