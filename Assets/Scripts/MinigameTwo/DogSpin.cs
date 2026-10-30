using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DogSpin : MonoBehaviour
{
    public enum SpinDirection { Any, Clock, CounterClock }

    [SerializeField] private Transform dog;
    [SerializeField] private DogMovement dogMovement;
    [SerializeField] private CourseCamera courseCamera;
    [SerializeField] private Camera cam;

    [Header("Platform")]
    [SerializeField] private Transform spinPoint;
    [SerializeField] private Collider platformZone;
    [SerializeField] private Transform exitPoint;

    [Header("Spinning")]
    [SerializeField] private float laps = 3f;
    [SerializeField] private SpinDirection direction = SpinDirection.Any;
    [SerializeField] private float deadZonePixels = 30f;

    public UnityEvent onComplete;

    private CharacterController dogCharacterController;
    private InputAction clickAction, pointAction;
    private bool active, spinning;
    private float progress, lastAngle;
    private int lockedSign;

    public float SpinProgress => Mathf.Clamp01(progress / (360f * laps));

    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        dogCharacterController = dog.GetComponent<CharacterController>();
        if (cam == null) cam = Camera.main;
    }
    public void BeginMinigame()
    {
        active = true;
        spinning = false;
        progress = 0f;
        lockedSign = 0;

        dogMovement.SetControl(false);
        Teleport(spinPoint.position);
    }

    private void Update()
    {
        if (!active) return;

        Vector2 position = pointAction.ReadValue<Vector2>();

        if (!spinning)
        {
            if (clickAction.WasPressedThisFrame() && PressedOnPlatform(position))
            {
                spinning = true;
                lastAngle = GetAngle(position);
            }
            return;
        }

        if (!clickAction.IsPressed())
        {
            spinning = false;
            return;
        }

        Spin(position);
    }

    private void Spin(Vector2 position)
    {
        if ((position - PlatformScreenPos()).magnitude < deadZonePixels) return;

        float angle = GetAngle(position);
        float delta = Mathf.DeltaAngle(lastAngle, angle);
        lastAngle = angle;
        dog.Rotate(Vector3.up, -delta, Space.World);

        int sign = delta > 0f ? 1 : -1;   // +1 = counter clockwise on screen
        if (direction == SpinDirection.CounterClock) lockedSign = 1;
        else if (direction == SpinDirection.Clock) lockedSign = -1;
        else if (lockedSign == 0 && Mathf.Abs(delta) > 0.01f) lockedSign = sign;

        bool correct = sign == lockedSign;
        progress = Mathf.Max(0f, progress + (correct ? Mathf.Abs(delta) : -Mathf.Abs(delta)));

        if (progress >= 360f * laps) Finish();
    }
    private void Finish()
    {
        active = false;
        spinning = false;

        Teleport(exitPoint.position);
        dog.rotation = Quaternion.Euler(0f, exitPoint.eulerAngles.y, 0f);

        dogMovement.SetControl(true);
        courseCamera.ReturnToFollow();
        onComplete.Invoke();
    }
    private void Teleport(Vector3 position)
    {
        dogCharacterController.enabled = false;
        dog.position = position;
        dogCharacterController.enabled = true;
    }

    private bool PressedOnPlatform(Vector2 position)
    {
        if (platformZone == null) return true;
        return platformZone.Raycast(cam.ScreenPointToRay(position), out _, 1000f);
    }

    private Vector2 PlatformScreenPos() => cam.WorldToScreenPoint(spinPoint.position);

    private float GetAngle(Vector2 screenPosition)
    {
        Vector2 v = screenPosition - PlatformScreenPos();
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
}
