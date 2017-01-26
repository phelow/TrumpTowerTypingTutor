using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistakeEffect : MonoBehaviour {
    [SerializeField]
    private CanvasGroup m_mistakesCanvas;

    public static MistakeEffect ms_instance;
	// Use this for initialization
	void Start () {
        ms_instance = this;
    }

    public IEnumerator MakeMistake()
    {
        TrumpTower.ms_instance.ResetCombo();
        float t = 0.0f;

        float lerpTime = .08f;

        while (t < lerpTime)
        {
            t += Time.deltaTime;
            m_mistakesCanvas.alpha = Mathf.Lerp(0.0f, 0.4f, t / lerpTime);

            yield return new WaitForEndOfFrame();
        }

        t = 0.0f;

        while (t < lerpTime)
        {
            t += Time.deltaTime;
            m_mistakesCanvas.alpha = Mathf.Lerp(0.4f, 0.0f, t / lerpTime);

            yield return new WaitForEndOfFrame();
        }
    }
}
