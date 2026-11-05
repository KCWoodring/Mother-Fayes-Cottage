using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerMiniGame3 : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string name;
        public GameObject roomObject;
        public RectTransform[] hotspots;   // invisible rect over this room on the map
        public RectTransform markerSpot;

        [Header("Gameplay")]
        public HidingSpot[] hidingSpots;
        public bool hintOnLeft; // for kitchen and bedroom
    }
    [System.Serializable]
    public class HidingSpot
    {
        public Button button;
        public GameObject dogPopup;
        public GameObject gotchaImage;
    }

    [Header("Rooms")]
    public Room[] rooms;
    public int startingRoom = 0;

    [Header("Map")]
    public RectTransform map;
    public Image mapImage;
    public RectTransform youMarker;

    [Header("Hover Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor;
    public float hoverScaleMultiplier = 1.05f;
    public float expandedScaleMultiplier = 2.5f;
    public float animateTime = 0.15f;

    [Header("Gameplay")]
    private int dogSpot;
    public GameObject hintTopRight;
    public GameObject hintTopLeft;
    public int blinkCount = 3;
    public float blinkInterval = 0.25f;
    public float gotchaDelay = 2f;
    public string nextSceneName;
    public bool allowDogInStartingRoom = true;

    private int currentRoom;
    private int dogRoom;
    private bool gameOver;
    private bool spotArmed;
    private Coroutine hintRoutine;

    private Vector3 basePosition;
    private Vector3 centerPosition;
    private Vector3 baseScale;
    private bool isHovering;
    private bool isExpanded;
    private Coroutine scaleRoutine;
    private Canvas canvas;

    void Awake()
    {
        var hidingSpotsChoices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].hidingSpots == null || rooms[i].hidingSpots.Length == 0) continue;
            if (!allowDogInStartingRoom && i == startingRoom) continue;
            hidingSpotsChoices.Add(i);
        }

        dogRoom = hidingSpotsChoices[Random.Range(0, hidingSpotsChoices.Count)];
        dogSpot = Random.Range(0, rooms[dogRoom].hidingSpots.Length);

        Debug.Log("Dog is hiding in: " + rooms[dogRoom].name + " (spot " + dogSpot + ")");
    }

    void Start()
    {
        baseScale = map.localScale;
        basePosition = map.position;

        canvas = map.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        centerPosition = canvasRect.TransformPoint(canvasRect.rect.center);

        if (mapImage == null) mapImage = map.GetComponent<Image>();
        ShowRoom(startingRoom);
        UpdateVisuals();

        Button spot = rooms[dogRoom].hidingSpots[dogSpot].button;
        spot.onClick.AddListener(OnDogFound);
    }
    //Map
    public void OnMapHover(bool hovering)
    {
        if (gameOver) return;

        isHovering = hovering;
        UpdateVisuals();
    }

    public void OnMapClicked(PointerEventData eventData)
    {
        if (gameOver) return;

        if (!isExpanded)
        {
            isExpanded = true;
            UpdateVisuals();
            return;
        }
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Debug.Log("Clicked at " + eventData.position);

        for (int i = 0; i < rooms.Length; i++)
        {
            if (RoomContainsPoint(rooms[i], eventData.position, cam))
            {
                Debug.Log("Matched room: " + rooms[i].name);
                ShowRoom(i);
                break;
            }
        }
        isExpanded = false;
        UpdateVisuals();
    }
    bool RoomContainsPoint(Room room, Vector2 screenPoint, Camera cam)
    {
        if (room.hotspots == null) return false;

        foreach (RectTransform rect in room.hotspots)
        {
            if (rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, cam))
                return true;
        }
        return false;
    }
    public void ShowRoom(int index)
    {
        currentRoom = index;
        spotArmed = false;
        ClearFeedback();

        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].roomObject.SetActive(i == index);
        }  
        Room room = rooms[index];
        RectTransform spot = room.markerSpot != null ? room.markerSpot : (room.hotspots != null && room.hotspots.Length > 0 ? room.hotspots[0] : null);

        youMarker.position = spot.TransformPoint(spot.rect.center);
    }
    void UpdateVisuals()
    {
        float multiplier = isExpanded ? expandedScaleMultiplier: isHovering ? hoverScaleMultiplier: 1f;

        Color color = (isHovering || isExpanded) ? hoverColor : normalColor;
        color.a = 1f;
        mapImage.color = color;

        Vector3 targetPos = isExpanded ? centerPosition : basePosition;

        if (isExpanded) map.SetAsLastSibling(); // keep it above everything while open

        if (scaleRoutine != null)  StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(AnimateTo(targetPos, baseScale * multiplier));
    }

    IEnumerator AnimateTo(Vector3 targetPos, Vector3 targetScale)
    {
        Vector3 startPos = map.position;
        Vector3 startScale = map.localScale;
        float time = 0f;

        while (time < animateTime)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, time / animateTime);
            map.position = Vector3.Lerp(startPos, targetPos, k);
            map.localScale = Vector3.Lerp(startScale, targetScale, k);
            yield return null;
        }

        map.position = targetPos;
        map.localScale = targetScale;
    }
    // Gameplay
    public void OnSqueakyToyClicked()
    {
        if (gameOver || isExpanded) return;

        ClearFeedback();

        if (currentRoom == dogRoom)
        {
            spotArmed = true;
            hintRoutine = StartCoroutine(FlashSpot(rooms[dogRoom].hidingSpots[dogSpot].button));
        }
        else
        {
            GameObject hint = rooms[currentRoom].hintOnLeft ? hintTopLeft : hintTopRight;
            hintRoutine = StartCoroutine(FlashObject(hint));
        }
    }

    void OnDogFound()
    {
        if (gameOver || !spotArmed || currentRoom != dogRoom) return;
        gameOver = true;
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        ClearFeedback();

        HidingSpot spot = rooms[dogRoom].hidingSpots[dogSpot];
        if (spot.dogPopup != null) spot.dogPopup.SetActive(true);
        if (spot.gotchaImage != null) spot.gotchaImage.SetActive(true);

        yield return new WaitForSecondsRealtime(gotchaDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FlashObject(GameObject obj)
    {
        if (obj == null) yield break;
        for (int i = 0; i < blinkCount; i++)
        {
            obj.SetActive(true);
            yield return new WaitForSecondsRealtime(blinkInterval);
            obj.SetActive(false);
            yield return new WaitForSecondsRealtime(blinkInterval);
        }
    }

    IEnumerator FlashSpot(Button spot)
    {
        if (spot == null) yield break;
        Graphic g = spot.targetGraphic;
        for (int i = 0; i < blinkCount; i++)
        {
            SetAlpha(g, 1f);
            yield return new WaitForSecondsRealtime(blinkInterval);
            SetAlpha(g, 0f);
            yield return new WaitForSecondsRealtime(blinkInterval);
        }
    }

    void ClearFeedback()
    {
        if (hintRoutine != null) StopCoroutine(hintRoutine);
        if (hintTopLeft != null) hintTopLeft.SetActive(false);
        if (hintTopRight != null) hintTopRight.SetActive(false);

        foreach (Room room in rooms)
        {
            if (room.hidingSpots == null) continue;
            foreach (HidingSpot s in room.hidingSpots)
            {
                if (s.button != null) SetAlpha(s.button.targetGraphic, 0f);
                if (s.dogPopup != null) s.dogPopup.SetActive(false);
                if (s.gotchaImage != null) s.gotchaImage.SetActive(false);
            }
        }
    }

    void SetAlpha(Graphic g, float a)
    {
        if (g == null) return;
        Color c = g.color;
        c.a = a;
        g.color = c;
    }
    void OnDrawGizmos()
    {
        if (rooms == null) return;
        Color[] colors = { Color.green, Color.cyan, Color.yellow, Color.magenta };
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].hotspots == null) continue;
            Gizmos.color = colors[i % colors.Length];
            foreach (var r in rooms[i].hotspots)
            {
                if (r == null) continue;
                Vector3[] c = new Vector3[4];
                r.GetWorldCorners(c);
                for (int k = 0; k < 4; k++)
                    Gizmos.DrawLine(c[k], c[(k + 1) % 4]);
            }
        }
    }
}
