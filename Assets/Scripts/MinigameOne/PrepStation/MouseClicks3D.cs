using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseClicks3D : MonoBehaviour
{
    InputAction clickAction;
    InputAction pointAction;
    Animator animator;
    string clickedObjectName;
    GameObject clickedObject;
    GameObject lastClickedObject;
    
    public GameObject puppy;

    public GameObject prepStationZone;
    public GameObject cookStationZone;

    Animator recipeAnimator;

    public PrepStationManager prepStationManager; // Reference to the PrepStationManager script

    // Start is called before the first frame update
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool atPrep = animator.GetBool("AtPrep");
        prepStationManager.enabled = atPrep;
        if (prepStationZone != null) prepStationZone.SetActive(!atPrep);

        if (clickAction.WasPressedThisFrame())
        {
            clickedObject = GetClickedObject(out RaycastHit hit);
            if (recipeAnimator != null && recipeAnimator.GetBool("Open") && (clickedObject == null || clickedObject.name != "Recipe"))
            {
                recipeAnimator.SetBool("Open", false);
                return;
            }
            if(clickedObject == null) { return; }
            clickedObjectName = clickedObject.name;
            Debug.Log("Clicked on: " + clickedObjectName);

            bool puppyWantsAttention = puppy != null && puppy.GetComponent<Animator>().GetBool("GettingAttention");

            
            if ("Toy" == clickedObjectName)
            {
                puppy.GetComponent<Animator>().SetBool("GettingAttention", false);
            }     
            if ("Counter" == clickedObjectName)
            {
                ZoomToCounter();
            }
            if (!puppyWantsAttention)
            {
                if ("Recipe" == clickedObjectName)
                {
                    recipeAnimator = clickedObject.GetComponent<Animator>();

                    if (clickedObject.GetComponent<Animator>().GetBool("Open"))
                    {
                        clickedObject.GetComponent<Animator>().SetBool("Open", false);
                    }
                    else
                    {
                        clickedObject.GetComponent<Animator>().SetBool("Open", true);
                    }
                }
                if (clickedObject == prepStationZone || "PrepStation" == clickedObjectName)
                {
                    animator.SetBool("AtCounter", false);
                    animator.SetBool("AtCook", false);
                    animator.SetBool("AtPrep", true);
                }
                if (clickedObject == cookStationZone || "CookStation" == clickedObjectName)
                {
                    animator.SetBool("AtCounter", false);
                    animator.SetBool("AtPrep", false);
                    animator.SetBool("AtCook", true);
                }
            }
            lastClickedObject = clickedObject;
        }
    }
    void ZoomToCounter()
    {
        animator.SetBool("AtPrep", false);
        animator.SetBool("AtCook", false);
        animator.SetBool("AtCounter", true);
        prepStationManager.enabled = false;
    }

    bool IsCuttable(GameObject item)
    {
        return item != null && (item.name == "Pumpkin" || item.name == "HoneyDew");
    }

    bool IsBoard(GameObject item)
    {
        return item != null && (item.name == "CuttingBoard");
    }

    bool IsPrepObject(GameObject item)
    {
        if (IsCuttable(item) || IsBoard(item)) return true;
        if (item.name == "PumpkinPuree" || item.name == "HoneyDewPuree") return true;

        GameObject blender = prepStationManager.blender;
        return blender != null && (item == blender || item.transform.IsChildOf(blender.transform));
    }

    void PlaceOnBoard(GameObject item, GameObject board)
    {
        item.transform.position = board.transform.position + new Vector3(0, 1, -.05f);
        prepStationManager.cuttingActive = true;

        if (item.name == "Pumpkin") prepStationManager.SelectPumpkinToCut();
        else prepStationManager.SelectHoneyDewToCut();
    }

    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        var ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            if (!isPointerOverUIObject()) { target = hit.collider.gameObject; }
        }
        return target;
    }
    private bool isPointerOverUIObject()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(pointAction.ReadValue<Vector2>().x, pointAction.ReadValue<Vector2>().y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }
}