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

    private RectTransform shakeTarget;
    private float shakeMagnitudeMultiplier = 0.75f;
    private float baseshakeDuration = 0.25f;
    private int shakeVibrato = 5;
    private float shakeRandomness = 30f;

    private int resourceCount = 0;
    private int displayedResourceCount = 0;
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
            resourceCount = newResourceCount;
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