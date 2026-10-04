
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trade : MonoBehaviour, IDropHandler
{
    [SerializeField] private RectTransform recTransformSlot;
    [SerializeField] private Production production;
    [SerializeField] private RectTransform ObjectSlot;

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
           
             if (eventData.pointerDrag.CompareTag("Food"))
               {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = ObjectSlot.anchoredPosition;
                production.ProduceItem();
                
                Destroy(eventData.pointerDrag.gameObject);
             }
        }
    }
}
