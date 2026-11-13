using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoneButton : MonoBehaviour
{
    public MeatCooking meatCooking;
    [SerializeField] private PotMixtures potMixtures;
    [SerializeField] private CookingManager cookingManager;
    [SerializeField] private PuppyAttention puppy;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text burnLabel;
    [SerializeField] private GameObject meatTimer;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color flashColor = Color.green;
    [SerializeField] private Color idleBurnColor = Color.red;
    [SerializeField] private Color flashBurnColor = Color.red;
    [SerializeField] private float flashSpeed = 2f;
    [SerializeField] private string victorySceneName = "MinigameTwo";

    [SerializeField] private GameObject meatBurningtxt;
    [SerializeField] private GameObject cookStationtxt;
    [SerializeField] private GameObject prepStationtxt;


    private InputAction clickAction;
    private InputAction pointAction;
    private Collider buttonCollider;
    private Camera cam;

    private void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        buttonCollider = GetComponent<Collider>();
        cam = Camera.main;
        label.color = idleColor;
        meatBurningtxt.SetActive(false);
    }

    private void Update()
    {
        UpdateFlash();

        if (!clickAction.WasPressedThisFrame()) return;
        if (!CanPress()) return;

        Ray ray = cam.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == buttonCollider)
            PressDone();
    }

    private void UpdateFlash()
    {
        if (label == null) return;

        if (IsDoneReady())
        {
            label.color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
            burnLabel.color = Color.Lerp(idleBurnColor, flashBurnColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
        }

        else
        {
            label.color = idleColor;
            burnLabel.color = idleBurnColor;
        }
    }

    private void PressDone()
    {
        if (cookingManager.IsFinished)
        {
            SceneManager.LoadScene(victorySceneName);
            return;
        }

        meatCooking.PauseCooking();
        potMixtures.ShowStage(0);
        cookingManager.BeginRecipe();
        meatTimer.SetActive(false);
        meatCooking.gameObject.SetActive(false);
        meatBurningtxt.SetActive(false);
        cookStationtxt.SetActive(true);
        prepStationtxt.SetActive(true);
    }
    private bool IsDoneReady()
    {

        bool meatReady = meatCooking.isCooking && meatCooking.state == MeatCooking.CookState.Brown;
        meatBurningtxt.SetActive(meatReady);
        cookStationtxt.SetActive(!meatReady);
        prepStationtxt.SetActive(!meatReady);
        return meatReady || cookingManager.IsFinished;
    }
    private bool CanPress()
    {
        bool dogBlocking = puppy != null && puppy.DogOnScreen;
        return IsDoneReady() && !dogBlocking;
    }
}