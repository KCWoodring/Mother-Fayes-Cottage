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
    private Vector3 scaleChange;
    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleChange = new Vector3(-0.01f, -0.01f, -0.01f);
        originalScale = new Vector3(10.718f, 10.718f, 10.718f);
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
            bubble1.transform.localScale += scaleChange;
            if(bubble1.transform.localScale.x <= 1)
            {
                bubble1.SetActive(false); 
                poppedBubbles.Add(bubble1);                
                bubble1.transform.localScale = originalScale;    
            }      
        } 
        if (IsPointerOverSpecificElement(bubble2) && bubble2.activeSelf) 
        {            
            bubble2.transform.localScale += scaleChange;
            if(bubble2.transform.localScale.x <= 1)
            {
                bubble2.SetActive(false); 
                poppedBubbles.Add(bubble2);
                bubble2.transform.localScale = originalScale; 
            }  
        } 
        if (IsPointerOverSpecificElement(bubble3) && bubble3.activeSelf) 
        {            
            bubble3.transform.localScale += scaleChange;
            if(bubble3.transform.localScale.x <= 1)
            {
                bubble3.SetActive(false); 
                poppedBubbles.Add(bubble3);                
                bubble3.transform.localScale = originalScale;    
            }      
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
        bubble1.transform.localScale = originalScale;  
        bubble2.transform.localScale = originalScale;  
        bubble3.transform.localScale = originalScale;  
        yield return new WaitForSeconds(Random.Range(15,30));
        foreach(GameObject bubble in impairingBubbles)
        {
            bubble.SetActive(true);
            bubble.GetComponent<Animator>().Play("IncreaseSize");
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
