using UnityEngine;
using System.Collections;
using TMPro;

public class MinigameCounter : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private float tiltDegrees = -6f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color flashColor;
    [SerializeField] private Color finalColor;

    [SerializeField] private float popScale = 1.6f;
    [SerializeField] private float popDuration = 0.45f;

    private Vector3 baseScale;

    private TMP_Text label;
    private Coroutine popRoutine;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
        label.richText = true;
        label.fontStyle = FontStyles.Bold | FontStyles.Italic;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.outlineWidth = 0.35f;
        label.outlineColor = Color.black;

        transform.localRotation = Quaternion.Euler(0f, 0f, tiltDegrees);
        baseScale = transform.localScale;
    }
    private void OnEnable()
    {
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null) return;

        gameManager.OnProgressChanged += HandleProgress;
        SetText(gameManager.Completed, gameManager.MinigamesToWin);
    }

    private void OnDisable()
    {
        if (gameManager != null) gameManager.OnProgressChanged -= HandleProgress;
    }

    private void HandleProgress(int completed, int total)
    {
        SetText(completed, total);
        if (popRoutine != null) StopCoroutine(popRoutine);
        popRoutine = StartCoroutine(Pop(completed >= total));
    }
    private void SetText(int completed, int total)
    {
        label.text = $"{completed}/{total}";
    }

    private IEnumerator Pop(bool isFinal)
    {
        Color endColor = isFinal ? finalColor : normalColor;
        float time = 0f;

        while (time < popDuration)
        {
            time += Time.unscaledDeltaTime;
            float pop = Mathf.Clamp01(time / popDuration);

            float scale = 1f + (popScale - 1f) * Mathf.Sin(pop * Mathf.PI) * (1f - pop * 0.5f);
            transform.localScale = baseScale * scale;

            float wobble = Mathf.Sin(pop * Mathf.PI * 4f) * (1f - pop) * 8f;
            transform.localRotation = Quaternion.Euler(0f, 0f, tiltDegrees + wobble);

            label.color = Color.Lerp(flashColor, endColor, pop);
            yield return null;
        }

        transform.localScale = baseScale;
        transform.localRotation = Quaternion.Euler(0f, 0f, tiltDegrees);
        label.color = endColor;
        popRoutine = null;
    }
}

