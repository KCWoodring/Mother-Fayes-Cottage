using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PuppyAttention : MonoBehaviour
{
    public GameObject mainCamera;
    [SerializeField] private CutTrail cutTrail;
    [SerializeField] private TreatDrag treat;
    [SerializeField] private float attentionInterval = 15f;

    [SerializeField] private float barkInterval = 5f;
    private Coroutine barkRoutine;
    private CanvasGroup dogGroup;

    public UnityEvent onFed;
    private Animator puppyAnimator;
    private Animator camAnimator;
    private AudioSource audioSource;
    private bool waiting;

    public bool DogOnScreen { get; private set; }

    private void Awake()
    {
        puppyAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        camAnimator = mainCamera.GetComponent<Animator>();
        dogGroup = GetComponent<CanvasGroup>();
        SetDogVisible(false);
    }
    public void StartAttentionCycle()
    {
        CancelInvoke("PuppyWantsAttention");// stops double calling the dog
        InvokeRepeating("PuppyWantsAttention", attentionInterval, attentionInterval);
    }
    void PuppyWantsAttention()
    {
        if (waiting) return;

        waiting = true;
        SetDogVisible(true);
        puppyAnimator.SetBool("GettingAttention", true);
        puppyAnimator.SetTrigger("Bark");
        audioSource.Play();


        camAnimator.SetBool("AtPrep", false);
        camAnimator.SetBool("AtCook", false);
        camAnimator.SetBool("AtCounter", true);

        cutTrail.Clear();
        treat.Show();

        barkRoutine = StartCoroutine(BarkLoop());
    }
    private IEnumerator BarkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(barkInterval);
            puppyAnimator.SetTrigger("Bark");
            audioSource.Play(); // bark sound on each bark
        }
    }
    public void FeedPuppy()
    {
        if (!waiting) return;
        waiting = false;

        if (barkRoutine != null) StopCoroutine(barkRoutine);
        puppyAnimator.ResetTrigger("Bark");
        audioSource.Stop();

        puppyAnimator.SetBool("GettingAttention", false);  // then leave
        StartAttentionCycle();                             // reset the 20s timer
    }
    private void SetDogVisible(bool visible)
    {
        DogOnScreen = visible;
        dogGroup.alpha = visible ? 1f : 0f;
        dogGroup.blocksRaycasts = false;
    }
    public void OnLeaveFinished() => SetDogVisible(false);
}
