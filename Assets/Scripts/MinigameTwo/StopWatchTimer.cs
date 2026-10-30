using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class StopWatchTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool showHundredths = false;

    [Header("Mistake penalty")]
    [SerializeField] private float penaltySeconds = 3f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color penaltyColor = Color.red;
    [SerializeField] private float flashDuration = 0.6f;

    [Header("Time limit")]
    [SerializeField] private float timeLimit = 90f;
    [SerializeField] private string loserSceneName = "GameOverScene";

    public float Elapsed { get; private set; }
    public bool IsRunning { get; private set; }

    private float flashTimeRemaining;

    private void Start()
    {
        label.color = normalColor;
        UpdateLabel();
        if (startOnAwake) StartWatch();
    }

    private void Update()
    {
        UpdateFlash();

        if (!IsRunning) return;

        Elapsed += Time.deltaTime;
        UpdateLabel();
        CheckTimeLimit();
    }

    public void StartWatch() => IsRunning = true;

    public void PauseWatch() => IsRunning = false;

    public float StopWatch()
    {
        IsRunning = false;
        UpdateLabel();
        return Elapsed;
    }

    public void ResetWatch()
    {
        Elapsed = 0f;
        UpdateLabel();
    }

    public void Restart()
    {
        ResetWatch();
        StartWatch();
    }
    public void AddPenalty()
    {
        Elapsed += penaltySeconds;
        flashTimeRemaining = flashDuration;
        UpdateLabel();
        CheckTimeLimit();
    }

    private void UpdateFlash()
    {
        if (label == null || flashTimeRemaining <= 0f) return;

        flashTimeRemaining -= Time.deltaTime;

        // starts fully red, then fades back to normal
        float flashAmount = Mathf.Clamp01(flashTimeRemaining / flashDuration);
        label.color = Color.Lerp(normalColor, penaltyColor, flashAmount);
    }

    private void UpdateLabel()
    {
        if (label != null) label.text = Format(Elapsed, showHundredths);
    }
    private void CheckTimeLimit()
    {
        if (!IsRunning || Elapsed < timeLimit) return;

        IsRunning = false;
        SceneManager.LoadScene(loserSceneName);
    }

    public static string Format(float seconds, bool hundredths = false)
    {
        int minutes = (int)(seconds / 60f);
        int secs = (int)(seconds % 60f);

        if (hundredths)
        {
            int cs = (int)((seconds * 100f) % 100f);
            return $"{minutes:00}:{secs:00}.{cs:00}";
        }
        return $"{minutes:00}:{secs:00}";
    }
}
