using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoneButton : MonoBehaviour
{
    public MeatCooking meatCooking;
    [SerializeField] private PotMixtures potMixtures;
    [SerializeField] private CookingManager cookingManager;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject meatTimer;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color flashColor = Color.green;
    [SerializeField] private float flashSpeed = 2f;
    [SerializeField] private string victorySceneName = "WinScene";


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
    }

    private void Update()
    {
        UpdateFlash();

        if (!clickAction.WasPressedThisFrame()) return;

        Ray ray = cam.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == buttonCollider)
            PressDone();
    }

    private void UpdateFlash()
    {
        if (label == null) return;

        bool meatReady = meatCooking.isCooking && meatCooking.state == MeatCooking.CookState.Brown;
        bool shouldFlash = meatReady || cookingManager.IsFinished;

        if (shouldFlash)
            label.color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
        else
            label.color = idleColor;
    }

    private void PressDone()
    {
        if (cookingManager.IsFinished)
        {
            SceneManager.LoadScene(victorySceneName);
            return;
        }

        if (!meatCooking.isCooking) return;

        meatCooking.PauseCooking();

        if (meatCooking.state == MeatCooking.CookState.Brown)
        {
            potMixtures.ShowStage(0);
            cookingManager.BeginRecipe();
            meatTimer.SetActive(false);
            meatCooking.gameObject.SetActive(false);
        }
    }
}