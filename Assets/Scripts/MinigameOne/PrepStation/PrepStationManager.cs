using UnityEngine;
using UnityEngine.InputSystem;

public class PrepStationManager : MonoBehaviour
{
    public bool cuttingActive = false; // Flag to indicate if cutting is active
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject startCut;
    public GameObject endCut;
    public GameObject cutLine;

    public GameObject blender;

    public Transform blenderDropPoint;

    public Vector3 finalPumpkinTransform;
    public Vector3 finalHoneyDewTransform;
    public Vector3 cutOneStartPoint;
    public Vector3 cutOneEndPoint;
    public Vector3 cutTwoStartPoint;
    public Vector3 cutTwoEndPoint;
    public Vector3 cutThreeStartPoint;
    public Vector3 cutThreeEndPoint;
    public Vector3 cutFourStartPoint;
    public Vector3 cutFourEndPoint;

    public bool startedCut = false;
    public bool pumpkinInBlender = false;
    public bool honeyDewInBlender = false;
    private bool isCuttingPumpkin = false;
    private bool isCuttingHoneyDew = false;
    public bool BlenderNeedsAttention => (cuttingActive && cutAmount == 3) || pumpkinInBlender || honeyDewInBlender;


    private float cutAmount = 0;

    [SerializeField] private Transform[] points;
    [SerializeField] private CutController cutController;
    [SerializeField] private CookingManager cookingManager;

    [Header("Cut Guides (index 0 = Line, 1 = Plus, 2 = Asterisk)")]
    [SerializeField] private CutGuide[] guides;
    [SerializeField] private float cutHitRadius = 0.5f;
    [SerializeField] private CutTrail cutTrail;

    private CutGuide.Segment activeSegment;
    private Transform activeTarget;


    InputAction clickAction;
    InputAction pointAction;

    public GameObject pumpkin;
    public Material halvedPumpkinMaterial;
    public Material quarterPumpkinMaterial;
    public Material eighthPumpkinMaterial;
    public Material pumpkinPuree;

    public GameObject honeyDew;
    public Material halvedHoneyDewMaterial;
    public Material quarterHoneyDewMaterial;
    public Material eighthHoneyDewMaterial;
    public Material honeyDewPuree;

    private AudioSource audioSource;
    public AudioClip blenderSound;
    public AudioClip cuttingSoundP;
    public AudioClip cuttingSoundHD;
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCutDrag();

        if (!clickAction.WasPressedThisFrame()) return;

        Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return;

        string clickedObjectName = hit.collider.gameObject.name;
        Debug.Log("Clicked on: " + clickedObjectName);

        if (clickedObjectName == "PumpkinPuree" && pumpkinInBlender)
        {
            pumpkin.transform.position = finalPumpkinTransform;
            pumpkinInBlender = false;
            Debug.Log("Pumpkin puree successfully moved to final location!");

            if (cookingManager != null) cookingManager.RefreshCurrentStepDrag();
            return;
        }
        if (clickedObjectName == "HoneyDewPuree" && honeyDewInBlender)
        {
            honeyDew.transform.position = finalHoneyDewTransform;
            honeyDewInBlender = false;
            Debug.Log("HoneyDew puree successfully moved to final location!");

            if (cookingManager != null) cookingManager.RefreshCurrentStepDrag();
            return;
        }
    }
    private void CompleteCut()
    {
        if (cutAmount == 0)
        {
            if (isCuttingPumpkin)
            {
                pumpkin.GetComponent<MeshRenderer>().material = halvedPumpkinMaterial;
                if (cuttingSoundP != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundP, Camera.main.transform.position, 1.0f);
                }
            }
            else
            {
                honeyDew.GetComponent<MeshRenderer>().material = halvedHoneyDewMaterial;
                if (cuttingSoundHD != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundHD, Camera.main.transform.position, 1.0f);
                }
            }
            cutAmount = 1;
            CutTwo();
        }
        else if (cutAmount == 1)
        {
            if (isCuttingPumpkin)
            {
                pumpkin.GetComponent<MeshRenderer>().material = quarterPumpkinMaterial;
                if (cuttingSoundP != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundP, Camera.main.transform.position, 1.0f);
                }
            }
            else
            {
                honeyDew.GetComponent<MeshRenderer>().material = quarterHoneyDewMaterial;
                if (cuttingSoundHD != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundHD, Camera.main.transform.position, 1.0f);
                }
            }

            cutAmount = 2;
            CutThree();
        }
        else if (cutAmount == 2)
        {
            if (isCuttingPumpkin)
            {
                pumpkin.GetComponent<MeshRenderer>().material = eighthPumpkinMaterial;
                if (cuttingSoundP != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundP, Camera.main.transform.position, 1.0f);
                }
            }
            else
            {
                honeyDew.GetComponent<MeshRenderer>().material = eighthHoneyDewMaterial;
                if (cuttingSoundHD != null && audioSource != null)
                {
                    Debug.Log("Sound");
                    AudioSource.PlayClipAtPoint(cuttingSoundHD, Camera.main.transform.position, 1.0f);
                }
            }
            cutAmount = 3;
            HideGuides();
        }
    }
    private void HandleCutDrag()
    {
        if (!cuttingActive || cutAmount == 3)
        {
            startedCut = false;
            if (cutTrail != null) cutTrail.Clear();
            return;
        }

        CutGuide guide = guides[(int)cutAmount];

        Ray ray = Camera.main.ScreenPointToRay(pointAction.ReadValue<Vector2>());
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit);

        if (clickAction.WasPressedThisFrame())
        {
            if (hasHit) Debug.Log("Hit " + hit.collider.name + " at " + hit.point);
            CancelDrag();
            if (hasHit && guide.TryBegin(hit.point, cutHitRadius, out activeSegment, out activeTarget))
            {
                startedCut = true;
                if (cutTrail != null) cutTrail.AddPoint(hit.point, hit.normal);
            }
            return;
        }

        if (!startedCut) return;

        if (!clickAction.IsPressed())
        {
            CancelDrag();
            return;
        }

        if (hasHit && cutTrail != null) cutTrail.AddPoint(hit.point, hit.normal);

        if (hasHit && Vector2.Distance(new Vector2(activeTarget.position.x, activeTarget.position.y), new Vector2(hit.point.x, hit.point.y)) < cutHitRadius)
        {
            guide.MarkDone(activeSegment);
            CancelDrag();

            if (guide.AllDone) CompleteCut();
        }
    }
    public void SelectPumpkinToCut()
    {
        isCuttingPumpkin = true;
        cuttingActive = true;
        CutOne();
    }

    public void SelectHoneyDewToCut()
    {
        isCuttingPumpkin = false;
        cuttingActive = true;
        CutOne();
    }

    public void CutOne()
    {
        ShowGuide(0);
    }

    public void CutTwo()
    {
        ShowGuide(1);
    }
    public void CutThree()
    {
        ShowGuide(2);
    }
    public bool CanBlend(bool pumpkinItem)
    {
        if (!cuttingActive || cutAmount != 3) return false;
        if (isCuttingPumpkin != pumpkinItem) return false;
        return pumpkinItem ? !honeyDewInBlender : !pumpkinInBlender;
    }

    public void SendToBlender(bool pumpkinItem)
    {
        if (!CanBlend(pumpkinItem)) return;

        Vector3 blenderSpot = blenderDropPoint.position;

        if (pumpkinItem)
        {
            pumpkin.transform.position = blenderSpot;
            pumpkin.GetComponent<MeshRenderer>().material = pumpkinPuree;
            pumpkin.name = "PumpkinPuree";
            pumpkinInBlender = true;
        }
        else
        {
            honeyDew.transform.position = blenderSpot;
            honeyDew.GetComponent<MeshRenderer>().material = honeyDewPuree;
            honeyDew.name = "HoneyDewPuree";
            honeyDewInBlender = true;
        }

        if (blenderSound != null && audioSource != null)
        {
            Debug.Log("Sound");
            AudioSource.PlayClipAtPoint(blenderSound, Camera.main.transform.position, 1.0f);
        }

        cuttingActive = false;
        HideGuides();
        cutAmount = 0;
    }
    private void ShowGuide(int index)
    {
        for (int i = 0; i < guides.Length; i++) guides[i].gameObject.SetActive(i == index);
        if (index >= 0 && index < guides.Length) guides[index].ResetGuide();
    }

    private void HideGuides()
    {
        foreach (var g in guides) g.gameObject.SetActive(false);
    }
    private void CancelDrag()
    {
        startedCut = false;
        activeSegment = null;
        activeTarget = null;
        if (cutTrail != null) cutTrail.Clear();
    }
}
