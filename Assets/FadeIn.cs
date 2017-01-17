using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Fader.Instance.FadeOut(1.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
