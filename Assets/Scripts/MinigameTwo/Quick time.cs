using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Quicktime : MonoBehaviour
{
    public enum Zone { Grey, Red, Yellow, Green }
    public event Action<Zone> Finished;

    [SerializeField] RectTransform Ring;
    [SerializeField] RectTransform Target;
    public float startRatio = 0.9f;
    public float shrinkSpeed = 0.2f;

    [Header("Zone sizes")]
    [SerializeField] private float redRing = 0.90f;
    [SerializeField] private float yellowRing = 0.55f;
    [SerializeField] private float greenRing = 0.33f;
    [SerializeField] private float greyRing = 0.05f;

    private float ratio;
    private bool isTargetActive = false;

    public void Begin()
    {
        ratio = startRatio;
        isTargetActive = true;
        Ring.GetComponent<Image>().enabled = true;
        Target.GetComponent<Image>().enabled = true;
        Resize();
    }
    void Update()
    {
        if (!isTargetActive) return;

        ratio -= shrinkSpeed * Time.deltaTime;
        Resize();

        if (SpacePressed()) { End(GetZone(ratio)); return; }
        if (ratio <= 0f) End(Zone.Grey);
    }
    void End(Zone zone)
    {
        isTargetActive = false;
        Finished?.Invoke(zone);
    }
    Zone GetZone(float ratio)
    {
        if (ratio > redRing) return Zone.Grey;      
        if (ratio > yellowRing) return Zone.Red;
        if (ratio > greenRing) return Zone.Yellow;
        if (ratio > greyRing) return Zone.Green;
        return Zone.Grey;                       
    }
    bool SpacePressed()
    {
        return (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
    }
    public void Resize()
    {
        float rectCorrection = Target.rect.width / Ring.rect.width;
        Ring.localScale = Target.localScale * (Mathf.Max(ratio, 0f) * rectCorrection);
        CenterRing();
    }

    void CenterRing()
    {
        Vector3 targetCenter = Target.TransformPoint(Target.rect.center);
        Vector3 ringOffset = Ring.TransformVector(Ring.rect.center);
        Ring.position = targetCenter - ringOffset;
    }
}
    


