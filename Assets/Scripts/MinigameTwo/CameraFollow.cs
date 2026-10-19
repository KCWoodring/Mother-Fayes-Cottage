using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    public GameObject dog;

    void Update() 
    {
        Vector3 pos = new Vector3(dog.transform.position.x, transform.position.y, dog.transform.position.z);
        transform.position = pos;
    }
}
