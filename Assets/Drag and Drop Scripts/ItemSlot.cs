using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private RectTransform recTransformSlot;
    [SerializeField]private Production production;
    
    private void Awake()
    {
        recTransformSlot = GetComponent<RectTransform>();
        Production production = GetComponent<Production>();
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("dropped item");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = recTransformSlot.anchoredPosition;
            // if (eventData.pointerDrag.CompareTag("Knife"))
            //{
            //   production.ProduceItem();
            // }
            eventData.pointerDrag = null;
        }
    }
}


