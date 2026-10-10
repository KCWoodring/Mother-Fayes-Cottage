using TMPro;
using UnityEngine;

public class MeatTimer : MonoBehaviour
{
    [SerializeField] private MeatCooking meatCooking;
    [SerializeField] private TMP_Text meatTimerText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private void Update()
    {
        bool started = meatCooking.isCooking || meatCooking.cookTime > 0f;
        meatTimerText.enabled = started;
        if (!started) return;

        float remaining = Mathf.Max(0f, meatCooking.burntTime - meatCooking.cookTime);
        meatTimerText.text = Mathf.CeilToInt(remaining).ToString();

        bool burningSoon = meatCooking.isCooking && meatCooking.state == MeatCooking.CookState.Brown;
        meatTimerText.color = burningSoon ? warningColor : normalColor;
    }
}