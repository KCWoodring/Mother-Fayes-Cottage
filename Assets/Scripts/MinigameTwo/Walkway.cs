using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Walkway : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] private float startDelay = 1.5f;
    private bool starting;

    [Header("Walkway")]
    [SerializeField] private Transform startPoint;     
    [SerializeField] private Transform endPoint;
    [SerializeField] private float dogHeightOffset = 0.3f;

    [Header("References")]
    [SerializeField] private Transform dog;
    [SerializeField] private DogMovement dogMovement;
    [SerializeField] private CourseCamera courseCamera;
    [SerializeField] private Transform cameraView;
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    [Header("Tilt")]
    [SerializeField] private float failAngle = 65f;
    [SerializeField] private float baseLean = 8f;
    [SerializeField] private float leanGrowth = 0.6f;
    [SerializeField] private float centerSpeed = 60f;
    [SerializeField] private float centerArea = 3f;
    [SerializeField] private Vector3 tiltAxis = Vector3.forward;   

    [Header("Moving Forward")]
    [SerializeField] private float centeredSpeed = 0.5f;
    [SerializeField] private float tiltedSpeed = 1.5f;

    [Header("Camera View")]
    [SerializeField] private bool moveCameraView = true;
    [SerializeField] private float camDistance = 6f;
    [SerializeField] private float camHeight = 2f;
    [SerializeField] private float lookAtHeight = 1f;

    [Header("On Fail")]
    [SerializeField] private Vector3 failSpawnOffset = new Vector3(0f, 0.5f, 2f);

    public UnityEvent onMistake;
    public UnityEvent onComplete;

    private CharacterController dogController;
    private HoldButtonImage leftHold, rightHold;
    private bool active;
    private bool leaning;
    private float angle, leanDir, moveInput;

    Vector3 lineDir;          
    float lineLength;
    float travelled;          
    Vector3 heading;          
    Quaternion baseRotation;

    private bool flipped;

    private void Awake()
    {
        dogController = dog.GetComponent<CharacterController>();
        leftHold = GetOrAddHold(leftArrow);
        rightHold = GetOrAddHold(rightArrow);
        SetArrows(false);
    }
    public void SetFlipped(bool flip)
    {
        if (active || flip == flipped) return;

        (startPoint, endPoint) = (endPoint, startPoint);
        flipped = flip;
    }
    public void BeginMinigame()
    {
        if (active) return;

        dogMovement.SetControl(false);

        if (dogController != null) dogController.enabled = false;

        Vector3 a = startPoint.position;
        Vector3 b = endPoint.position;
        lineDir = (b - a).normalized;
        lineLength = Vector3.Distance(a, b);

        heading = Vector3.ProjectOnPlane(lineDir, Vector3.up).normalized;
        baseRotation = Quaternion.LookRotation(heading, Vector3.up);

        ResetRun();

        PlaceCameraView(dog.position);

        StartCoroutine(BeginAfterDelay());
    }
    private IEnumerator BeginAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);

        SetArrows(true);
        UpdateArrowColors();
        starting = false;
        active = true;
    }
    private void ResetRun()
    {
        travelled = 0f;
        angle = 0f;
        leaning = false;
        RollLeanDirection();
        dog.position = startPoint.position + Vector3.up * dogHeightOffset;
        ApplyRotation();
    }
    void Update()
    {
        if (!active) return;

        ReadInput();

        float dt = Time.deltaTime;

        angle += leanDir * (baseLean + Mathf.Abs(angle) * leanGrowth) * dt;
        angle += moveInput * centerSpeed * dt;

        float abs = Mathf.Abs(angle);
        if (abs > centerArea * 2f)
            leaning = true;
        else if (leaning && abs <= centerArea)
        {
            leaning = false;
            RollLeanDirection();
        }

        if (abs >= failAngle)
        {
            angle = Mathf.Clamp(angle, -failAngle, failAngle);
            ApplyRotation();
            Fail();
            return;
        }

        ApplyRotation();

     
        float speed = Mathf.Lerp(centeredSpeed, tiltedSpeed, abs / failAngle);
        travelled += speed * dt;

        if (travelled >= lineLength)
        {
            dog.position = endPoint.position + Vector3.up * dogHeightOffset;
            Finish();
            return;
        }

        dog.position = startPoint.position + lineDir * travelled + Vector3.up * dogHeightOffset;
        FollowWithCamera();
    }
    private void ReadInput()
    {
        float input = 0f;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) input += 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) input -= 1f;

            if (leftHold != null && leftHold.IsHeld) input += 1f;
            if (rightHold != null && rightHold.IsHeld) input -= 1f;

            input = Mathf.Clamp(input, -1f, 1f);
        }

        moveInput = input;
        UpdateArrowColors();
    }
    private void Fail()
    {
        dog.position = endPoint.position + baseRotation * failSpawnOffset;

        onMistake.Invoke();
        Finish();
    }
    private void FollowWithCamera() => PlaceCameraView(dog.position);

    private void PlaceCameraView(Vector3 dogPos)
    {
        if (!moveCameraView || cameraView == null) return;

        Vector3 pos = dogPos + heading * camDistance + Vector3.up * camHeight;
        Vector3 focus = dogPos + Vector3.up * lookAtHeight;
        cameraView.SetPositionAndRotation(pos, Quaternion.LookRotation(focus - pos, Vector3.up));
    }
    private void Finish()
    {
        active = false;
        moveInput = 0f;
        SetArrows(false);

        dog.rotation = Quaternion.LookRotation(heading, Vector3.up);
        dog.position = endPoint.position + dog.rotation * failSpawnOffset;
        if (dogController != null) dogController.enabled = true;

        dogMovement.SetControl(true);
        courseCamera.ReturnToFollow();
        onComplete.Invoke();
    }
    private void ApplyRotation() => dog.rotation = baseRotation * Quaternion.AngleAxis(-angle, tiltAxis);

    private void RollLeanDirection() => leanDir = Random.value < 0.5f ? -1f : 1f;

    private void SetArrows(bool on)
    {
        if (leftArrow != null) leftArrow.gameObject.SetActive(on);
        if (rightArrow != null) rightArrow.gameObject.SetActive(on);
    }
    private void UpdateArrowColors()
    {
        if (leftArrow != null) leftArrow.color = moveInput > 0 ? Color.gray : Color.white;
        if (rightArrow != null) rightArrow.color = moveInput < 0 ? Color.gray : Color.white;
    }
    private HoldButtonImage GetOrAddHold(Image arrow)
    {
        if (arrow == null) return null;
        arrow.raycastTarget = true;
        var hold = arrow.GetComponent<HoldButtonImage>();
        return hold != null ? hold : arrow.gameObject.AddComponent<HoldButtonImage>();
    }
}

