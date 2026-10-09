using UnityEngine;
using UnityEngine.UI;
using static Stirring;

public class StirringEvnets : MonoBehaviour
{
    [SerializeField] private Stirring stirring;
    [SerializeField] private Renderer arrow;
    [SerializeField] private Color clockColor = Color.red;
    [SerializeField] private Color counterClockColor = Color.blue;
    [SerializeField] private bool flipForCounterClock = true;

    private Vector3 baseScale;
    private MaterialPropertyBlock block;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        baseScale = arrow.transform.localScale;
    }

    private void OnEnable() { stirring.OnDirectionChange.AddListener(DirectionChange); }
    private void OnDisable() { stirring.OnDirectionChange.RemoveListener(DirectionChange); }

    public void DirectionChange(StirDirection direction)
    {
        bool clock = direction == StirDirection.Clock;
        Color color = clock ? clockColor : counterClockColor;

        arrow.GetPropertyBlock(block);
        block.SetColor(BaseColorId, color);
        block.SetColor(ColorId, color);
        arrow.SetPropertyBlock(block);

        if (flipForCounterClock)
        {
            Vector3 s = baseScale;
            if (!clock) s.x = -s.x;
            arrow.transform.localScale = s;
        }
    }
}
