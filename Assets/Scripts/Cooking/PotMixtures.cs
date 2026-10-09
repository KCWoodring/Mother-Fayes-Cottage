using UnityEngine;

public class PotMixtures : MonoBehaviour
{
    [SerializeField] private Renderer potRenderer;
    [SerializeField] private Material[] mixtures;

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
    }
}