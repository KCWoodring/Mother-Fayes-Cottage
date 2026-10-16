using UnityEngine;

public class CutTrail : MonoBehaviour
{
    [SerializeField] private float minPointDistance = 0.05f;
    [SerializeField] private float surfaceOffset = 0.02f;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 0;
    }

    public void AddPoint(Vector3 worldPoint, Vector3 normal)
    {
        Vector3 p = worldPoint + normal * surfaceOffset;
        int count = line.positionCount;

        if (count > 0 && Vector3.Distance(line.GetPosition(count - 1), p) < minPointDistance) return;

        line.positionCount = count + 1;
        line.SetPosition(count, p);
    }

    public void Clear()
    {
        line.positionCount = 0;
    }
}
