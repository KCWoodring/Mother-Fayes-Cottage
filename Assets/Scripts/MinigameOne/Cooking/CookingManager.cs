using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CookingManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public GameObject[] ingredientObjects;
        public int mixtureStage;
    }

    [SerializeField] private List<Step> steps = new List<Step>();

    [SerializeField] private Stirring stirring;
    [SerializeField] private PotMixtures potMixtures;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stirDoneClip;

    public UnityEvent onIngredientAdded;
    public UnityEvent onRecipeDone;

    private readonly HashSet<GameObject> added = new HashSet<GameObject>();
    private int stepIndex;
    private bool started;
    private bool waitingForStir;

    public bool IsFinished => stepIndex >= steps.Count;

    [SerializeField] private PrepStationManager prepStationManager;

    private void Awake()
    {
        foreach (Step step in steps)
            SetStepObjects(step, false);
    }

    private void OnEnable() { stirring.OnSoupCompleet.AddListener(HandleStirDone); }
    private void OnDisable() { stirring.OnSoupCompleet.RemoveListener(HandleStirDone); }

    public void BeginRecipe()
    {
        if (started || IsFinished) return;

        started = true;
        SetStepObjects(steps[stepIndex], true);
    }

    public bool CanAccept(GameObject ingredient)
    {
        if (!started || IsFinished || waitingForStir) return false;

        return System.Array.IndexOf(steps[stepIndex].ingredientObjects, ingredient) >= 0
            && !added.Contains(ingredient);
    }

    public void AddIngredient(GameObject ingredient)
    {
        if (!CanAccept(ingredient)) return;

        added.Add(ingredient);
        SetFlash(ingredient, false);
        onIngredientAdded?.Invoke();

        Step step = steps[stepIndex];
        if (added.Count < step.ingredientObjects.Length) return;

        potMixtures.ShowStage(step.mixtureStage);
        waitingForStir = true;
        stirring.BeginStirPhase();
    }

    private void HandleStirDone()
    {
        if (!waitingForStir) return;

        waitingForStir = false;
        if (audioSource != null && stirDoneClip != null)
            audioSource.PlayOneShot(stirDoneClip);

        SetStepObjects(steps[stepIndex], false);
        stepIndex++;
        added.Clear();

        if (IsFinished)
        {
            onRecipeDone?.Invoke();
            return;
        }

        SetStepObjects(steps[stepIndex], true);
    }

    private void SetStepObjects(Step step, bool on)
    {
        foreach (GameObject gameObject in step.ingredientObjects)
        {
            if (gameObject == null) continue;

            RiceCookerDrag ghostDrag = gameObject.GetComponent<RiceCookerDrag>();
            if (ghostDrag != null) ghostDrag.enabled = on;

            DragFruit fruitDrag = gameObject.GetComponent<DragFruit>();
            if (fruitDrag != null)
            {
                bool shouldBeOn = on;

                if (on && prepStationManager != null)
                {
                    bool isPumpkin = (gameObject == prepStationManager.pumpkin || gameObject.name.Contains("Pumpkin"));
                    bool isHoneyDew = (gameObject == prepStationManager.honeyDew || gameObject.name.Contains("HoneyDew"));

                    if (isPumpkin)
                    {
                        if (prepStationManager.pumpkinInBlender || gameObject.name == "Pumpkin")
                        {
                            shouldBeOn = false;
                        }
                    }
                    if (isHoneyDew)
                    {
                        if (prepStationManager.honeyDewInBlender || gameObject.name == "HoneyDew")
                        {
                            shouldBeOn = false;
                        }
                    }
                }

                fruitDrag.enabled = shouldBeOn;
            }

            SetFlash(gameObject, on);
        }
    }
    public void RefreshCurrentStepDrag()
    {
        if (!started || IsFinished || waitingForStir) return;
        SetStepObjects(steps[stepIndex], true);
    }
    private void SetFlash(GameObject gameObject, bool on)
    {
        FlashTint flash = gameObject.GetComponent<FlashTint>();
        if (flash == null)
        {
            if (!on) return;
            flash = gameObject.AddComponent<FlashTint>();
        }
        flash.enabled = on;
    }
}