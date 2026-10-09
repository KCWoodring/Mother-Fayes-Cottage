using UnityEngine;
using UnityEngine.InputSystem;

public class RiceCookerDrag : MonoBehaviour
{
    [SerializeField] private CookingManager manager;
    [SerializeField] private Collider potCollider;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private float hoverOffset = 0.5f;
    [SerializeField] private float dragScale = 1f;

    private InputAction clickAction;
    private InputAction pointAction;
    private Collider source;
    private GameObject ghost;
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
        Vector2 pos = pointAction.ReadValue<Vector2>();

        if (ghost == null)
        {
            if (clickAction.WasPressedThisFrame() && manager.CanAccept(gameObject) && source.Raycast(cam.ScreenPointToRay(pos), out _, 1000f))
                StartDrag(pos);
            return;
        }

        MoveGhost(pos);
        if (clickAction.WasReleasedThisFrame()) EndDrag(pos);
    }

    private void StartDrag(Vector2 pos)
    {
        float riceDepth = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);
        float potDepth = Vector3.Dot(potCollider.bounds.center - cam.transform.position, cam.transform.forward);
        depth = Mathf.Min(riceDepth, potDepth) - hoverOffset;

        ghost = Instantiate(dragPrefab, transform.position, dragPrefab.transform.rotation);
        ghost.transform.localScale *= dragScale;

        foreach (Collider c in ghost.GetComponentsInChildren<Collider>())
            c.enabled = false;

        MoveGhost(pos);
    }

    private void EndDrag(Vector2 pos)
    {
        bool overPot = potCollider.Raycast(cam.ScreenPointToRay(pos), out _, 1000f);

        Destroy(ghost);
        ghost = null;

        if (overPot) manager.AddIngredient(gameObject);
    }

    private void MoveGhost(Vector2 pos)
    {
        ghost.transform.position = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, depth));
    }
}