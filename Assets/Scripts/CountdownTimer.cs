
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Reflection.Metadata.Ecma335;

public class CountdownTimer : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 60f;
    public bool startOnAwake = false;
    public TMP_Text timerText;

    public float timeRemaining { get; private set; }
    public bool isRunning { get; private set; }
    void Start()
    {
        timeRemaining = duration;
        UpdateText();
        if (startOnAwake) StartTimer();
    }
    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateText();

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            UpdateText();
            TimerFinished();
        }
    }
    void TimerFinished()
    {
        Debug.Log("Timer finished!");
    }
    public void StartTimer()
    {
        timeRemaining = duration;
        isRunning = true;
    }
    public void PauseTimer()
    {
        isRunning = false;
    }
    public void ResetTimer()
    {
        timeRemaining = duration;
        isRunning = false;
        UpdateText();
    }
    void UpdateText()
    {
        if (timerText == null) return;
        int total = Mathf.CeilToInt(timeRemaining);
        timerText.text = (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }
}
