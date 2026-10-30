using UnityEngine;

public class CourseCamera : MonoBehaviour
{
    [SerializeField] private Transform dog;

    [Header("Follow view")]
    [SerializeField] private float distanceBehind = 6f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float lookHeight = 1f;
    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private float rotationSmoothSpeed = 5f;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1.5f;

    private Transform obstacleView;
    private Vector3 smoothVelocity;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float time = 1f;
    private bool transitioning;

    private float currentYaw;

    private void Start()
    {
        currentYaw = dog.eulerAngles.y;
        transform.position = FollowPosition();
        transform.rotation = FollowRotation();
    }

    public void ShowObstacle(Transform view)
    {
        obstacleView = view;
        BeginTransition();
    }

    public void ReturnToFollow()
    {
        obstacleView = null;
        BeginTransition();
    }

    private void BeginTransition()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        time = 0f;
        transitioning = true;
        smoothVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        bool goingToObstacle = obstacleView != null;

        // keep the camera yaw trailing the dog's facing direction
        currentYaw = Mathf.LerpAngle(currentYaw, dog.eulerAngles.y, rotationSmoothSpeed * Time.deltaTime);

        Vector3 goalPosition = goingToObstacle ? obstacleView.position : FollowPosition();
        Quaternion goalRotation = goingToObstacle ? obstacleView.rotation : FollowRotation();

        if (transitioning)
        {
            time = Mathf.MoveTowards(time, 1f, Time.deltaTime / transitionDuration);
            float easedTime = Mathf.SmoothStep(0f, 1f, time);
            float positionBlend = goingToObstacle ? easedTime : easedTime * easedTime;
            float rotationBlend = goingToObstacle ? easedTime * easedTime : 1f - (1f - easedTime) * (1f - easedTime);

            transform.position = Vector3.Lerp(startPosition, goalPosition, positionBlend);
            transform.rotation = Quaternion.Slerp(startRotation, goalRotation, rotationBlend);

            if (time >= 1f) transitioning = false;
        }
        else if (goingToObstacle)
        {
            transform.position = goalPosition;
            transform.rotation = goalRotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref smoothVelocity, followSmoothTime);
            transform.rotation = goalRotation;
        }
    }

    private float BasePitch => Mathf.Atan2(height - lookHeight, distanceBehind) * Mathf.Rad2Deg;

    private Vector3 FollowPosition()
    {
        Vector3 pivot = dog.position + Vector3.up * lookHeight;
        Vector3 offset = new Vector3(0f, height - lookHeight, -distanceBehind);
        return pivot + Quaternion.Euler(0f, currentYaw, 0f) * offset;
    }

    private Quaternion FollowRotation() => Quaternion.Euler(BasePitch, currentYaw, 0f);
}