using UnityEngine;
using UnityEngine.Events;

public class ObstacleCameraZoom : MonoBehaviour
{
    [SerializeField] private CourseCamera courseCamera;
    [SerializeField] private Transform cameraView;
    public UnityEvent onDogEntered;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Dog")) return;

        courseCamera.ShowObstacle(cameraView);
        onDogEntered.Invoke();
        GetComponent<Collider>().enabled = false;   // only fire once
    }
    public void MakeSolid()
    {
        col.isTrigger = false;
        col.enabled = true;
    }
}
