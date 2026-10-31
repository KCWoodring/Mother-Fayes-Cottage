using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class TireJumpQTE : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector director;
    public PlayableAsset approachTimeline;
    public PlayableAsset successTimeline; 
    public PlayableAsset partialTimeline; 
    public PlayableAsset failTimeline; 

    [Header("UI")]
    public GameObject qtePanel;
    public Quicktime quicktime;
    public GameObject approachText;

    [Header("Dog Control")]
    public MonoBehaviour dogMovementScript;
    public CharacterController dogCharacterController;
    public Animator dogAnimator;

    [Header("Penalties")]
    [SerializeField] private float failPenaltySeconds = 60f;
    [SerializeField] private float partialPenaltySeconds = 15f;

    [Header("Fail / Restart")]
    public StopWatchTimer stopWatch;
    public Transform dog;
    [SerializeField] private float restartDelay = 0.5f;
    [SerializeField] private float restartCameraDelay = 0.5f;

    [Header("Finish")]
    public DogMovement dogMovement;
    public CourseCamera courseCamera;
    public UnityEvent onComplete;

    Vector3 startPosition;
    Quaternion startRotation;

    public float cameraDelay = 2f;

    bool resolved;
    bool hasStart;

    void Start()
    {
        qtePanel.SetActive(false);
        approachText.SetActive(false);
        director.extrapolationMode = DirectorWrapMode.Hold;
    }
    public void BeginMinigame()
    {
        if (!hasStart)
        {
            startPosition = dog.position;
            startRotation = dog.rotation;
            hasStart = true;
        }

        StartRun(cameraDelay);
    }

    public void OnApproachFinished()
    {
        if (!resolved) RunQTE();
    }

    void PlayTimeline(PlayableAsset asset)
    {
        director.playableAsset = asset;

        if (asset is TimelineAsset timeline)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track is AnimationTrack)
                    director.SetGenericBinding(track, dogAnimator);
            }
        }

        director.time = 0;
        director.Play();
    }

    void RunQTE()
    {
        director.Pause();
        qtePanel.SetActive(true); 
        quicktime.Finished += OnQuicktimeFinished;
        quicktime.Begin();
    }

    void OnQuicktimeFinished(Quicktime.Zone zone)
    {
        quicktime.Finished -= OnQuicktimeFinished;
        resolved = true;
        qtePanel.SetActive(false);

        PlayableAsset result;
        float penalty = 0f;

        switch (zone)
        {
            case Quicktime.Zone.Green:
                result = successTimeline;
                break;
            case Quicktime.Zone.Yellow:
                result = partialTimeline;
                penalty = partialPenaltySeconds;
                break;
            default:
                result = failTimeline;
                penalty = failPenaltySeconds;
                break;
        }

        if (stopWatch != null && penalty > 0f) stopWatch.AddPenalty(penalty);

        PlayTimeline(result);
        StartCoroutine(FinishAfter(result));
    }

    void StartRun(float delay)
    {
        resolved = false;
        dogMovement.SetControl(false);
        StartCoroutine(BeginAfterDelay(delay));
    }

    void SetDogControl(bool enabled)
    {
        if (dogCharacterController != null) dogCharacterController.enabled = enabled;
    }

    void Finish()
    {
        director.Stop();

        dog.rotation = startRotation;
        SetDogControl(true);
        dogMovement.SetControl(true);
        courseCamera.ReturnToFollow();
        onComplete.Invoke();
    }

    IEnumerator FinishAfter(PlayableAsset asset)
    {
        yield return new WaitForSeconds((float)asset.duration);
        Finish();
    }

    IEnumerator BeginAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayTimeline(approachTimeline);
        yield return new WaitForSeconds(1f);
        approachText.SetActive(true);

        yield return new WaitForSeconds((float)approachTimeline.duration - 1f);

        approachText.SetActive(false);

        OnApproachFinished();
    }
}