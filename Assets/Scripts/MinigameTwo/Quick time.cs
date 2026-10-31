using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Quicktime : MonoBehaviour
{
    public enum Zone { Grey, Red, Yellow, Green }
    public event Action<Zone> Finished;

    [SerializeField] RectTransform Ring;
    [SerializeField] RectTransform Target;
    public float startRatio = 0.9f;
    public float shrinkSpeed = 0.2f;

    [Header("Zone sizes")]
    [SerializeField] private float redRing = 0.65f;
    [SerializeField] private float yellowRing = 0.55f;
    [SerializeField] private float greenRing = 0.33f;
    [SerializeField] private float greyRing = 0.05f;

    private float ratio;
    private bool isTargetActive = false;
    private float TimeAdded;

    public void Begin()
    {
        Ring.GetComponent<Image>().enabled = false;
        Target.GetComponent<Image>().enabled = false;
    }

    private void StartQuickT()
    {
        ratio = startRatio;
        isTargetActive = true;
        Resize();
    }
    void Update()
    {
        if (!isTargetActive) return;

        ratio -= shrinkSpeed * Time.deltaTime;
        Resize();




        if (ratio <= 0f)
        {
            isTargetActive = false;
            TimeAdded += greyTime;
            
        }


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

    void End(Zone zone)
    {
        isTargetActive = false;
        // get rid of the // if resizing messes something up
       if (ratio > redRing)
        {
            TimeAdded = greyTime;
           // Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > yellowRing)
        {
            TimeAdded = redTime;
            //Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > greenRing)
        {
            TimeAdded = yellowTime;
           // Debug.Log(ratio + "  " + TimeAdded);
        }
       else if (ratio > greyRing)
        {
            TimeAdded = greenTime;
           // Debug.Log(ratio + "  " + TimeAdded);
        }
       else
        {
            TimeAdded = greyTime;
            Debug.Log(ratio + "  " + TimeAdded);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TireJump"))
        {
            Ring.GetComponent<Image>().enabled = true;
            Target.GetComponent<Image>().enabled = true;
            StartQuickT();
        }
    }
}
    


