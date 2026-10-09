using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static MeatChopping;

public class Pot : MonoBehaviour
{
    [SerializeField] private string meatTag = "Food";
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -0.05f);

    // Called by MeatToPot
    public void PlaceMeat(GameObject meat)
    {
        if (!meat.CompareTag(meatTag)) return;

        meat.transform.position = transform.position + offset;

        MeatChopping chopping = meat.GetComponent<MeatChopping>();

        if (chopping != null) chopping.canChop = true;
    }
}
