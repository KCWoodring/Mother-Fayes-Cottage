using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TreatDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform mouthTarget;
    [SerializeField] private PuppyAttention puppy;
    [SerializeField] private float returnTime = 0.25f;
    private FlashTintUI flash;

    private RectTransform rectangle;
    private Canvas canvas;
    private CanvasGroup group;
    private Vector2 homePosition;

    private void Awake()
    {
        rectangle = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
        group = GetComponent<CanvasGroup>();
        flash = GetComponent<FlashTintUI>();
        homePosition = rectangle.anchoredPosition;
        Hide();
    }
    public void Show()
    {
        StopAllCoroutines();
        rectangle.anchoredPosition = homePosition;
        group.alpha = 1f;
        group.blocksRaycasts = true;
        gameObject.SetActive(true);
        if (flash != null) flash.SetFlashing(true);
    }
    public void Hide()
    {
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (flash != null) flash.SetFlashing(false);
        group.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectangle.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        group.blocksRaycasts = true;

        if (OverMouth())
        {
            Hide();
            puppy.FeedPuppy();
        }
        else
        {
            StartCoroutine(ReturnHome());
        }
    }
    private bool OverMouth()
    {
        return ScreenRectangle(rectangle).Overlaps(ScreenRectangle(mouthTarget));
    }
    private IEnumerator ReturnHome()
    {
        Vector2 from = rectangle.anchoredPosition;
        for (float t = 0; t < 1f; t += Time.deltaTime / returnTime)
        {
            rectangle.anchoredPosition = Vector2.Lerp(from, homePosition, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        rectangle.anchoredPosition = homePosition;
    }
    private static Rect ScreenRectangle(RectTransform rt)
    {
        Vector3[] c = new Vector3[4];
        rt.GetWorldCorners(c);
        return new Rect(c[0].x, c[0].y, c[2].x - c[0].x, c[2].y - c[0].y);
    }
}
