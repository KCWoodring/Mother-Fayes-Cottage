using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] public Canvas canvas;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    private void Awake()
    {
            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        
        canvas = GetComponentInParent<Canvas>();


    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("object is not being dragged");
        canvasGroup.blocksRaycasts = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("object is being dragged");
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("object is ready to move");
        rectTransform.anchoredPosition += eventData.delta/ canvas.scaleFactor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("object is cliked on");
    }


}

