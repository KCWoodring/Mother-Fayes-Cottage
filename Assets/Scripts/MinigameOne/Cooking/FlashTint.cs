using UnityEngine;

public class FlashTint : MonoBehaviour
{
    public Color idleColor = Color.white;
    public Color flashColor = Color.green;
    public float flashSpeed = 2f;

    private Renderer[] targets;
    private MaterialPropertyBlock block;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        targets = GetComponentsInChildren<Renderer>();
    }
    private void Update()
    {
        if (gameObject.name == "Pumpkin" || gameObject.name == "HoneyDew")
        {
            ApplyColor(idleColor);
            return;
        }

        Color color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        foreach (Renderer target in targets)
        {
            if (target == null) continue;

            SpriteRenderer sprite = target as SpriteRenderer;
            if (sprite != null)
            {
                sprite.color = color;
                continue;
            }

            target.GetPropertyBlock(block);
            block.SetColor(BaseColorId, color);
            block.SetColor(ColorId, color);
            target.SetPropertyBlock(block);
        }
    }

    private void OnDisable()
    {
        if (targets == null) return;
        ApplyColor(idleColor);
    }
}