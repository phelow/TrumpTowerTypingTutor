using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItineraryBubble : MonoBehaviour
{
    private Vector3 m_iconSpawnPoint;
    private float m_iconSpawnIncrementation = .25f;

    [SerializeField]
    private List<GameObject> m_startPoints;

    private List<GameObject> m_characterHeadshots;

    [SerializeField]
    SpriteRenderer m_spriteRenderer;

    [SerializeField]
    private Sprite m_singleBubble;

    [SerializeField]
    private Sprite m_doubleBubble;

    [SerializeField]
    private Sprite m_tripleBubble;
    [SerializeField]
    private Sprite m_quadrupleBubble;
    [SerializeField]
    private Sprite m_quintupleBubble;
    [SerializeField]
    private Sprite m_sextupleBubble;

    [SerializeField]
    private SpringJoint2D m_springjoint;

    [SerializeField]
    private Rigidbody2D m_rigidbody;

    private float m_jitterRatio = 0.0f;

    // Use this for initialization
    void Start()
    {
        m_characterHeadshots = new List<GameObject>();
        ResetSpawnPoint();
        StartCoroutine(Jitter());
    }

    public void SetSpringStart(Rigidbody2D rigidbody)
    {
        m_springjoint.connectedBody = rigidbody;
    }

    private IEnumerator Jitter()
    {
        while (true)
        {
            m_rigidbody.AddForce((new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))).normalized * Mathf.Pow(1+m_jitterRatio,6.5f));

            yield return new WaitForSeconds(Random.Range(.9f - m_jitterRatio, 1.1f - m_jitterRatio));
        }
    }

    void ResetSpawnPoint()
    {
        m_iconSpawnPoint = new Vector3(1.2f, -0.75f);
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
        m_jitterRatio = angerRatio;
        m_spriteRenderer.color = Color.Lerp(Color.white, Color.red, angerRatio);
    }

    public void SetItinerary(List<Character> characters)
    {
        ResetSpawnPoint();
        foreach (GameObject headshots in m_characterHeadshots)
        {
            Destroy(headshots);
        }

        if (characters.Count >= 6)
        {
            m_iconSpawnPoint = m_startPoints[0].transform.position;
            m_spriteRenderer.sprite = m_sextupleBubble;
        }
        else if(characters.Count >= 5)
        {
            m_iconSpawnPoint = m_startPoints[1].transform.position;
            m_spriteRenderer.sprite = m_quintupleBubble;
        }
        else if(characters.Count >= 4)
        {
            m_iconSpawnPoint = m_startPoints[2].transform.position;
            m_spriteRenderer.sprite = m_quadrupleBubble;
        }
        else if (characters.Count >= 3)
        {
            m_iconSpawnPoint = m_startPoints[3].transform.position;
            m_spriteRenderer.sprite = m_tripleBubble;
        }
        else if (characters.Count >= 2)
        {
            m_iconSpawnPoint = m_startPoints[4].transform.position;
            m_spriteRenderer.sprite = m_doubleBubble;
        }
        else if (characters.Count >= 1)
        {
            m_iconSpawnPoint = m_startPoints[5].transform.position;
            m_spriteRenderer.sprite = m_singleBubble;
        }

        m_characterHeadshots = new List<GameObject>();
        foreach (Character c in characters)
        {
            GameObject headshot = GameObject.Instantiate(c.GetIcon(), this.transform);
            headshot.transform.position = m_iconSpawnPoint;
            m_characterHeadshots.Add(headshot);
            m_iconSpawnPoint.x += m_iconSpawnIncrementation;
        }

    }

}
