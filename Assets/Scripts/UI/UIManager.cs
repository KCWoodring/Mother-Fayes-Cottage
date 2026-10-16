using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject instructionsPage;
    public Animator recipeAnimator;
    public CountdownTimer timer;
    public PuppyAttention puppyAttention;

    bool recipeShown = false;
    bool timerStarted = false;

    public bool InstructionsOpen => instructionsPage.activeSelf;

    void Start()
    {
        OpenInstructions();
    }
    public void OpenInstructions()
    {
        instructionsPage.SetActive(true);
    }

    public void CloseInstructions()
    {
        instructionsPage.SetActive(false);

        if (!recipeShown)
        {
            recipeShown = true;
            recipeAnimator.SetBool("Open", true);
        }
    }
    //3D mouse clicks calls this
    public void NotifyRecipeClosed()
    {
        if (recipeShown && !timerStarted)
        {
            timerStarted = true;
            timer.StartTimer();
            puppyAttention.StartAttentionCycle();
        }
    }
}
