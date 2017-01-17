using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItineraryBubble : MonoBehaviour
{
    private Vector3 m_iconSpawnPoint;
    private float m_iconSpawnIncrementation = .5f;

    private List<GameObject> m_characterHeadshots;

    [SerializeField]
    SpriteRenderer m_spriteRenderer;

    // Use this for initialization
    void Start()
    {
        m_characterHeadshots = new List<GameObject>();
        ResetSpawnPoint();
    }

    void ResetSpawnPoint()
    {
        m_iconSpawnPoint = new Vector3(-1.3f, -0.75f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFade(float fadeRatio)
    {
        m_spriteRenderer.color = Color.Lerp(Color.clear, Color.white, fadeRatio);
    }

    public void SetAnger(float angerRatio)
    {
        m_spriteRenderer.color = Color.Lerp(Color.white, Color.red, angerRatio);
    }

    public void SetItinerary(List<Character> characters)
    {
        ResetSpawnPoint();
        foreach (GameObject headshots in m_characterHeadshots)
        {
            Destroy(headshots);
        }

        m_characterHeadshots = new List<GameObject>();
        foreach (Character c in characters)
        {
            GameObject headshot = GameObject.Instantiate(c.GetIcon(), this.transform);
            headshot.transform.localPosition = m_iconSpawnPoint;
            m_characterHeadshots.Add(headshot);
            m_iconSpawnPoint.x += m_iconSpawnIncrementation;
        }

    }

}
