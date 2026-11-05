using UnityEngine;

public class MinigameMarkers : MonoBehaviour
{
    [SerializeField] private Renderer signRenderer;
    [SerializeField] private Material[] numberMaterials;
    [SerializeField] private int materialSlot = 0;

    [SerializeField] private Vector3 flipAxis = Vector3.up;
    private Quaternion startRotation;
    private bool captured;

    [SerializeField] private Transform flippedSpot;
    private float startHeight;
    private Vector3 startLocalPosition;
    [SerializeField] private float flippedLift = 0f;


    private void Awake()
    {
        if (signRenderer == null)
            signRenderer = GetComponentInChildren<Renderer>(true);
    }

    public void SetNumber(int number)
    {
        int index = number - 1;


        Material[] mats = signRenderer.sharedMaterials;
        mats[materialSlot] = numberMaterials[index];
        signRenderer.sharedMaterials = mats;
    }
    public void SetFlipped(bool flip)
    {
        if (signRenderer == null)
            signRenderer = GetComponentInChildren<Renderer>(true);
        if (signRenderer == null) return;

        Transform card = signRenderer.transform;
        if (!captured)
        {
            startHeight = transform.InverseTransformPoint(card.position).y;
            startRotation = card.localRotation;
            captured = true;
        }

        card.localRotation = flip ? startRotation * Quaternion.AngleAxis(180f, flipAxis): startRotation;

        if (flip && flippedSpot != null)
        {
            Vector3 position = transform.InverseTransformPoint(flippedSpot.position);
            position.y = startHeight;
            card.position = transform.TransformPoint(position) + Vector3.up * flippedLift;
        }
        else
        {
            card.localPosition = startLocalPosition;
        }
    }
}
