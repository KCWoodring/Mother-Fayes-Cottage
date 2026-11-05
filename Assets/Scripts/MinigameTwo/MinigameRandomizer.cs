using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinigameRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] minigames = new GameObject[4];
    [SerializeField] private Transform[] spawnPoints = new Transform[4];
    [SerializeField] private int[] markerNumbers = { 1, 2, 4, 5 };
    [SerializeField] private bool matchRotation = true;

    [SerializeField] private bool keepOriginalHeight = true;
    [SerializeField] private float originalGroundY = 0f;

    [SerializeField] private int[] flipWalkwayOnMarkers = { 1, 2 };

    private float[] originalY;

    private void Awake()
    {
        originalY = new float[minigames.Length];
        for (int i = 0; i < minigames.Length; i++)
        {
            originalY[i] = minigames[i].transform.position.y;
        }
        Randomize();
    }

    public void Randomize()
    {
        if (minigames.Length != spawnPoints.Length)
        {
            return;
        }
        List<Transform> shuffled = new List<Transform>(spawnPoints);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        for (int i = 0; i < minigames.Length; i++)
        {
            Transform t = minigames[i].transform;
            Vector3 markerPos = shuffled[i].position;

            float y = keepOriginalHeight
                ? originalY[i]
                : originalY[i] + (markerPos.y - originalGroundY);

            t.position = new Vector3(markerPos.x, y, markerPos.z);

            if (matchRotation) t.rotation = shuffled[i].rotation;

            int markerNumber = markerNumbers[Array.IndexOf(spawnPoints, shuffled[i])];

            Walkway walkway = minigames[i].GetComponentInChildren<Walkway>(true);
            if (walkway != null)
                walkway.SetFlipped(Array.IndexOf(flipWalkwayOnMarkers, markerNumber) >= 0);

            MinigameMarkers sign = minigames[i].GetComponentInChildren<MinigameMarkers>(true);
            sign.SetNumber(markerNumber);
            bool startedSouth = markerNumbers[i] <= 2;
            bool nowSouth = markerNumber <= 2;
            sign.SetFlipped(startedSouth != nowSouth);

            bool changedHalf = (markerNumbers[i] <= 2) != (markerNumber <= 2);

            WeavePoles weave = minigames[i].GetComponentInChildren<WeavePoles>(true);
            if (weave != null) weave.SetFlipped(changedHalf);

            TunnelDraw tunnel = minigames[i].GetComponentInChildren<TunnelDraw>(true);
            if (tunnel != null) tunnel.SetFlipped(changedHalf);
        }
        Physics.SyncTransforms();
    }
}
