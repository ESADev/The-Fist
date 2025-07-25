using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles the visual animation of health bars using DOTween.
/// This script should be placed on the HealthBar parent object with Image children: BG, EffectBar, and RealBar.
/// Listens to Health component's OnHealthChanged event and animates the bars accordingly.
/// </summary>
public class HealthBarAnimationHandler : MonoBehaviour
{
    [Header("Child Image References")]
    [Tooltip("Background image - should be a child named 'BG'")]
    [SerializeField] private Image backgroundImage;
    
    [Tooltip("Effect bar image - should be a child named 'EffectBar' (filled type)")]
    [SerializeField] private Image effectBarImage;
    
    [Tooltip("Health bar image - should be a child named 'RealBar' (filled type)")]
    [SerializeField] private Image realBarImage;

    [Header("Animation Settings")]
    [Tooltip("Duration for the health bar to animate to new value")]
    [SerializeField] private float healthBarAnimationDuration = 0.3f;
    
    [Tooltip("Duration for the effect bar to follow after health bar animation")]
    [SerializeField] private float effectBarAnimationDuration = 0.8f;
    
    [Tooltip("Delay before effect bar starts animating after health bar")]
    [SerializeField] private float effectBarDelay = 0.2f;
    
    [Tooltip("Easing type for health bar animation")]
    [SerializeField] private Ease healthBarEase = Ease.OutQuad;
    
    [Tooltip("Easing type for effect bar animation")]
    [SerializeField] private Ease effectBarEase = Ease.OutCubic;

    [Header("Visibility Settings")]
    [Tooltip("Duration for show/hide animations")]
    [SerializeField] private float visibilityAnimationDuration = 0.5f;
    
    [Tooltip("Easing type for show/hide animations")]
    [SerializeField] private Ease visibilityEase = Ease.OutQuad;
    
    [Tooltip("Auto-hide when health is full. If false, visibility must be controlled manually.")]
    [SerializeField] private bool autoHideWhenFull = true;

    [Header("Components")]
    [Tooltip("CanvasGroup component for controlling visibility. If null, will try to find one on this GameObject.")]
    [SerializeField] private CanvasGroup canvasGroup;

    private Health healthComponent;

    // Private variables for animation management
    private Sequence currentAnimationSequence;
    private Tween currentVisibilityTween;
    private float currentHealthPercentage = 1f;
    private float currentEffectPercentage = 1f;
    private bool isInitialized = false;
    private bool isVisible = false;

    private void Awake()
    {
        healthComponent = GetComponentInParent<Entity>()?.Health;
        if (healthComponent == null)
        {
            Debug.LogError($"[HealthBarAnimationHandler] No Health component found on {gameObject.name} or its parents. " +
                           "Please assign one manually or ensure this script is on a GameObject with a Health component.", this);
            enabled = false;
            return;
        }

        // Auto-find CanvasGroup if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                Debug.LogWarning($"[HealthBarAnimationHandler] No CanvasGroup found on {gameObject.name}. Adding one automatically.", this);
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Auto-find child images if not assigned
        if (backgroundImage == null)
            backgroundImage = transform.Find("BG")?.GetComponent<Image>();
            
        if (effectBarImage == null)
            effectBarImage = transform.Find("EffectBar")?.GetComponent<Image>();

        if (realBarImage == null)
            realBarImage = transform.Find("RealBar")?.GetComponent<Image>();

        // Validate required components
        if (effectBarImage == null || realBarImage == null)
        {
            Debug.LogError($"[HealthBarAnimationHandler] Missing required Image components on {gameObject.name}. " +
                          "Ensure EffectBar and RealBar child objects exist with Image components.", this);
            enabled = false;
            return;
        }

        // Ensure images are set to filled type
        if (effectBarImage.type != Image.Type.Filled)
        {
            Debug.LogWarning($"[HealthBarAnimationHandler] EffectBar Image on {gameObject.name} is not set to Filled type. Setting it now.", this);
            effectBarImage.type = Image.Type.Filled;
        }

        if (realBarImage.type != Image.Type.Filled)
        {
            Debug.LogWarning($"[HealthBarAnimationHandler] RealBar Image on {gameObject.name} is not set to Filled type. Setting it now.", this);
            realBarImage.type = Image.Type.Filled;
        }
    }

    private void Start()
    {
        // Find health component if not assigned
        if (healthComponent == null)
        {
            healthComponent = GetComponent<Health>();
            
            if (healthComponent == null)
            {
                healthComponent = GetComponentInParent<Health>();
            }
        }

        if (healthComponent == null)
        {
            Debug.LogError($"[HealthBarAnimationHandler] No Health component found on {gameObject.name} or its parents. " +
                          "Please assign one manually or ensure this script is on a GameObject with a Health component.", this);
            enabled = false;
            return;
        }

        // Subscribe to health changed events
        healthComponent.OnHealthChanged += OnHealthChanged;
        
        // Initialize bars to full health
        InitializeBars();
        isInitialized = true;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= OnHealthChanged;
        }

        // Kill any running animations
        if (currentAnimationSequence != null && currentAnimationSequence.IsActive())
        {
            currentAnimationSequence.Kill();
        }
        
        if (currentVisibilityTween != null && currentVisibilityTween.IsActive())
        {
            currentVisibilityTween.Kill();
        }
    }

    /// <summary>
    /// Initializes the health bars to full health state.
    /// </summary>
    private void InitializeBars()
    {
        realBarImage.fillAmount = 1f;
        effectBarImage.fillAmount = 1f;
        currentHealthPercentage = 1f;
        currentEffectPercentage = 1f;

        // Check if we need to update visibility based on UI health level
        if (autoHideWhenFull)
        {
            UpdateVisibilityBasedOnHealth();
        }
    }

    /// <summary>
    /// Called when the health component's health changes.
    /// </summary>
    /// <param name="currentHealth">Current health value</param>
    /// <param name="maxHealth">Maximum health value</param>
    /// <param name="isDamage">True if this change was caused by damage, false if by healing</param>
    private void OnHealthChanged(float currentHealth, float maxHealth, bool isDamage)
    {
        if (!isInitialized) return;

        float targetHealthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        AnimateHealthBars(targetHealthPercentage, isDamage);
    }

    /// <summary>
    /// Animates the health bars based on the new health percentage.
    /// </summary>
    /// <param name="targetHealthPercentage">Target health percentage (0-1)</param>
    /// <param name="isDamage">Whether this change is due to damage or healing</param>
    private void AnimateHealthBars(float targetHealthPercentage, bool isDamage)
    {
        // Kill any existing animation to prevent conflicts
        if (currentAnimationSequence != null && currentAnimationSequence.IsActive())
        {
            currentAnimationSequence.Kill();
        }

        // Clamp the target percentage
        targetHealthPercentage = Mathf.Clamp01(targetHealthPercentage);

        // Create new animation sequence
        currentAnimationSequence = DOTween.Sequence();

        if (isDamage)
        {
            // On damage: Health bar drops immediately, then effect bar follows
            currentAnimationSequence.Append(
                realBarImage.DOFillAmount(targetHealthPercentage, healthBarAnimationDuration)
                    .SetEase(healthBarEase)
                    .OnUpdate(() => {
                        currentHealthPercentage = realBarImage.fillAmount;
                        if (autoHideWhenFull) UpdateVisibilityBasedOnHealth();
                    })
            );

            currentAnimationSequence.AppendInterval(effectBarDelay);

            currentAnimationSequence.Append(
                effectBarImage.DOFillAmount(targetHealthPercentage, effectBarAnimationDuration)
                    .SetEase(effectBarEase)
                    .OnUpdate(() => {
                        currentEffectPercentage = effectBarImage.fillAmount;
                        if (autoHideWhenFull) UpdateVisibilityBasedOnHealth();
                    })
            );
        }
        else
        {
            // On healing: Both bars animate together, but effect bar leads slightly
            currentAnimationSequence.Append(
                effectBarImage.DOFillAmount(targetHealthPercentage, effectBarAnimationDuration)
                    .SetEase(effectBarEase)
                    .OnUpdate(() => {
                        currentEffectPercentage = effectBarImage.fillAmount;
                        if (autoHideWhenFull) UpdateVisibilityBasedOnHealth();
                    })
            );

            currentAnimationSequence.Join(
                realBarImage.DOFillAmount(targetHealthPercentage, healthBarAnimationDuration)
                    .SetEase(healthBarEase)
                    .SetDelay(effectBarDelay * 0.5f) // Slight delay for health bar on healing
                    .OnUpdate(() => {
                        currentHealthPercentage = realBarImage.fillAmount;
                        if (autoHideWhenFull) UpdateVisibilityBasedOnHealth();
                    })
            );
        }

        // Start the animation sequence
        currentAnimationSequence.Play();
        
        // Check if we need to update visibility based on UI health level
        if (autoHideWhenFull)
        {
            UpdateVisibilityBasedOnHealth();
        }
    }

    /// <summary>
    /// Shows the health bar with a smooth fade-in animation.
    /// </summary>
    public void Show()
    {
        if (canvasGroup == null) return;
        
        // Kill any existing visibility animation
        if (currentVisibilityTween != null && currentVisibilityTween.IsActive())
        {
            currentVisibilityTween.Kill();
        }
        
        isVisible = true;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        currentVisibilityTween = canvasGroup.DOFade(1f, visibilityAnimationDuration)
            .SetEase(visibilityEase);
    }

    /// <summary>
    /// Hides the health bar with a smooth fade-out animation.
    /// </summary>
    public void Hide()
    {
        if (canvasGroup == null) return;
        
        // Kill any existing visibility animation
        if (currentVisibilityTween != null && currentVisibilityTween.IsActive())
        {
            currentVisibilityTween.Kill();
        }
        
        isVisible = false;
        
        currentVisibilityTween = canvasGroup.DOFade(0f, visibilityAnimationDuration)
            .SetEase(visibilityEase)
            .OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
    }

    /// <summary>
    /// Updates visibility based on the current UI health level.
    /// Shows when health is between 0 and 1, hides when health is full or zero (based on UI display, not actual health).
    /// </summary>
    private void UpdateVisibilityBasedOnHealth()
    {
        // Use the lower of the two percentages to determine if we should be visible
        // This ensures we don't hide before both bars reach full
        float displayedHealthLevel = Mathf.Min(currentHealthPercentage, currentEffectPercentage);
        bool shouldBeVisible = displayedHealthLevel > 0f && displayedHealthLevel < 1f;
        
        if (shouldBeVisible && !isVisible)
        {
            Show();
        }
        else if (!shouldBeVisible && isVisible)
        {
            Hide();
        }
    }

    /// <summary>
    /// Manually sets the health bar values without animation. Useful for initialization.
    /// </summary>
    /// <param name="healthPercentage">Health percentage (0-1)</param>
    public void SetHealthBarImmediate(float healthPercentage)
    {
        healthPercentage = Mathf.Clamp01(healthPercentage);
        
        // Kill any running animations
        if (currentAnimationSequence != null && currentAnimationSequence.IsActive())
        {
            currentAnimationSequence.Kill();
        }
        
        if (currentVisibilityTween != null && currentVisibilityTween.IsActive())
        {
            currentVisibilityTween.Kill();
        }

        realBarImage.fillAmount = healthPercentage;
        effectBarImage.fillAmount = healthPercentage;
        currentHealthPercentage = healthPercentage;
        currentEffectPercentage = healthPercentage;
        
        // Update visibility based on the new health level
        if (autoHideWhenFull)
        {
            UpdateVisibilityBasedOnHealth();
        }
    }

    /// <summary>
    /// Gets the current health percentage being displayed.
    /// </summary>
    public float GetCurrentHealthPercentage()
    {
        return currentHealthPercentage;
    }

    /// <summary>
    /// Gets the current effect bar percentage being displayed.
    /// </summary>
    public float GetCurrentEffectPercentage()
    {
        return currentEffectPercentage;
    }

    /// <summary>
    /// Gets whether the health bar is currently visible.
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }

    /// <summary>
    /// Manually sets the auto-hide behavior.
    /// </summary>
    /// <param name="autoHide">Whether to automatically hide when health is full</param>
    public void SetAutoHide(bool autoHide)
    {
        autoHideWhenFull = autoHide;
        
        if (autoHideWhenFull)
        {
            UpdateVisibilityBasedOnHealth();
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Validates the component setup in the editor.
    /// </summary>
    private void OnValidate()
    {
        // Ensure duration values are positive
        healthBarAnimationDuration = Mathf.Max(0.1f, healthBarAnimationDuration);
        effectBarAnimationDuration = Mathf.Max(0.1f, effectBarAnimationDuration);
        effectBarDelay = Mathf.Max(0f, effectBarDelay);
        visibilityAnimationDuration = Mathf.Max(0.1f, visibilityAnimationDuration);
    }
    #endif
}
