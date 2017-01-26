using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public static CameraShake ms_instance;
    [SerializeField]
    private Rigidbody2D m_cameraRigidbody;

    private float m_maxForce = 600.0f;
    private float m_minForce = 500.0f;

    private float m_maxMegaForce = 6000.0f;
    private float m_minMegaForce = 5000.0f;
    // Use this for initialization
    void Start () {
        ms_instance = this;
    }

    public void Shake()
    {
        m_cameraRigidbody.AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * Random.Range(m_minForce, m_maxForce));
    }

    public void MegaShake()
    {
        m_cameraRigidbody.AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * Random.Range(m_minMegaForce, m_maxMegaForce));

    }
}
