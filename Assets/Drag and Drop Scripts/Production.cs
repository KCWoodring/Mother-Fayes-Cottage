using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Production : MonoBehaviour
{
    [SerializeField] private Image ProductpreFab;
    [SerializeField] private RectTransform rectTransformProduct;
    [SerializeField] private Canvas Productcanvas;
   
    
    private void Awake()
    {
        rectTransformProduct = GetComponent<RectTransform>();
        
        
        
    }

    public void ProduceItem()
    {
       Instantiate(ProductpreFab, Productcanvas.transform);
        
        rectTransformProduct.SetParent(Productcanvas.transform, true);
       
    }
}
