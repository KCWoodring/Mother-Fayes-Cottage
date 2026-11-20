using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Showering : MonoBehaviour
{
    public float timeLimit = 300f;
    public Transform dog;          
    public TMP_Text timerText;
    public string winScreen;
    public string loseScreen;

    public SudSpot[] spots;
    float timeLeft;
    bool done;
    public bool isRunning;

    public GameObject instructionsPanel;
    public GameObject instructionsButon;

    void Start()
    {
        spots = dog.GetComponentsInChildren<SudSpot>();
        timeLeft = timeLimit;
        isRunning = false;
        done = false;
        instructionsButon.SetActive(true);
        instructionsPanel.SetActive(true);
    }

    void Update()
    {
        if (done) return;
        if (isRunning == false) return;

        timeLeft -= Time.deltaTime;
        int t = (int)Mathf.Max(timeLeft, 0);
        timerText.text = $"{t / 60}:{t % 60:00}";

        if (AllClean()) { done = true; OnWin(); }
        else if (timeLeft <= 0f) { done = true; OnLose(); }
    }
    public void StartGame()
    {
        instructionsPanel.SetActive(false);
        isRunning = true;
    }
    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }

    bool AllClean()
    {
        foreach (var suds in spots)
            if (suds != null) return false;
        return true;
    }
    void OnWin() 
    { 
        SceneManager.LoadScene(winScreen);
    }

    void OnLose() 
    { 
        SceneManager.LoadScene(loseScreen);
    }
}