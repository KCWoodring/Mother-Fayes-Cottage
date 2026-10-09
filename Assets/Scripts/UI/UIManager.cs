using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject instructionsPage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OpenInstructions()
    {
        instructionsPage.SetActive(true);
    }

    public void CloseInstructions()
    {
        instructionsPage.SetActive(false);
    }
}
