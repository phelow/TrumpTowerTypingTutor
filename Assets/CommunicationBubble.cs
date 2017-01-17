using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationBubble : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer m_icon;
    [SerializeField]
    private SpriteRenderer m_bubble;
    private float m_fadeOutMin = .5f;
    private float m_fadeOutMax = 2.0f;
    
    // Use this for initialization
	void Start () {
        StartCoroutine(FadeOutBubble());
	}

    private IEnumerator FadeOutBubble()
    {
        float lerpTime = Random.Range(m_fadeOutMin, m_fadeOutMax);
        float t = 0.0f;

        while(t < lerpTime)
        {
            t += Time.deltaTime;

            m_icon.color = Color.Lerp(Color.white, Color.clear, t / lerpTime);

            yield return new WaitForEndOfFrame();
        }

        lerpTime = Random.Range(m_fadeOutMin, m_fadeOutMax);
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
