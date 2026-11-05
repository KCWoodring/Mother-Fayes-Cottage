using UnityEngine;
using UnityEngine.EventSystems;

public class MapClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameManagerMiniGame3 manager;

    public void OnPointerEnter(PointerEventData eventData) => manager.OnMapHover(true);
    public void OnPointerExit(PointerEventData eventData) => manager.OnMapHover(false);
    public void OnPointerClick(PointerEventData eventData) => manager.OnMapClicked(eventData);
}
