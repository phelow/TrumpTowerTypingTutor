using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnyKeyToStart : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            Fader.Instance.FadeIn(1.0f).LoadLevel(2);
        }
	}
}
