using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Quicktime : MonoBehaviour
{
    [SerializeField] RectTransform Ring;
    [SerializeField] RectTransform Target;
    public float startRatio = 0.9f;
    public float shrinkSpeed = 0.2f;

    [Header("Zone sizes")]
    [SerializeField] private float redRing = 0.65f;
    [SerializeField] private float yellowRing = 0.55f;
    [SerializeField] private float greenRing = 0.42f;
    [SerializeField] private float greyRing = 0.35f;

    [Header("Add Time")]
    public float greyTime;
    public float redTime;
    public float yellowTime;
    public float greenTime;
    private float ratio;
    private bool isTargetActive = false;
    private float TimeAdded; 


    private void Awake()
    {
        ratio = startRatio;
        isTargetActive = true;
        Resize();
    }
    void Update()
    {
        if (!isTargetActive)
            return;
        ratio -= shrinkSpeed * Time.deltaTime;
        Resize();




        if (ratio <= 0f)
        {
            isTargetActive = false;
            TimeAdded += greyTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }


    }
    public void Resize()
    {
        float rectCorrection = Target.rect.width / Ring.rect.width;
        Ring.localScale = Target.localScale * (MathF.Max(ratio, 0f) * rectCorrection);
        centerRing();
    }
    private void centerRing()
    { 
        Vector3 targetCenter = Target.TransformPoint(Target.rect.center);
        Vector3 ringOffset = Ring.TransformVector(Ring.rect.center);
        Ring.position = targetCenter - ringOffset;

    }
    public void HurdleQuickTime(InputAction.CallbackContext context)
    {
        if (!context.performed || !isTargetActive)
            return;
        isTargetActive = false;

       if (ratio > redRing)
        {
            TimeAdded = greyTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > yellowRing)
        {
            TimeAdded = redTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > greenRing)
        {
            TimeAdded = yellowTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > greyRing)
        {
            TimeAdded = greenTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
       else
        {
            TimeAdded = greyTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
    }
}
    


