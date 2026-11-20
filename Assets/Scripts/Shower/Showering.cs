using TMPro;
using UnityEngine;


public class Showering : MonoBehaviour
{
    public float timeLimit = 300f;
    public Transform dog;          
    public TMP_Text timerText;

    public SudSpot[] spots;
    float timeLeft;
    bool done;

    void Start()
    {
        spots = dog.GetComponentsInChildren<SudSpot>();
        timeLeft = timeLimit;
    }

    void Update()
    {
        if (done) return;

        timeLeft -= Time.deltaTime;
        int t = (int)Mathf.Max(timeLeft, 0);
        timerText.text = $"{t / 60}:{t % 60:00}";

        if (AllClean()) { done = true; OnWin(); }
        else if (timeLeft <= 0f) { done = true; OnLose(); }
    }

    bool AllClean()
    {
        foreach (var suds in spots)
            if (suds != null) return false;
        return true;
    }

    
    void OnWin() { Debug.Log("Dog is clean!"); }
    void OnLose() { Debug.Log("Out of time!"); }
}