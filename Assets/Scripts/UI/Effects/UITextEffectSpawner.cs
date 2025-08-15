using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Generic spawner for transient UI text effects (e.g. +100, -25, notifications).
/// </summary>
public class UITextEffectSpawner : MonoBehaviour
{
    public static UITextEffectSpawner Instance { get; private set; }

    [Header("Lifecycle / Singleton")]
    [SerializeField] private bool dontDestroyOnLoad = false;
    [Header("Prefab & Parenting")] 
    [Tooltip("Prefab that holds a TMP_Text or TextMeshProUGUI component.")]
    [SerializeField] private GameObject textEffectPrefab;
    [Tooltip("Optional default parent. If null, this component's RectTransform will be used.")]
    [SerializeField] private RectTransform defaultParent;

    [Header("Animation Settings")] 
    [SerializeField] private float moveUpDistance = 40f;
    [SerializeField] private float horizontalSpawnSpread = 30f;
    [SerializeField] private float horizontalEndDriftSpread = 30f;
    [SerializeField] private float travelDuration = 0.9f;
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float holdDuration = 0f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;

    [Header("Start Position Random Offset")] 
    [Tooltip("Enable extra random offset (X/Y) applied to the initial spawn position in addition to horizontalSpawnSpread (x-only).")]
    [SerializeField] private bool enableStartRandomOffset = false;
    [Tooltip("Inclusive random range for X offset added to start position.")]
    [SerializeField] private Vector2 startRandomOffsetRangeX = new(-10f, 10f);
    [Tooltip("Inclusive random range for Y offset added to start position.")]
    [SerializeField] private Vector2 startRandomOffsetRangeY = new(-10f, 10f);

    [Header("Styling")] 
    [SerializeField] private Color increaseColor = Color.green;
    [SerializeField] private Color decreaseColor = Color.red;
    [SerializeField] private float punchScaleMultiplier = 1f;
    [SerializeField] private float punchScaleDuration = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If another instance exists, keep the first one and destroy this duplicate.
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SpawnDelta(int delta, RectTransform anchor)
    {
        if (delta == 0) return;
        string prefix = delta > 0 ? "+" : string.Empty;
        Color col = delta > 0 ? increaseColor : decreaseColor;
        SpawnText(prefix + FormattingHelper.FormatNumber(delta), anchor, col);
    }

    public void SpawnText(string message, RectTransform anchor, Color? overrideColor = null)
    {
        if (textEffectPrefab == null || anchor == null) return;

        RectTransform parent = defaultParent != null ? defaultParent : (transform as RectTransform);
        if (parent == null) parent = anchor.parent as RectTransform; // fallback
        if (parent == null) return;

        GameObject instance = Instantiate(textEffectPrefab, parent);
        TMP_Text tmp = instance.GetComponent<TMP_Text>();
        RectTransform rect = instance.GetComponent<RectTransform>();
        if (tmp == null || rect == null)
        {
            Destroy(instance);
            return;
        }
        Vector2 basePos = GetAnchoredPositionInParent(anchor, parent);
        // Existing horizontal spread (legacy behavior)
        basePos.x += Random.Range(-horizontalSpawnSpread, horizontalSpawnSpread);
        // Additional X/Y jitter if enabled
        if (enableStartRandomOffset)
        {
            basePos.x += Random.Range(startRandomOffsetRangeX.x, startRandomOffsetRangeX.y);
            basePos.y += Random.Range(startRandomOffsetRangeY.x, startRandomOffsetRangeY.y);
        }
    rect.anchoredPosition = basePos;

        Vector2 endPos = rect.anchoredPosition + new Vector2(Random.Range(-horizontalEndDriftSpread, horizontalEndDriftSpread), moveUpDistance);

        Color startColor = overrideColor ?? tmp.color;
        startColor.a = 0f;
        tmp.color = startColor;
        tmp.text = message;

        float fadeIn = Mathf.Max(0.01f, fadeInDuration);
        float fadeOut = Mathf.Max(0.01f, fadeOutDuration);
        float travel = Mathf.Max(0.05f, travelDuration);

        Sequence seq = DOTween.Sequence();
        seq.Append(tmp.DOFade(1f, fadeIn));
        seq.Join(rect.DOAnchorPos(endPos, travel).SetEase(moveEase));
        if (holdDuration > 0f) seq.AppendInterval(holdDuration);
        seq.Append(tmp.DOFade(0f, fadeOut));
        seq.OnComplete(() => { if (instance != null) Destroy(instance); });

        if (punchScaleMultiplier > 1f)
        {
            rect.localScale = Vector3.one;
            seq.Join(rect.DOPunchScale(Vector3.one * (punchScaleMultiplier - 1f), punchScaleDuration, 8, 0.8f));
        }
    }

    /// <summary>
    /// Spawn a text effect at a specific anchored position (relative to parentOverride or default parent).
    /// </summary>
    public void SpawnTextAtAnchoredPosition(string message, Vector2 anchoredPosition, RectTransform parentOverride = null, Color? overrideColor = null)
    {
        if (textEffectPrefab == null) return;
        RectTransform parent = parentOverride != null ? parentOverride : (defaultParent != null ? defaultParent : transform as RectTransform);
        if (parent == null) return;

        GameObject instance = Instantiate(textEffectPrefab, parent);
        TMP_Text tmp = instance.GetComponent<TMP_Text>();
        RectTransform rect = instance.GetComponent<RectTransform>();
        if (tmp == null || rect == null)
        {
            Destroy(instance);
            return;
        }

        Vector2 spawnPos = anchoredPosition + new Vector2(Random.Range(-horizontalSpawnSpread, horizontalSpawnSpread), 0f);
        if (enableStartRandomOffset)
        {
            spawnPos.x += Random.Range(startRandomOffsetRangeX.x, startRandomOffsetRangeX.y);
            spawnPos.y += Random.Range(startRandomOffsetRangeY.x, startRandomOffsetRangeY.y);
        }
        rect.anchoredPosition = spawnPos;
        Vector2 endPos = rect.anchoredPosition + new Vector2(Random.Range(-horizontalEndDriftSpread, horizontalEndDriftSpread), moveUpDistance);

        Color startColor = overrideColor ?? tmp.color; startColor.a = 0f; tmp.color = startColor; tmp.text = message;
        float fadeIn = Mathf.Max(0.01f, fadeInDuration); float fadeOut = Mathf.Max(0.01f, fadeOutDuration); float travel = Mathf.Max(0.05f, travelDuration);
        Sequence seq = DOTween.Sequence();
        seq.Append(tmp.DOFade(1f, fadeIn));
        seq.Join(rect.DOAnchorPos(endPos, travel).SetEase(moveEase));
        if (holdDuration > 0f) seq.AppendInterval(holdDuration);
        seq.Append(tmp.DOFade(0f, fadeOut));
        seq.OnComplete(() => { if (instance != null) Destroy(instance); });
        if (punchScaleMultiplier > 1f)
        {
            rect.localScale = Vector3.one;
            seq.Join(rect.DOPunchScale(Vector3.one * (punchScaleMultiplier - 1f), punchScaleDuration, 8, 0.8f));
        }
    }

    /// <summary>
    /// Convenience spawn using a world position converted into the parent canvas's anchored space.
    /// </summary>
    public void SpawnTextAtWorldPosition(string message, Vector3 worldPosition, Canvas targetCanvas = null, Color? overrideColor = null)
    {
        RectTransform parent = defaultParent != null ? defaultParent : (transform as RectTransform);
        if (parent == null) return;
        Canvas canvas = targetCanvas != null ? targetCanvas : parent.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, cam, out Vector2 localPoint))
        {
            SpawnTextAtAnchoredPosition(message, localPoint, parent, overrideColor);
        }
    }

    private Vector2 GetAnchoredPositionInParent(RectTransform anchor, RectTransform parent)
    {
        if (anchor == null || parent == null) return Vector2.zero;
        if (anchor.parent == parent) return anchor.anchoredPosition;
        Canvas parentCanvas = parent.GetComponentInParent<Canvas>();
        Camera cam = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? parentCanvas.worldCamera : null;
        Vector3 worldCenter = anchor.TransformPoint(anchor.rect.center);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, cam, out Vector2 localPoint);
        return localPoint;
    }

    // -------- Static Convenience API --------
    public static void SpawnDeltaGlobal(int delta, RectTransform anchor)
        => Instance?.SpawnDelta(delta, anchor);

    public static void SpawnTextGlobal(string message, RectTransform anchor, Color? overrideColor = null)
        => Instance?.SpawnText(message, anchor, overrideColor);

    public static void SpawnTextAtAnchoredPositionGlobal(string message, Vector2 anchoredPos, RectTransform parentOverride = null, Color? overrideColor = null)
        => Instance?.SpawnTextAtAnchoredPosition(message, anchoredPos, parentOverride, overrideColor);

    public static void SpawnTextAtWorldPositionGlobal(string message, Vector3 worldPos, Canvas canvas = null, Color? overrideColor = null)
        => Instance?.SpawnTextAtWorldPosition(message, worldPos, canvas, overrideColor);
}
