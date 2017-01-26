using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoom : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    [SerializeField]
    private List<Sprite> m_sprites;
	// Use this for initialization
	void Start () {
        m_spriteRenderer.sprite = m_sprites[Random.Range(0, m_sprites.Count)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
