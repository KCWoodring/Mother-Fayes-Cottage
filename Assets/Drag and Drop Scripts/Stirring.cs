using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stirring : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    public void OnEndDrag(PointerEventData eventData)
    {

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("object is cliked on");

    }

    //when the mouse is on the obbject and clicking , you move the nouse and the pot spinns
    // Spoon?
    //first clicking and draggin like in the what do you call it
    // tutorial that thing it is dragging but not actuall moving so that is what we want.


}
