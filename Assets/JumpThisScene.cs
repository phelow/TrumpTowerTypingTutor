using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThisScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Fader.Instance.FadeIn(1.0f).LoadLevel(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
