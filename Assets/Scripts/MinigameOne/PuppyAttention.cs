using UnityEngine;

public class PuppyAttention : MonoBehaviour
{
    public GameObject mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("puppyWantsAttention", 30, 30);  
    }

    // Update is called once per frame
    void puppyWantsAttention()
    {
        this.gameObject.GetComponent<Animator>().SetBool("GettingAttention", true);
        mainCamera.GetComponent<Animator>().SetBool("AtPrep", false);
        mainCamera.GetComponent<Animator>().SetBool("AtCounter", false);
        mainCamera.GetComponent<Animator>().SetBool("AtCounter", true);
        this.gameObject.GetComponent<AudioSource>().Play();
    }
}
