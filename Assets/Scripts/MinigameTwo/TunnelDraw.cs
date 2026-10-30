using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TunnelDraw : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;   // left entrance to right exit, in order
    [SerializeField] private float reachRadius = 0.8f;
    [SerializeField] private float tolerance = 1f;     // how far from the path you can stray'
    [SerializeField] private float dogSpeed = 8f;
    [SerializeField] private float exitDistance = 1.5f;
    [SerializeField] private float dogOffsetDistance = 0.5f;

    [SerializeField] private Transform dog;
    [SerializeField] private DogMovement dogMovement;
    private CharacterController dogCharacterController;
    [SerializeField] private CourseCamera courseCamera;

    [SerializeField] private LineRenderer guideLine;   // shows the route
    [SerializeField] private LineRenderer drawnLine;   // the player's line
    [SerializeField] private float lineHeight = 3f;

    public UnityEvent onMistake;
    public UnityEvent onComplete;

    private InputAction clickAction, pointAction;
    private bool active, drawing;
    private bool firstGo;
    private int next;   // the waypoint to reach next

    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

        dogCharacterController = dog.GetComponent<CharacterController>();

        guideLine.useWorldSpace = true;
        guideLine.positionCount = waypoints.Length;
        for (int i = 0; i < waypoints.Length; i++)
        {
            guideLine.SetPosition(i, waypoints[i].position + Vector3.up * lineHeight);
        }
            
        guideLine.gameObject.SetActive(false);
        drawnLine.gameObject.SetActive(false);
    }

    public void BeginMinigame()
    {
        active = true;
        drawing = false;
        firstGo = true;
        next = 1;
        dogMovement.SetControl(false);
        guideLine.gameObject.SetActive(true);
        drawnLine.gameObject.SetActive(true);
        drawnLine.positionCount = 0;
    }

    private void Update()
    {
        if (!active) return;

        if (firstGo)
        {
            Vector3 targetPosition = waypoints[next - 1].position + DogOffset();
            Vector3 directionToTarget = targetPosition - dog.position;
            directionToTarget.y = 0f;
            dogCharacterController.Move(Vector3.ClampMagnitude(directionToTarget, dogSpeed * Time.deltaTime));
        }

        Vector3 mouse = MousePoint();

        if (!drawing)
        {
            if (clickAction.WasPressedThisFrame() && HorizontalDistance(mouse, waypoints[0].position) < reachRadius)
            {
                drawing = true;
                next = 1;
                drawnLine.positionCount = 1;
                drawnLine.SetPosition(0, waypoints[0].position + Vector3.up * lineHeight);
                firstGo = false;
            }
            return;
        }

        // let go or wander off the path: reset
        if (!clickAction.IsPressed() || DistanceToSegment(mouse) > tolerance)
        {
            if (clickAction.IsPressed()) onMistake.Invoke();
            ResetLine();
            return;
        }

        // add a point to the drawn line
        Vector3 position = mouse;
        int number = drawnLine.positionCount;
        if (number == 0 || Vector3.Distance(drawnLine.GetPosition(number - 1), position) > 0.05f)
        {
            drawnLine.positionCount = number + 1;
            drawnLine.SetPosition(number, position);
        }

        // reached the next waypoint
        if (HorizontalDistance(mouse, waypoints[next].position) < reachRadius)
        {
            next++;
            if (next >= waypoints.Length) Finish();
        }
    }
    private void ResetLine()
    {
        drawing = false;
        next = 1;
        drawnLine.positionCount = 0;
    }
    private void Finish()
    {
        active = false;
        drawing = false;
        guideLine.gameObject.SetActive(false);
        drawnLine.gameObject.SetActive(false);

        Vector3 directionToEnd = ExitPoint() - dog.position;
        directionToEnd.y = 0f;
        dogCharacterController.Move(directionToEnd);

        dogMovement.SetControl(true);
        courseCamera.ReturnToFollow();
        onComplete.Invoke();
    }
    private Vector3 ExitPoint()
    {
        Vector3 last = waypoints[waypoints.Length - 1].position;
        Vector3 along = last - waypoints[waypoints.Length - 2].position;
        along.y = 0f;
        return last + along.normalized * exitDistance + DogOffset();
    }
    private Vector3 MousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        Plane drawingPlane = new Plane(Vector3.up, waypoints[0].position + Vector3.up * lineHeight);
        drawingPlane.Raycast(ray, out float distanceAlongRay);
        return ray.GetPoint(distanceAlongRay);
    }
    private float DistanceToSegment(Vector3 position)
    {
        Vector3 a = waypoints[next - 1].position, b = waypoints[next].position;
        a.y = b.y = position.y;
        Vector3 ab = b - a;
        float time = Mathf.Clamp01(Vector3.Dot(position - a, ab) / ab.sqrMagnitude);
        return Vector3.Distance(position, a + ab * time);
    }
    private float HorizontalDistance(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    private Vector3 DogOffset()
    {
        Vector3 screenLeft = -Vector3.ProjectOnPlane(Camera.main.transform.up, Vector3.up).normalized;
        return screenLeft * dogOffsetDistance;
    }
    private void OnDrawGizmos()
    {
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.15f);
            if (i > 0 && waypoints[i - 1] != null)
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
        }
    }
}
