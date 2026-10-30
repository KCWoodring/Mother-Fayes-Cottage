using UnityEngine;
using UnityEngine.UI;

public class FlashTintUI : MonoBehaviour
{
    public Color idleColor = Color.white;
    public Color flashColor = Color.green;
    public float flashSpeed = 2f;

    private Image image;
    private bool flashing = true;

    private void Awake() => image = GetComponent<Image>();

    private void Update()
    {
        if (!flashing) return;
        image.color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
    }
    public void SetFlashing(bool on)
    {
        flashing = on;
        if (!on) image.color = idleColor;
    }

    private void OnDisable()
    {
        if (image != null) image.color = idleColor;
    }
}
