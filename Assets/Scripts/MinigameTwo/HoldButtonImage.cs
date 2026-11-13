using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButtonImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsHeld { get; private set; }
    private bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
        pressed = true;
    }
    public void OnPointerUp(PointerEventData eventData) => IsHeld = false;
    public void OnPointerExit(PointerEventData eventData) => IsHeld = false;
    public bool PressDown()
    {
        bool result = pressed;
        pressed = false;
        return result;
    }
    private void OnDisable() => IsHeld = false;
}
