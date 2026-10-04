using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnClick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    
    [SerializeField] private Image ProductpreFab;
    [SerializeField] private RectTransform rectTransformProduct;
    [SerializeField] private Canvas Productcanvas;
    [SerializeField] private float MaxSpwns;
    [SerializeField] private RectTransform AnchoredPlace;
    private float currentSpawns;

   
    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(ProduceItem());
    }
    private void Awake()
    {
        rectTransformProduct = GetComponent<RectTransform>();

        Productcanvas = GetComponentInParent<Canvas>();
        transform.parent = GetComponentInParent<RectTransform>();
        

    }
    private void Start()
    {
        AnchoredPlace = GetComponentInParent<RectTransform>();
    }

    public IEnumerator ProduceItem()
    {
        if (currentSpawns < MaxSpwns)
        {
            yield return new WaitForSeconds(0.1f);
                Instantiate(ProductpreFab, AnchoredPlace.position, Quaternion.identity, Productcanvas.transform);

                

                currentSpawns++;
            
        }
        if (currentSpawns == MaxSpwns)
        {
            Destroy(gameObject);
        }
        yield return new Null();
        
    }
}
