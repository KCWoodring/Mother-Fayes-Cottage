using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private RectTransform recTransformSlot;
    [SerializeField]private Production production;

    [Header("Cooking")]
    [SerializeField] private CountdownTimer meatTimer;
    [SerializeField] private string meatTag = "Food";

    private bool meatIsCooked;

    private void Awake()
    {
        recTransformSlot = GetComponent<RectTransform>();
        Production production = GetComponent<Production>();
        
    }
    void Update()
    {
        if (!meatIsCooked && meatTimer != null && !meatTimer.isRunning && meatTimer.timeRemaining <= 0f)//calls when timer runs out
        {
            meatIsCooked = true;
            MeatCooked();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("dropped item");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = recTransformSlot.anchoredPosition;
            if (eventData.pointerDrag.CompareTag(meatTag))//Activates meat timer
            {
                meatTimer.gameObject.SetActive(true);
                meatIsCooked = true;
                meatTimer.StartTimer();
            }
            // if (eventData.pointerDrag.CompareTag("Knife"))
            //{
            //   production.ProduceItem();
            // }
            eventData.pointerDrag = null;
        }
    }
    private void MeatCooked()
    {
        Debug.Log("Meat is cooked!");
    }
}


