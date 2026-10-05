using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

public class Stirring : MonoBehaviour, IPointerUpHandler,IPointerDownHandler, IDragHandler
{
    public enum StirDirection { Clock, CounterClock}

    [Header("Refrence Tings")]
    [SerializeField] private RectTransform Soup;
    [SerializeField] private Camera camera;
    [SerializeField] private StirringEvnets stiiringEvents;

    [Header("Rotation Tings")]
    public StirDirection currentDirection;
    private bool isStirring, compleeted;
    private float LastAngle;
    public float TotalRotation;
    private PointerEventData pointer;
    public float StirProg;

    [Header("Modifying Tings")]
    [SerializeField]private float CenterArea = 5f;
    [SerializeField] private float VisSpeed;
    [SerializeField] private float StirRounds;
    [SerializeField] private float Laps;
    [SerializeField] private float SwitchChance;
    [SerializeField] private StirDirection StartDirection = StirDirection.Clock;
    [SerializeField] private float Penalty;
    
    [Header("Events")]
    public UnityEvent onStirredCompleet;
    public UnityEvent<StirDirection> OnDirectionChange;
    public UnityEvent OnSoupCompleet;

    [Header("Progress Indicators")]
    public float StirProgress => Mathf.Clamp01(StirProg / (360f * Laps));
    
    private int currentRound;
    public StirDirection CurrentDirection => currentDirection;
    public int CurrentRound => currentRound;

    private void Start()
    {
        currentDirection = StartDirection;
        OnDirectionChange?.Invoke(currentDirection);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isStirring || compleeted)
        {
            return;
        }

        Vector2 center = GetCenter();
        if ((eventData.position - center).sqrMagnitude < CenterArea * CenterArea)
        {
            return;
        }
       float angle = GetMouseAngle(eventData.position);
        float Delta = Mathf.DeltaAngle(LastAngle, angle);
        LastAngle = angle;
        
        

        Soup.Rotate(0f, 0f, Delta * VisSpeed);

        bool correctWay = (currentDirection == StirDirection.CounterClock) == Delta > 0f;
        if (correctWay)
        {
            StirProg += Mathf.Abs(Delta);
        }
        else
        {
            StirProg = Mathf.Max(0f,StirProg - Mathf.Abs(Delta) ); //add *penalty if we want ti
        }
        TotalRotation += Delta;
        if(StirProg >= 360* Laps)
        {
            StirringCompleet();
        }
    }

    private void StirringCompleet()
    {
        currentRound++;
        StirProg = 0f;
        onStirredCompleet?.Invoke();

        if(currentRound >= StirRounds)
        {
            compleeted = true;
            isStirring = false;
            OnSoupCompleet?.Invoke();
            return;
        }
        if(Random.value < SwitchChance)
        {
            switch (currentDirection)
            {
                case StirDirection.CounterClock:
                    currentDirection = StirDirection.Clock;
                    break;
                case StirDirection.Clock:
                    currentDirection = StirDirection.CounterClock;
                    break;
            }
            
            OnDirectionChange?.Invoke(currentDirection);
            stiiringEvents.DirectionChange();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isStirring = true;
        LastAngle = GetMouseAngle(eventData.position);
        pointer = eventData;

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isStirring = false;
    }
    
  
    private Vector2 GetCenter()
    {
        Canvas canvas = Soup.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        return Soup.position;
        return camera.WorldToScreenPoint(Soup.position);
    }
    private float GetMouseAngle(Vector2 screenPos)
    {
        
        Vector2 toMouse = screenPos - GetCenter();
        return Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;
    }

}
