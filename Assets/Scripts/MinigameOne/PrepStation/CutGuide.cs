using UnityEngine;

public class CutGuide : MonoBehaviour
{
    [System.Serializable]
    public class Segment
    {
        public Transform start;
        public Transform end;
        [HideInInspector] public bool done;
        [HideInInspector] public LineRenderer highlight;
    }

    public Segment[] segments;

    [Header("Highlight")]
    public Material highlightMaterial;
    public Color highlightColor = Color.green;
    public float highlightWidth = 0.05f;
    public float surfaceOffset = 0.02f;

    public bool AllDone
    {
        get
        {
            foreach (var s in segments) if (!s.done) return false;
            return true;
        }
    }

    public void ResetGuide()
    {
        foreach (var s in segments)
        {
            s.done = false;
            if (s.highlight != null) s.highlight.gameObject.SetActive(false);
        }
    }

    // Did the player press near the end of an uncut segment Returns the segment and the end they need to reach.
    public bool TryBegin(Vector3 point, float radius, out Segment seg, out Transform target)
    {
        foreach (var s in segments)
        {
            Debug.Log("start " + s.start.position + " end " + s.end.position + " click " + point);
            if (s.done) continue;
            if (Dist2D(point, s.start.position) < radius) { seg = s; target = s.end; return true; }
            if (Dist2D(point, s.end.position) < radius) { seg = s; target = s.start; return true; }
        }
        seg = null; target = null;
        return false;
    }

    public void MarkDone(Segment s)
    {
        s.done = true;

        if (s.highlight == null)
        {
            var go = new GameObject("CutHighlight");
            go.transform.SetParent(transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.material = highlightMaterial;
            lr.startColor = lr.endColor = highlightColor;
            lr.startWidth = lr.endWidth = highlightWidth;
            s.highlight = lr;
        }

        Vector3 up = transform.forward * -surfaceOffset; // adjust if the guide faces a different way
        s.highlight.SetPosition(0, s.start.position + up);
        s.highlight.SetPosition(1, s.end.position + up);
        s.highlight.gameObject.SetActive(true);
    }
    static float Dist2D(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
    }

    // Shows the segments in the Scene view
    private void OnDrawGizmos()
    {
        if (segments == null) return;
        Gizmos.color = Color.yellow;
        foreach (var s in segments)
            if (s.start != null && s.end != null) Gizmos.DrawLine(s.start.position, s.end.position);
    }
}