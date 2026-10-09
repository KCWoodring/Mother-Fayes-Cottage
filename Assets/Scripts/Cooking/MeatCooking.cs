using UnityEngine;
using UnityEngine.SceneManagement;

public class MeatCooking : MonoBehaviour
{
    public enum CookState { OnPlate, Raw, Half, Brown, Burnt }

    [Header("Timing (seconds since cooking started)")]
    public float halfTime = 10f;
    public float brownTime = 20f;
    public float burntTime = 30f;

    [Header("Visuals")]
    public MeshRenderer meatRenderer;

    public Material meatOnPlateMaterial;
    public Material wholeMaterial;
    public Material rawmaterial;
    public Material halfDoneMaterial;
    public Material brownMaterial;
    public Material burntMaterial;

    public float cookTime { get; private set; }
    public bool isCooking { get; private set; }
    public CookState state { get; private set; } = CookState.Raw;

    void Awake()
    {
        if (meatRenderer == null) meatRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        ApplyState(CookState.OnPlate);
    }

    void Update()
    {
        if (!isCooking) return;

        cookTime += Time.deltaTime;

        CookState newState = GetStateForTime(cookTime);
        if (newState != state)
        {
            ApplyState(newState);
        }           
    }
    CookState GetStateForTime(float time)
    {
        if (time >= burntTime) return CookState.Burnt;
        if (time >= brownTime) return CookState.Brown;
        if (time >= halfTime) return CookState.Half;
        return CookState.Raw;
    }

    void ApplyState(CookState newState)
    {
        state = newState;

        switch (state)
        {
            case CookState.OnPlate:
                meatRenderer.material = meatOnPlateMaterial;
                break;
            case CookState.Raw:
                meatRenderer.material = wholeMaterial;
                break;
            case CookState.Half:
                meatRenderer.material = halfDoneMaterial;
                break;
            case CookState.Brown:
                meatRenderer.material = brownMaterial;
                break;
            case CookState.Burnt:
                meatRenderer.material = burntMaterial;
                SceneManager.LoadScene("GameOverScene");
                isCooking = false; // nothing left to cook
                break;
        }
    }
    public void SendToPot()
    {
        ApplyState(CookState.Raw);
        StartCooking();
    }

    public void SetColor(Color color)
    {
        if (meatRenderer != null && meatRenderer.material != null)
        {
            meatRenderer.material.color = color;
        }
    }

    public void StartCooking() { isCooking = true; }
    public void PauseCooking() { isCooking = false; }

    public void ResetCooking()
    {
        cookTime = 0f;
        isCooking = false;
        ApplyState(CookState.Raw);
    }
}
