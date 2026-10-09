using UnityEngine;
using UnityEngine.InputSystem;

public class DragFruit : MonoBehaviour
{
    [SerializeField] private CookingManager manager;
    [SerializeField] private Collider potCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private float hoverOffset = 0.5f;

    private InputAction clickAction;
    private InputAction pointAction;
    private Collider source;
    private bool dragging;
    private Vector3 startPosition;
    private float depth;

    private void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        source = GetComponent<Collider>();
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        Vector2 position = pointAction.ReadValue<Vector2>();

        if (!dragging)
        {
            if (clickAction.WasPressedThisFrame() && manager.CanAccept(gameObject) && source.Raycast(cam.ScreenPointToRay(position), out _, 1000f))
                StartDrag(position);
            return;
        }

        MoveToCursor(position);
        if (clickAction.WasReleasedThisFrame()) EndDrag(position);
    }

    private void StartDrag(Vector2 position)
    {
        dragging = true;
        startPosition = transform.position;

        float selfDepth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);
        float potDepth = Vector3.Dot(potCollider.bounds.center - cam.transform.position, cam.transform.forward);
        depth = Mathf.Min(selfDepth, potDepth) - hoverOffset;

        source.enabled = false;
        MoveToCursor(position);
    }

    private void EndDrag(Vector2 position)
    {
        dragging = false;
        bool overPot = potCollider.Raycast(cam.ScreenPointToRay(position), out _, 1000f);

        if (overPot)
        {
            manager.AddIngredient(gameObject);
            gameObject.SetActive(false);
            return;
        }

        transform.position = startPosition;
        source.enabled = true;
    }

    private void MoveToCursor(Vector2 position)
    {
        transform.position = cam.ScreenToWorldPoint(new Vector3(position.x, position.y, depth));
    }
}