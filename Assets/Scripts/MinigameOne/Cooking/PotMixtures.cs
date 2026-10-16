using UnityEngine;

public class PotMixtures : MonoBehaviour
{
    [SerializeField] private Renderer potRenderer;
    [SerializeField] private Material[] mixtures;

    private MaterialPropertyBlock block;
    private static readonly int AngleId = Shader.PropertyToID("_Angle");

    private void Awake()
    {
        HideAll();
    }

    public void HideAll()
    {
        potRenderer.enabled = false;
    }

    public void ShowStage(int index)
    {
        if (index < 0 || index >= mixtures.Length) return;
        Debug.Log("ShowStage " + index + " of " + mixtures.Length);

        potRenderer.material = mixtures[index];
        potRenderer.enabled = true;
        SetSwirlAngle(0f);
    }

    public void SetSwirlAngle(float radians)
    {
        if (block == null) block = new MaterialPropertyBlock();
        potRenderer.GetPropertyBlock(block);
        block.SetFloat(AngleId, radians);
        potRenderer.SetPropertyBlock(block);
    }
}