using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int minigamesToWin = 5;
    [SerializeField] private string victorySceneName = "WinScene";
    [SerializeField] private float victoryDelay = 1.5f;
    [SerializeField] private StopWatchTimer stopWatch;

    public int Completed { get; private set; }
    public int MinigamesToWin => minigamesToWin;
    public static float FinalTime { get; private set; }
    public event Action<int, int> OnProgressChanged;

    private bool won;
    public void MinigameCompleted()
    {
        if (won) return;

        Completed++;
        Debug.Log("Minigames completed: " + Completed + "/" + minigamesToWin);

        OnProgressChanged?.Invoke(Completed, minigamesToWin);

        if (Completed >= minigamesToWin) Win();
    }
    private void Win()
    {
        won = true;

        if (stopWatch != null) FinalTime = stopWatch.StopWatch();

        Invoke(nameof(LoadVictory), victoryDelay);
    }

    private void LoadVictory() => SceneManager.LoadScene(victorySceneName);
}
