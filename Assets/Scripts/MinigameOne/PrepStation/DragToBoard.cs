using UnityEngine;
using UnityEngine.InputSystem;

public class DragToBoard : MonoBehaviour
{
    [SerializeField] private PrepStationManager prepStationManager;
    [SerializeField] private Collider boardCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private float hoverOffset = 0.5f;
    [SerializeField] private bool isPumpkin = true;

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
            if (!prepStationManager.enabled || prepStationManager.cuttingActive) return;

            if (clickAction.WasPressedThisFrame() && source.Raycast(cam.ScreenPointToRay(position), out _, 1000f))
                StartDrag(position);
            return;
        }

        if (!prepStationManager.enabled)
        {
            CancelDrag();
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
        float boardDepth = Vector3.Dot(boardCollider.bounds.center - cam.transform.position, cam.transform.forward);
        depth = Mathf.Min(selfDepth, boardDepth) - hoverOffset;

        source.enabled = false;
        MoveToCursor(position);
    }

    private void EndDrag(Vector2 position)
    {
        dragging = false;
        source.enabled = true;

        if (boardCollider.Raycast(cam.ScreenPointToRay(position), out _, 1000f))
        {
            transform.position = boardCollider.transform.position + new Vector3(0, 1, -.05f);
            prepStationManager.cuttingActive = true;

            if (isPumpkin) prepStationManager.SelectPumpkinToCut();
            else prepStationManager.SelectHoneyDewToCut();

            enabled = false;
            return;
        }

        transform.position = startPosition;
    }

    private void CancelDrag()
    {
        dragging = false;
        source.enabled = true;
        transform.position = startPosition;
    }

    private void MoveToCursor(Vector2 position)
    {
        transform.position = cam.ScreenToWorldPoint(new Vector3(position.x, position.y, depth));
    }
}