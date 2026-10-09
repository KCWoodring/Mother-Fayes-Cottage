using UnityEngine;
using UnityEngine.InputSystem;

public class PrepStationManager : MonoBehaviour
{

    public bool cuttingActive = false; // Flag to indicate if cutting is active
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject startCut;
    public GameObject endCut;
    public GameObject cutLine;

    public Vector3 cutOneStartPoint;
    public Vector3 cutOneEndPoint;
    public Vector3 cutTwoStartPoint;
    public Vector3 cutTwoEndPoint;
    public bool startedCut = false;

    [SerializeField] private Transform[] points;
    [SerializeField] private CutController cutController;

    InputAction clickAction;
    InputAction pointAction;

    public GameObject pumpkin;
    public Material halvedPumpkinMaterial;


    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clickAction.WasPressedThisFrame() && cuttingActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (!startedCut)
                {
                    if(Vector3.Distance(startCut.transform.position, hit.point) < 1f)
                    {
                        startedCut = true;
                    }
                }
                else
                {
                    if(Vector3.Distance(endCut.transform.position, hit.point) < 1f)
                    {
                        startedCut = false;
                        pumpkin.GetComponent<Renderer>().material = halvedPumpkinMaterial;
                        CutTwo();
                    }
                }
            }
        }
    }

    public void CutOne()
    {
        cutLine.SetActive(true);
        cutController.SetUpLine(points);
        startCut.transform.position = cutOneStartPoint;
        endCut.transform.position = cutOneEndPoint;
    }

    public void CutTwo()
    {
        cutLine.SetActive(true);
        cutController.SetUpLine(points);
        startCut.transform.position = cutTwoStartPoint;
        endCut.transform.position = cutTwoEndPoint;
    }
}
