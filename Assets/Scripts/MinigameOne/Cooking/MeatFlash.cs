using UnityEngine;

public class MeatFlash : MonoBehaviour
{
    [SerializeField] private MeatChopping chopping;
    [SerializeField] private Renderer target;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color flashColor = Color.green;
    [SerializeField] private float flashSpeed = 2f;

    private MaterialPropertyBlock block;
    private bool wasFlashing;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        if (chopping == null) chopping = GetComponent<MeatChopping>();
        if (target == null) target = GetComponent<Renderer>();
    }

    private void Update()
    {
        bool shouldFlash = chopping.canChop && chopping.state != MeatChopping.ChopState.Bits;

        if (shouldFlash)
        {
            Color color = Color.Lerp(idleColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
            target.GetPropertyBlock(block);
            block.SetColor(BaseColorId, color);
            block.SetColor(ColorId, color);
            target.SetPropertyBlock(block);
            wasFlashing = true;
        }
        else if (wasFlashing)
        {
            target.SetPropertyBlock(null);
            wasFlashing = false;
        }
    }
}