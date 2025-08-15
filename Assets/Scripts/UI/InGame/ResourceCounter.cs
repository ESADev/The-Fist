using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [Space]
    [SerializeField] private TextMeshProUGUI resourceText;
    [Space]
    [Header("Shake Settings")]
    [Tooltip("Enable or disable the shake effect on resource count changes")]
    [SerializeField] private bool enableShake = true;

    [Space]
    [Header("Text Effect Popup")] 
    [Tooltip("Skip spawning delta text for the initial set value (to avoid giant starting numbers).")]
    [SerializeField] private bool skipFirstSpawn = true;
    [Tooltip("If true, uses direct anchored position spawning instead of passing the RectTransform anchor.")]
    [SerializeField] private bool useDirectAnchoredSpawn = false;
    [Tooltip("Offset applied to popup spawn position (anchored units).")]
    [SerializeField] private Vector2 popupOffset = Vector2.zero;
    [Tooltip("Override color for positive deltas (only when using direct anchored spawn). Leave alpha at 1.")]
    [SerializeField] private Color positiveDeltaColor = Color.green;
    [Tooltip("Override color for negative deltas (only when using direct anchored spawn). Leave alpha at 1.")]
    [SerializeField] private Color negativeDeltaColor = Color.red;

    private RectTransform shakeTarget;
    private float shakeMagnitudeMultiplier = 0.75f;
    private float baseshakeDuration = 0.25f;
    private int shakeVibrato = 5;
    private float shakeRandomness = 30f;

    private int resourceCount = 0;
    private int displayedResourceCount = 0;
    private int previousResourceCount = 0;
    private bool hasInitialized = false;
    Tween animationTween;
    Tween shakeTween;

    Vector2 baseLocalPosition = new();

    void Awake()
    {
        GameEvents.OnPlayerResourceChanged += UpdateResourceCount;

        // If no shake target is assigned, use the text component's RectTransform
        if (shakeTarget == null && resourceText != null)
        {
            shakeTarget = resourceText.rectTransform;
        }

        baseLocalPosition = shakeTarget.anchoredPosition;
    }

    void UpdateResourceCount(ResourceType resourceType, int newResourceCount)
    {
        if (resourceType == this.resourceType)
        {
            previousResourceCount = resourceCount;
            resourceCount = newResourceCount;
            int delta = resourceCount - previousResourceCount;

            if (delta != 0 && resourceText != null && UITextEffectSpawner.Instance != null)
            {
                if (!skipFirstSpawn || hasInitialized)
                {
                    if (useDirectAnchoredSpawn)
                    {
                        var parent = resourceText.rectTransform.parent as RectTransform;
                        if (parent != null)
                        {
                            Vector2 anchoredPos = resourceText.rectTransform.anchoredPosition + popupOffset;
                            string msg = (delta > 0 ? "+" : "") + FormattingHelper.FormatNumber(delta);
                            Color col = delta > 0 ? positiveDeltaColor : negativeDeltaColor;
                            UITextEffectSpawner.SpawnTextAtAnchoredPositionGlobal(msg, anchoredPos, parent, col);
                        }
                        else
                        {
                            // Fallback to anchor-based if no parent rect
                            UITextEffectSpawner.SpawnDeltaGlobal(delta, resourceText.rectTransform); // offset not applied; parent missing
                        }
                    }
                    else
                    {
                        // Use anchor-based spawn; adjust by temporarily adding offset if non-zero.
                        if (popupOffset != Vector2.zero)
                        {
                            var original = resourceText.rectTransform.anchoredPosition;
                            resourceText.rectTransform.anchoredPosition = original + popupOffset;
                            UITextEffectSpawner.SpawnDeltaGlobal(delta, resourceText.rectTransform);
                            resourceText.rectTransform.anchoredPosition = original; // restore
                        }
                        else
                        {
                            UITextEffectSpawner.SpawnDeltaGlobal(delta, resourceText.rectTransform);
                        }
                    }
                }
            }
            hasInitialized = true;
            UpdateDisplayedCount();
        }
    }

    private void UpdateDisplayedCount()
    {
        // Calculate the change amount for shake intensity
        int changeAmount = Mathf.Abs(resourceCount - displayedResourceCount);
        
        // Trigger shake effect if there's a change and shake target is assigned
        if (changeAmount > 0 && shakeTarget != null && enableShake)
        {
            TriggerShake(changeAmount);
        }

        // Animatedly update the displayed resource count
        animationTween?.Kill();
        animationTween = DOTween.To(() => displayedResourceCount, x => displayedResourceCount = x, resourceCount, 0.5f)
            .OnUpdate(() =>
                resourceText.text = FormattingHelper.FormatNumber(displayedResourceCount))
            .SetEase(Ease.OutSine);
    }

    private void TriggerShake(int changeAmount)
    {
        // Kill any existing shake animation
        shakeTween?.Kill();

        if (shakeTarget.anchoredPosition != baseLocalPosition)
            shakeTarget.anchoredPosition = baseLocalPosition;

        // Calculate shake strength based on change amount and multiplier
        // Using logarithmic scaling to prevent excessive shake on large numbers
        float shakeStrength = Mathf.Log10(changeAmount + 1) * shakeMagnitudeMultiplier * 10f;
        
        // Trigger the shake effect and ensure it returns to the original position
        shakeTween = shakeTarget.DOShakeAnchorPos(
            baseshakeDuration,
            shakeStrength,
            shakeVibrato,
            shakeRandomness,
            false, // snapping
            true   // fadeOut
        ).OnKill(() => shakeTarget.anchoredPosition = baseLocalPosition)
         .OnComplete(() => shakeTarget.anchoredPosition = baseLocalPosition);
    }
    
    void OnDestroy()
    {
        // Clean up event subscription and kill any active tweens
        GameEvents.OnPlayerResourceChanged -= UpdateResourceCount;
        animationTween?.Kill();
        shakeTween?.Kill();
    }

}