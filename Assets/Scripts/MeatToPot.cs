using UnityEngine;
using UnityEngine.InputSystem;

public class MeatToPot : MonoBehaviour
{
    private InputAction clickAction;
    private InputAction pointAction;
    private GameObject lastClicked;
    private bool placed;

    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        if (!clickAction.WasPressedThisFrame()) return;

        Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        GameObject clicked = hit.collider.gameObject;

        if (clicked == gameObject && !placed && lastClicked != null)
        {
            Pot pot = lastClicked.GetComponent<Pot>();
            if (pot != null)
            {
                pot.PlaceMeat(gameObject);

                MeatCooking cooking = GetComponent<MeatCooking>();
                if (cooking != null)
                {
                    cooking.SendToPot();
                }

                // Enable chopping once it's in the pot
                MeatChopping chopping = GetComponent<MeatChopping>();
                if (chopping != null)
                {
                    chopping.canChop = true;
                }

                placed = true;
            }
        }

        lastClicked = clicked;
    }
}