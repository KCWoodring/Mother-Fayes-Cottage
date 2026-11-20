using UnityEngine;


public class DogRunner : MonoBehaviour
{
    public float speed = 900f;                         
    public Vector2 turnTime = new Vector2(0.4f, 1.5f);  
    public bool spriteFacesRight = false;               

    RectTransform rect, parentRect;
    float dir = 1f, nextTurn;

    void Awake()
    {
        rect = (RectTransform)transform;
        parentRect = (RectTransform)transform.parent;
    }

    void Update()
    {
        if (Time.time >= nextTurn)
        {
            dir = Random.value < 0.5f ? -1f : 1f;
            nextTurn = Time.time + Random.Range(turnTime.x, turnTime.y);
        }

        
        float limit = (parentRect.rect.width - rect.rect.width * Mathf.Abs(rect.localScale.x)) / 2f;

        var p = rect.anchoredPosition;
        p.x += dir * speed * Time.deltaTime;
        if (p.x <= -limit) { p.x = -limit; dir = 1f; }
        if (p.x >= limit) { p.x = limit; dir = -1f; }
        rect.anchoredPosition = p;

        var s = rect.localScale;
        s.x = Mathf.Abs(s.x) * dir * (spriteFacesRight ? 1f : -1f);
        rect.localScale = s;
    }
}