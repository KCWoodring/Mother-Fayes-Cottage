using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Stirring : MonoBehaviour
{
    public enum StirDirection { Clock, CounterClock }

    [SerializeField] private Transform soup;
    [SerializeField] private Collider potZone;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Animator cameraAnimator;

    [SerializeField] private float zoomDelay = 1.0f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stirSound;


    [SerializeField] private float laps = 3f;
    [SerializeField] private int rounds = 1;
    [SerializeField, Range(0f, 1f)] private float switchChance = 0.5f;
    [SerializeField] private StirDirection startDirection = StirDirection.Clock;
    [SerializeField] private float deadZonePixels = 30f;
    [SerializeField] private bool debugLogs = true;

    public UnityEvent<StirDirection> OnDirectionChange;
    public UnityEvent OnSoupCompleet;

    public StirDirection CurrentDirection { get; private set; }
    public float StirProgress => Mathf.Clamp01(progress / (360f * laps));

    private InputAction clickAction;
    private InputAction pointAction;
    private bool canStir, isStirring;
    private float progress, lastAngle;
    private int round, lastTenth;

    private float zoomTimer = 0f;
    private bool wasAtCook = false;

    private void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        if (cam == null) cam = Camera.main;
        if (potZone == null) potZone = GetComponent<Collider>();
        CurrentDirection = startDirection;
        OnDirectionChange?.Invoke(CurrentDirection);
        arrow.SetActive(false);
    }

    public void BeginStirPhase()
    {
        arrow.SetActive(true);
        round = 0;
        progress = 0f;
        lastTenth = 0;
        canStir = true;
        Debug.Log("Stirring unlocked. Need " + laps + " laps x " + rounds + " round(s). Direction: " + CurrentDirection);
    }

    private void Update()
    {
        bool isAtCook = cameraAnimator != null && cameraAnimator.GetBool("AtCook");

        if (isAtCook && !wasAtCook)
        {
            zoomTimer = zoomDelay;
        }
        wasAtCook = isAtCook;

        if (isAtCook && zoomTimer > 0f)
        {
            zoomTimer -= Time.deltaTime;
        }

        if (arrow != null)
        {
            arrow.SetActive(canStir && isAtCook && zoomTimer <= 0f);
        }

        if (!canStir || !isAtCook || zoomTimer > 0f)
        {
            if (isStirring)
            {
                StopStirringAudio();
                isStirring = false;
                Debug.Log("Stir cancelled due to leaving cook zone or zooming.");
            }
            return;
        }

        Vector2 pos = pointAction.ReadValue<Vector2>();
        bool pressed = clickAction.WasPressedThisFrame();
        bool overPot = pressed && potZone.Raycast(cam.ScreenPointToRay(pos), out _, 1000f);

        if (!canStir)
        {
            if (overPot) Debug.Log("Pressed on pot but stirring is locked (no ingredient added yet).");
            return;
        }

        if (pressed)
        {
            if (overPot)
            {
                isStirring = true;
                lastAngle = GetAngle(pos);
                Debug.Log("Stir started.");
            }
            else
            {
                Debug.Log("Press missed the pot zone collider.");
            }
        }

        if (isStirring && !clickAction.IsPressed())
        {
            StopStirringAudio();
            isStirring = false;
            Debug.Log("Stir released at " + Mathf.RoundToInt(StirProgress * 100f) + "%.");
        }

        if (isStirring) Stir(pos);
    }

    private void Stir(Vector2 pos)
    {
        if ((pos - PotScreenPos()).magnitude < deadZonePixels)
        {
            StopStirringAudio();
            return;
        }

        float angle = GetAngle(pos);
        float delta = Mathf.DeltaAngle(lastAngle, angle);
        lastAngle = angle;

        soup.Rotate(cam.transform.forward, -delta, Space.World);

        bool correct = (CurrentDirection == StirDirection.CounterClock) == (delta > 0f);
        progress = Mathf.Max(0f, progress + (correct ? Mathf.Abs(delta) : -Mathf.Abs(delta)));

        if (correct)
        {
            PlayStirringAudio();
        }
        else
        {
            StopStirringAudio();
        }

        int tenth = Mathf.FloorToInt(StirProgress * 10f);
        if (tenth != lastTenth)
        {
            lastTenth = tenth;
            Debug.Log("Progress " + tenth * 10 + "% (" + (correct ? "correct way" : "wrong way") + ", need " + CurrentDirection + ")");
        }

        if (progress >= 360f * laps) FinishRound();
    }
    private void PlayStirringAudio()
    {
        if (audioSource != null && stirSound != null)
        {
            if (!audioSource.isPlaying || audioSource.clip != stirSound)
            {
                audioSource.clip = stirSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    private void StopStirringAudio()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == stirSound)
        {
            audioSource.Stop();
        }
    }

    private void FinishRound()
    {
        StopStirringAudio();
        progress = 0f;
        lastTenth = 0;
        round++;
        Debug.Log("Round " + round + "/" + rounds + " finished.");

        if (round >= rounds)
        {
            canStir = false;
            isStirring = false;
            arrow.SetActive(false);
            Debug.Log("Stirring complete. Firing OnSoupCompleet.");
            OnSoupCompleet?.Invoke();
            return;
        }

        if (Random.value < switchChance)
        {
            CurrentDirection = CurrentDirection == StirDirection.Clock
                ? StirDirection.CounterClock : StirDirection.Clock;
            Debug.Log("Direction switched to " + CurrentDirection);
            OnDirectionChange?.Invoke(CurrentDirection);
        }
    }

    private Vector2 PotScreenPos() => cam.WorldToScreenPoint(soup.position);

    private float GetAngle(Vector2 screenPos)
    {
        Vector2 v = screenPos - PotScreenPos();
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    private void Log(string message)
    {
        if (debugLogs) Debug.Log("[Stirring] " + message);
    }
}