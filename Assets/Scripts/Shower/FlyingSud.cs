using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class FlyingSud : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float startScale = 0.1f;
    [SerializeField] private float endScale = 1.2f;
    [SerializeField] private float growTime = 0.55f;
    [SerializeField] private float stickTime = 2.5f;   
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float popTime = 0.12f;

    private CanvasGroup group;
    private float t;
    private bool popping;
    private float popStartScale;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        transform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        t += Time.deltaTime;

        if (popping)
        {
            float p = Mathf.Clamp01(t / popTime);
            transform.localScale = Vector3.one * Mathf.Lerp(popStartScale, popStartScale * 1.3f, p);
            group.alpha = 1f - p;
            if (p >= 1f) Destroy(gameObject);
            return;
        }

        if (t < growTime)
        {
           
            float p = t / growTime;
            p = 1f - (1f - p) * (1f - p);
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, p);
        }
        else
        {
            transform.localScale = Vector3.one * endScale;

            float fadeStart = growTime + stickTime;
            if (t > fadeStart)
            {
                group.alpha = 1f - Mathf.Clamp01((t - fadeStart) / fadeTime);
                if (t >= fadeStart + fadeTime) Destroy(gameObject);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (popping) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;

        popping = true;
        t = 0f;
        popStartScale = transform.localScale.x;
        group.blocksRaycasts = false; 
    }
}