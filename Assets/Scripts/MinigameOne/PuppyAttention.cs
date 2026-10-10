using Unity.VisualScripting;
using UnityEngine;

public class PuppyAttention : MonoBehaviour
{
    public GameObject mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("PuppyWantsAttention", 20, 20);  
    }

    // Update is called once per frame
    void PuppyWantsAttention()
    {
        this.gameObject.GetComponent<Animator>().SetBool("GettingAttention", true);
        Animator cam = mainCamera.GetComponent<Animator>();
        cam.SetBool("AtPrep", false);
        cam.SetBool("AtCook", false);
        cam.SetBool("AtCounter", true);
        this.gameObject.GetComponent<AudioSource>().Play();
    }
}
