using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayer;
    InputAction pointAction;
    public float speed = 5f;

    void Start()
    {
        
        pointAction = InputSystem.actions.FindAction("Point");
    } 
   
    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Keep the character upright

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
            }
        }

        
        transform.position += transform.forward * (speed * Time.deltaTime);
    }
}
