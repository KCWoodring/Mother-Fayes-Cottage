using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DogMovement : MonoBehaviour
{
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float turnSpeed = 540f;
    [SerializeField] private float gravity = -20f;

    [Header("Mouse movement")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float stopDistance = 0.3f;

    private CharacterController controller;
    private InputAction pointAction;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private Vector3 destination;
    private bool hasDestination;
    private bool waitForRelease;

    public bool HasControl { get; private set; } = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        pointAction = InputSystem.actions.FindAction("Point");
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!HasControl) return;

        ReadMouse();

        // direction toward the clicked spot
        Vector3 target = Vector3.zero;
        if (hasDestination)
        {
            Vector3 toDestination = destination - transform.position;
            toDestination.y = 0f;

            if (toDestination.magnitude <= stopDistance)
                hasDestination = false;
            else
                target = toDestination.normalized;
        }

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, target * runSpeed, acceleration * Time.deltaTime);

        // face the direction of travel
        if (horizontalVelocity.sqrMagnitude > 0.05f)
        {
            Quaternion look = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // gravity
        verticalVelocity = controller.isGrounded ? -1f : verticalVelocity + gravity * Time.deltaTime;

        controller.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void ReadMouse()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (waitForRelease)
        {
            if (!mouse.leftButton.isPressed) waitForRelease = false;
            return;
        }

        if (!mouse.leftButton.isPressed) return;

        // ignore clicks on UI buttons
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCamera.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
        {
            destination = hit.point;
            hasDestination = true;
        }
    }

    // When the minigame takes over control
    public void SetControl(bool on)
    {
        HasControl = on;
        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;
        hasDestination = false;

        // if control comes back while the button is still held, ignore it until released
        waitForRelease = on;
    }
}
