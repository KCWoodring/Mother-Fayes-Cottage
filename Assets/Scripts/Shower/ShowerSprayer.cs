using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ShowerSprayer : MonoBehaviour
{
    public float radius = 60f;         
    public LayerMask sudMask;          
    public GameObject flyingSudPrefab; 
    public Canvas canvas;

    public float sprayRinse = 0.25f;   
    public float blastRinse = 0.2f;    
    public float blastInterval = 0.12f;
    public Image ShowerHead;

    public Sprite ShowerHOn;
    public Sprite ShowerHOff;

    float nextBlast;

    void OnEnable() => Cursor.visible = false;  
    void OnDisable() => Cursor.visible = true;

    void Update()
    {
        var mouse = Mouse.current;

        
        transform.position = mouse.position.ReadValue();

        bool spray = mouse.leftButton.isPressed;
        bool blast = mouse.rightButton.isPressed;
        if (!spray && !blast) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, radius, sudMask);

        if (blast && Time.time >= nextBlast)
        {
            ShowerHead.sprite = ShowerHOn;
            nextBlast = Time.time + blastInterval;
            foreach (var h in hits)
            {
                var spot = h.GetComponent<SudSpot>();
                spot.Rinse(blastRinse);
                var sud = Instantiate(flyingSudPrefab, canvas.transform);
                sud.transform.position = spot.transform.position;
            }
        }
        else if (spray && !blast)
        {
            foreach (var h in hits)
                h.GetComponent<SudSpot>().Rinse(sprayRinse * Time.deltaTime);
            ShowerHead.sprite = ShowerHOn;

        }
        else if (!spray||!blast)
        {
            ShowerHead.sprite = ShowerHOff;
        }
    }

    void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(transform.position, radius);
}