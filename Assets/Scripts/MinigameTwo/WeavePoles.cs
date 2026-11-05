using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeavePoles : MonoBehaviour
{
    [SerializeField] private Transform[] weavePoints;
    [SerializeField] private float dogSpeed = 6f;
    [SerializeField] private float arriveDistance = 0.1f;

    [SerializeField] private Transform dog;
    [SerializeField] private DogMovement dogMovement;
    [SerializeField] private CourseCamera courseCamera;
    [SerializeField] private Transform cameraView;

    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private HoldButtonImage leftHold, rightHold;


    public UnityEvent onMistake;
    public UnityEvent onComplete;

    private CharacterController dogCharacterController;
    private bool active;
    private int targetIndex;

    private bool flipped;
    private void Awake()
    {
        dogCharacterController = dog.GetComponent<CharacterController>();
        leftHold = leftArrow.GetComponent<HoldButtonImage>();
        rightHold = rightArrow.GetComponent<HoldButtonImage>();
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
    }
    public void BeginMinigame()
    {
        active = true;
        targetIndex = 0;
        dogMovement.SetControl(false);
    }
    private void Update()
    {
        if (!active) return;

        ReadInput();
        MoveDog();
        FollowWithCamera();
    }
    private void ReadInput()
    {
        bool clickedLeft = leftHold.PressDown();
        bool clickedRight = rightHold.PressDown();

        if (targetIndex >= weavePoints.Length - 1) return;
        if (!HasArrived()) return;

        Vector3 nextStep = weavePoints[targetIndex + 1].position - weavePoints[targetIndex].position;
        bool nextNeedsRight = Vector3.Dot(nextStep, cameraView.right) > 0f;

        rightArrow.gameObject.SetActive(nextNeedsRight);
        leftArrow.gameObject.SetActive(!nextNeedsRight);

        bool pressedLeft = clickedLeft || Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame;
        bool pressedRight = clickedRight || Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame;
        if (!pressedLeft && !pressedRight) return;

        if (nextNeedsRight == pressedRight)
            targetIndex++;
        else
            onMistake.Invoke();
    }
    private void MoveDog()
    {
        Vector3 direction = weavePoints[targetIndex].position - dog.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            dogCharacterController.Move(Vector3.ClampMagnitude(direction, dogSpeed * Time.deltaTime));
            dog.rotation = Quaternion.Slerp(dog.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        }

        if (targetIndex == weavePoints.Length - 1 && HasArrived())
        {
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(false);
            Finish();
        }
    }
    private void FollowWithCamera()
    {
        Vector3 viewPosition = cameraView.position;
        cameraView.position = new Vector3(viewPosition.x, viewPosition.y, dog.position.z);
    }
    private bool HasArrived()
    {
        Vector3 toTarget = weavePoints[targetIndex].position - dog.position;
        toTarget.y = 0f;
        return toTarget.magnitude <= arriveDistance;
    }

    private void Finish()
    {
        active = false;
        dogMovement.SetControl(true);
        courseCamera.ReturnToFollow();
        onComplete.Invoke();
    }
    public void SetFlipped(bool flip)
    {
        if (active || flip == flipped) return;
        System.Array.Reverse(weavePoints);
        flipped = flip;
    }

    private void OnDrawGizmos()
    {
        if (weavePoints == null) return;
        Gizmos.color = Color.magenta;
        for (int i = 0; i < weavePoints.Length; i++)
        {
            if (weavePoints[i] == null) continue;
            Gizmos.DrawSphere(weavePoints[i].position, 0.15f);
            if (i > 0 && weavePoints[i - 1] != null)
                Gizmos.DrawLine(weavePoints[i - 1].position, weavePoints[i].position);
        }
    }
}
