using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minigame4Manager : MonoBehaviour
{
    public bool dogShaking;
    public List<GameObject> impairingBubbles = new List<GameObject>();
    public List<GameObject> poppedBubbles = new List<GameObject>();
    public GameObject bubble1;
    public GameObject bubble2;
    public GameObject bubble3;
    [SerializeField] private GraphicRaycaster raycaster;    
    [SerializeField] private EventSystem eventSystem;
    InputAction pointAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        impairingBubbles.Add(bubble1);
        impairingBubbles.Add(bubble2);
        impairingBubbles.Add(bubble3);
        StartCoroutine(ShakeDone());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPointerOverSpecificElement(bubble1) && bubble1.activeSelf) 
        {            
            bubble1.SetActive(false); 
            poppedBubbles.Add(bubble1);     
        } 
        if (IsPointerOverSpecificElement(bubble2) && bubble2.activeSelf) 
        {            
            bubble2.SetActive(false); 
            poppedBubbles.Add(bubble2);     
        } 
        if (IsPointerOverSpecificElement(bubble3) && bubble3.activeSelf) 
        {            
            bubble3.SetActive(false); 
            poppedBubbles.Add(bubble3);     
        } 
        if (poppedBubbles.Count > 2)
        {
            dogShaking = false;
            poppedBubbles.Clear();
        }
        if(dogShaking)
        { 
            foreach(GameObject bubble in impairingBubbles)
            {
                bubble.SetActive(true);
            }
            StartCoroutine(ShakeDone());
        }
    }

    IEnumerator ShakeDone()
    {
        dogShaking = false;
        yield return new WaitForSeconds(Random.Range(5,10));
        foreach(GameObject bubble in impairingBubbles)
        {
            bubble.SetActive(true);
        }
        dogShaking = true;
    }

    private bool IsPointerOverSpecificElement(GameObject targetGameObject) 
    {        
        PointerEventData pointerData = new PointerEventData(eventSystem) 
        {            
            position = pointAction.ReadValue<Vector2>()      
        };        
        List<RaycastResult> results = new List<RaycastResult>();        
        raycaster.Raycast(pointerData, results);        
        foreach (RaycastResult raycastResult in results) 
        {
            if (raycastResult.gameObject == targetGameObject) 
            {
                return true;
            }
        }
        return false;    
    }
}
