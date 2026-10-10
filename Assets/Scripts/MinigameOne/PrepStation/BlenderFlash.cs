using UnityEngine;

public class BlenderFlash : MonoBehaviour
{
    [SerializeField] private PrepStationManager prepStationManager;
    [SerializeField] private Renderer target;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color flashColor = Color.green;
    [SerializeField] private float flashSpeed = 2f;

    private MaterialPropertyBlock block;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        if (target == null) target = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        Color color = idleColor;

        if (prepStationManager.BlenderNeedsAttention)
            color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));

        SetColor(color);
    }

    private void SetColor(Color color)
    {
        SpriteRenderer sprite = target as SpriteRenderer;
        if (sprite != null)
        {
            sprite.color = color;
            return;
        }

        target.GetPropertyBlock(block);
        block.SetColor(BaseColorId, color);
        block.SetColor(ColorId, color);
        target.SetPropertyBlock(block);
    }
}