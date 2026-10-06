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
        if (clickAction.WasPressedThisFrame())
        {
            clickedObjectName = GetClickedObject(out RaycastHit hit).name;
            if ("AssemblyStation" == clickedObjectName)
            {
                animator.SetBool("AtCounter", false);
                animator.SetBool("AtPrep", false);
                animator.SetBool("AtCook", false);
                animator.SetBool("AtAssembly", true);
            }
            if ("Counter" == clickedObjectName)
            {
                animator.SetBool("AtAssembly", false);
                animator.SetBool("AtPrep", false);
                animator.SetBool("AtCook", false);
                animator.SetBool("AtCounter", true);
            }
            if ("PrepStation" == clickedObjectName)
            {
                animator.SetBool("AtCounter", false);
                animator.SetBool("AtAssembly", false);
                animator.SetBool("AtCook", false);
                animator.SetBool("AtPrep", true);
            }
            if ("CookStation" == clickedObjectName)
            {
                animator.SetBool("AtCounter", false);
                animator.SetBool("AtAssembly", false);
                animator.SetBool("AtPrep", false);
                animator.SetBool("AtCook", true);
            }
        }
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