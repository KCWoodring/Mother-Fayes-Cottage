using UnityEngine;
using UnityEngine.InputSystem;

public class MeatChopping : MonoBehaviour
{
    public enum ChopState { Whole, Four, Bits }

    [Header("Settings")]
    public float chopCooldown = 1f;

    [Header("Visuals")]
    public MeshRenderer meatRenderer;
    public MeatCooking meatCooking;

    public Material wholeSprite;
    public Material fourSprite;
    public Material bitsSprite;

    [Header("Audio")]
    public AudioClip chopSound;
    private AudioSource audioSource;

    private float nextChopTime;
    private bool _canChop;
    private int canChopFrame = -1;

    private InputAction clickAction;
    private InputAction pointAction;
    private Collider collider;

    public ChopState state { get; private set; } = ChopState.Whole;

    public bool canChop
    {
        get { return _canChop; }
        set { _canChop = value; if (value) canChopFrame = Time.frameCount; }
    }

    void Awake()
    {
        if (meatRenderer == null) meatRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        if (!_canChop || Time.frameCount == canChopFrame) return;
        if (!clickAction.WasPressedThisFrame()) return;

        Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == collider)
        {
            TryChop();
        }
    }

    void TryChop()
    {
        if (state == ChopState.Bits) return;
        if (Time.time < nextChopTime)
        {
            Debug.Log("ChopOnCooldown");
            return;
        }

        nextChopTime = Time.time + chopCooldown;
        state++;
        ApplyState();
    }

    void ApplyState()
    {
        MeatCooking meatCooking = GetComponent<MeatCooking>();
        if (chopSound != null && audioSource != null)
        {
            Debug.Log("Sound");
            AudioSource.PlayClipAtPoint(chopSound, Camera.main.transform.position, 1.0f);
        }
        switch (state)
        {
            case ChopState.Whole:
                meatRenderer.material = wholeSprite;
                break;
            case ChopState.Four:
                meatRenderer.material = fourSprite;
                break;
            case ChopState.Bits:
                meatCooking.StartCooking();
                meatRenderer.material = bitsSprite;
                break;
        }
        Debug.Log("Chop state: " + state);
    }
}