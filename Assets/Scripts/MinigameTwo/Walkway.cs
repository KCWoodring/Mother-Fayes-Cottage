using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.UI;

public class Walkway : MonoBehaviour
{
    [Header("Walkway")]
    [SerializeField] private Transform startPoint;     
    [SerializeField] private Transform endPoint;       
    [SerializeField] private string walkwayTag = "Walkway";

    [Header("Tilt")]
    [SerializeField] private float failAngle = 65f;
    [SerializeField] private float baseLean = 8f;
    [SerializeField] private float leanGrowth = 0.6f;
    [SerializeField] private float centerSpeed = 60f;
    [SerializeField] private float centerArea = 3f;
    [SerializeField] private Vector3 tiltAxis = Vector3.forward;   
    [SerializeField] private GameObject LeftArrow;
    [SerializeField] private GameObject RightArrow;

    [Header("Moving Forward")]
    [SerializeField] private float centeredSpeed = 0.5f;
    [SerializeField] private float tiltedSpeed = 1.5f;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private MonoBehaviour camFollow;
    [SerializeField] private MouseController mouse;
    [SerializeField] private float camDistance = 6f;
    [SerializeField] private float camHeight = 2f;
    [SerializeField] private float lookAtHeight = 1f;
    [SerializeField] private float camSmoothing = 4f;
    [SerializeField] private Vector3 topDownEuler = new Vector3(90f, 0f, 0f);
    bool camActive;
    bool camReturning;
    float angle, leanDir, moveInput;
    bool leaning, balancing;

    Vector3 lineDir;          
    float lineLength;
    float travelled;          
    Vector3 heading;          
    Quaternion baseRotation;

    Transform camT;
    Vector3 camOffset;
   
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (balancing || camActive || !other.CompareTag(walkwayTag)) return;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
            RightArrow.SetActive(true);
        LeftArrow.SetActive(true);
        camT = cam.transform;
        camOffset = camT.position - transform.position;
        

        camFollow.enabled = false;
        mouse.enabled = false;

       
        Vector3 a = startPoint.position;
        Vector3 b = endPoint.position;
        lineDir = (b - a).normalized;
        lineLength = Vector3.Distance(a, b);
        travelled = 0f;

        heading = Vector3.ProjectOnPlane(lineDir, Vector3.up).normalized;
        baseRotation = Quaternion.LookRotation(heading, Vector3.up);

        transform.position = a;          

        angle = 0f;
        leaning = false;
        RollLeanDirection();
        ApplyRotation();
        camActive = true;
        camReturning = false;
        balancing = true;
       

        other.enabled = false;           
    }

    void Update()
    {
        if (!balancing) return;

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
            
            EndBalance();
            return;
        }

        ApplyRotation();

     
        float speed = Mathf.Lerp(centeredSpeed, tiltedSpeed, abs / failAngle);
        travelled += speed * dt;

        if (travelled >= lineLength)
        {
            transform.position = endPoint.position;  
            EndBalance();
            return;
        }

        transform.position = startPoint.position + lineDir * travelled;
    }

    void LateUpdate()
    {
        if (!camActive) return;

        Vector3 targetPos;
        Quaternion targetRot = Quaternion.Euler(topDownEuler);

        if (camReturning)
        {
            targetPos = transform.position + camOffset;   
        }
        else
        {
            
            targetPos = transform.position + heading * camDistance + Vector3.up * camHeight;
            Vector3 focus = transform.position + Vector3.up * lookAtHeight;
            targetRot = Quaternion.LookRotation(focus - targetPos, Vector3.up);
        }

        
        float t = 1f - Mathf.Exp(-camSmoothing * Time.deltaTime);
        camT.position = Vector3.Lerp(camT.position, targetPos, t);
        camT.rotation = Quaternion.Slerp(camT.rotation, targetRot, t);

       
        if (camReturning && (camT.position - targetPos).sqrMagnitude < 0.01f && Quaternion.Angle(camT.rotation, targetRot) < 1f)
        {
            if (camReturning)
            {
                targetPos = transform.position + camOffset;
                targetRot = Quaternion.Euler(topDownEuler);
            }
            camT.position = targetPos;
            camT.rotation = targetRot;
            camActive = false;
            camReturning = false;
            mouse.enabled = true;
            transform.rotation = Quaternion.identity;
            camFollow.enabled = true;
        }
    }

    void EndBalance()
    {
        balancing = false;
        camReturning = true;

        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.freezeRotation = true;          
            rb.angularVelocity = Vector3.zero;
            rb.rotation = Quaternion.identity;
        }

        Destroy(LeftArrow);
        Destroy(RightArrow);
    }

    void ApplyRotation() => transform.rotation = baseRotation * Quaternion.AngleAxis(-angle, tiltAxis);

    void RollLeanDirection() => leanDir = Random.value < 0.5f ? -1f : 1f;
       
    

    public void Balance(InputAction.CallbackContext context)
    {
        moveInput = context.canceled ? 0f : context.ReadValue<float>();
        LeftArrow.GetComponent<Image>().color = moveInput > 0 ? Color.gray : Color.white;
        RightArrow.GetComponent<Image>().color = moveInput < 0 ? Color.gray : Color.white;
    }
}

