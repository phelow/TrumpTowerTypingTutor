using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationBubble : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer m_icon;
    [SerializeField]
    private List<Sprite> m_iconChoices;

    [SerializeField]
    private SpriteRenderer m_bubble;
    private float m_fadeOutMin = .5f;
    private float m_fadeOutMax = 2.0f;
    
    // Use this for initialization
	void Start () {
	}

    public IEnumerator FadeOutBubble()
    {
        m_icon.sprite = m_iconChoices[Random.Range(0, m_iconChoices.Count)];

        float lerpTime = 2.0f;
        float t = 0.0f;

        while(t < lerpTime)
        {
            t += Time.deltaTime;

            m_icon.color = Color.Lerp(Color.white, Color.clear, t / lerpTime);

            yield return new WaitForEndOfFrame();
        }

        lerpTime = .5f;
        t = 0.0f;

        while (t < lerpTime)
        {
            t += Time.deltaTime;

            m_bubble.color = Color.Lerp(Color.white, Color.clear, t / lerpTime);

            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
