using UnityEngine;
using UnityEngine.UI;

public class MeatCooking : MonoBehaviour
{
    public enum CookState { Raw, Half, Brown, Burnt }

    [Header("Timing (seconds since cooking started)")]
    public float halfTime = 10f;
    public float brownTime = 20f;
    public float burntTime = 30f;

    [Header("Visuals")]
    public Image meatImage;

    //Place holder till we get assets
    public Color rawColor = new Color(1f, 0.6f, 0.7f);    // pink
    public Color halfColor = new Color(0.8f, 0.5f, 0.45f); // pink/brown mix
    public Color brownColor = new Color(0.45f, 0.27f, 0.12f);
    public Color burntColor = Color.black;

    //Actual usage once assets are in
    // public Sprite rawSprite;
    // public Sprite halfSprite;
    // public Sprite brownSprite;
    // public Sprite burntSprite;

    public float cookTime { get; private set; }
    public bool isCooking { get; private set; }
    public CookState state { get; private set; } = CookState.Raw;

    void Start()
    {
        ApplyState(CookState.Raw);
    }

    void Update()
    {
        if (!isCooking) return;

        cookTime += Time.deltaTime;

        CookState newState = GetStateForTime(cookTime);
        if (newState != state)
            ApplyState(newState);
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
            case CookState.Raw:
                meatImage.color = rawColor;
                // meatImage.sprite = rawSprite;
                break;
            case CookState.Half:
                meatImage.color = halfColor;
                // meatImage.sprite = halfSprite;
                break;
            case CookState.Brown:
                meatImage.color = brownColor;
                // meatImage.sprite = brownSprite;
                break;
            case CookState.Burnt:
                meatImage.color = burntColor;
                // meatImage.sprite = burntSprite;
                isCooking = false; // nothing left to cook
                break;
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
