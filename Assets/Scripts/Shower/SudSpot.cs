using UnityEngine;


public class SudSpot : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [Range(0f, 1f)] public float amount = 1f;

    private Vector3 fullScale = Vector3.one;

    public bool IsEmpty => amount <= 0f;

    private void Awake()
    {
        if (visual != null) fullScale = visual.localScale;
        Check();
    }

    
    public bool Rinse(float delta)
    {
        if (IsEmpty) return false;
        amount = Mathf.Max(0f, amount - delta);
        Check();
        return true;
    }

   

    private void Check()
    {
        if (visual == null) return;
        visual.localScale = fullScale * amount;
        if (amount > 0.08f) return;
        Destroy(this.gameObject);
        
        
    }
   


}