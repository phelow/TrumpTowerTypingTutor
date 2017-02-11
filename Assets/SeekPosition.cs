using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekPosition : MonoBehaviour {
    [SerializeField]
    private GameObject m_nextSlot;

    [SerializeField]
    private GameObject m_unselectedBlock;

    [SerializeField]
    private Rigidbody2D m_unselectedRigidbody;

    [SerializeField]
    private Rigidbody2D m_selectedRigidbody;

    [SerializeField]
    private GameObject m_selectedBlock;

    bool selected = true;

    [SerializeField]
    private TextMesh m_unselectedText;
    [SerializeField]
    private TextMesh m_selectedText;

    [SerializeField]
    private AnimationCurve m_animationCurve;

    private Vector3 m_targetPosition;
    
    private void KickAway(Rigidbody2D kickBody, GameObject kickObject)
    {
        kickBody.AddForce((kickObject.transform.position - Camera.main.transform.position).normalized * 3000.0f);
    }

    public IEnumerator Cycleblocks(Rigidbody2D rb_kickAway, GameObject go_kickAway, GameObject go_lerpIn, Rigidbody2D rb_lerpIn)
    {

        go_lerpIn.transform.localPosition = new Vector3(Random.Range(-1.0f, 1.0f) * 20.0f, Random.Range(-1.0f, 1.0f), 0).normalized * 20.0f;
        //TODO: add random force to the selected block
        KickAway(rb_kickAway, go_kickAway);
        //lerp unselected block to the core position

        float t = 0.0f;
        float lerpTime = Random.Range(0.1f, .15f);
        Vector3 originalPosition = go_lerpIn.transform.position;

        rb_lerpIn.velocity *= 0;

        while (t <= lerpTime)
        {
            t += Time.deltaTime;
            go_lerpIn.transform.position = Vector3.Lerp(originalPosition, m_targetPosition, m_animationCurve.Evaluate(t / lerpTime));
            yield return new WaitForEndOfFrame();
        }
    }
    
	// Use this for initialization
	public IEnumerator Unselect()
    {
        if (!selected)
        { 
            yield break;
        }
        selected = false;
        yield return Cycleblocks(m_selectedRigidbody, m_selectedBlock, m_unselectedBlock, m_unselectedRigidbody);
    }


    // Use this for initialization
    public IEnumerator Select()
    {
        if (selected)
        {
            yield break;
        }
        selected = true;
        yield return Cycleblocks(m_unselectedRigidbody, m_unselectedBlock, m_selectedBlock, m_selectedRigidbody);
    }

    public void SetLetter(char character)
    {
        m_selectedText.text = "" + character;
        m_unselectedText.text = "" + character;
    }

    public void SetTarget(GameObject target)
    {
        m_targetPosition = target.transform.position;
        m_selectedBlock.transform.localPosition = new Vector3(Random.Range(-1.0f, 1.0f) * 20.0f, Random.Range(-1.0f, 1.0f), 0).normalized * 20.0f;
        m_unselectedBlock.transform.localPosition = new Vector3(Random.Range(-1.0f, 1.0f) * 20.0f, Random.Range(-1.0f, 1.0f), 0).normalized * 20.0f;
    }

    public IEnumerator End()
    {
        KickAway(m_selectedRigidbody,m_selectedBlock);
        KickAway(m_unselectedRigidbody,m_unselectedBlock);
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }

    public GameObject GetNextSlot()
    {
        return m_nextSlot;
    }

}
