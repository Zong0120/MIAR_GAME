using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlayerInputAction
{
public class InteractionScope : MonoBehaviour
{
    private static InteractionScope instance;
    private CircleCollider2D circleCollider2D;
    public SpriteRenderer boundarySpriteRenderer;
    public GameObject HintCircle;

    public float InitRadius;
    public float newRadius;
    //private List<GetHint> hintlist = new List<GetHint>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        circleCollider2D = transform.GetComponent<CircleCollider2D>();
        InitRadius = transform.localScale.x;
        boundarySpriteRenderer.enabled = false;
        newRadius = InitRadius;
    }
    public float GetRadiusScale()
    {
        return transform.localScale.x;
    }


    public void AddInteractionScope(float scale)
    {
        Debug.Log("AddInteractionScope:"+scale);
        newRadius +=scale;
        transform.localScale = new Vector3(newRadius, newRadius,newRadius);
        if(newRadius!=InitRadius)boundarySpriteRenderer.enabled = true;
        else if(newRadius == InitRadius)boundarySpriteRenderer.enabled = false;
    }
    public void ReduceInteractionScope(float scale)
    {
        newRadius -=scale;
        transform.localScale = new Vector3(newRadius,newRadius,newRadius);
        if(newRadius == InitRadius)
        {
            boundarySpriteRenderer.enabled = false;
        }
    }
    public void InitializeScope()
    {
        transform.localScale = new Vector3(InitRadius,InitRadius,InitRadius);
        boundarySpriteRenderer.enabled = false;
    }

    public void Searching()
    {
        //StartCoroutine(HintCircleEffect());
        /*
        foreach(GetHint hint in hintlist)
        {
            hint.StartCoroutine(hint.FloatingEffect());
        }
        */
    }
    IEnumerator HintCircleEffect()
    {
        HintCircle.transform.localScale = Vector3.zero;
        HintCircle.SetActive(true);
        Vector3 maxCircle = new Vector3(0.235f,0.235f,0.235f);
        for (float t = 0; t < 1.01; t += Time.deltaTime)
        {
            float normalizedTime = t / 1;
            HintCircle.transform.localScale = Vector3.Lerp(Vector3.zero, maxCircle, t);
            yield return null;
        }
        //Debug.Log("HintCircle:"+HintCircle.transform.localScale);
        HintCircle.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Item"))
        {
            other.GetComponent<GetItem>().enabled = true;
        }
        if(other.CompareTag("Story"))
        {
            try
            {
                other.GetComponent<GetStoryZone>().enabled = true;
            }
            catch
            {
                other.GetComponent<GetStoryChapter>().enabled = true;
            }
        }
        
        /*
        if(other.CompareTag("Texts"))
        {
            other.GetComponent<GetText>().enabled = true;
        }
        if(other.CompareTag("hint"))
        {
            other.GetComponent<GetHint>().enabled = true;
            hintlist.Add(other.GetComponent<GetHint>());
        }
        */
    }
    void OnTriggerExit2D(Collider2D other)
    {
        /*
        if(other.CompareTag("Texts"))
        {
            other.GetComponent<GetText>().enabled = false;

        }
        if(other.CompareTag("hint"))
        {
            hintlist.Remove(other.GetComponent<GetHint>());
            other.GetComponent<GetHint>().enabled = false;
        }
        */
    }

}
}