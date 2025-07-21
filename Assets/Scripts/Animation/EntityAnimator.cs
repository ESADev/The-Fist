using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Generic animation controller that listens to entity events and plays the
/// appropriate animations. Handles only the capabilities present on the entity.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EntityAnimator : MonoBehaviour
{
    [Header("Animator Parameters")]
    [Tooltip("Float parameter used to represent the entity's movement speed.")]
    [SerializeField] private string speedParameter = "Speed";

    [Tooltip("Trigger parameter fired when the entity dies.")]
    [SerializeField] private string deathTriggerParameter = "Die";

    [Tooltip("Trigger parameter fired when the entity takes damage.")]
    [SerializeField] private string damageTriggerParameter = "TakeDamage";

    [Tooltip("Trigger parameter fired when the entity upgrades.")]
    [SerializeField] private string upgradeTriggerParameter = "Upgrade";

    [Tooltip("Trigger parameter fired when the entity unlocks something.")]
    [SerializeField] private string unlockTriggerParameter = "Unlock";

    private Entity entity;
    private Animator animator;
    private Health health;
    private Attacker attacker;
    private MovementController movementController;

    private int speedParamId;
    private int deathTriggerId;
    private int damageTriggerId;
    private Vector3 lastPosition;
    private bool isDead = false;

    // Fields for direct animation clip playback
    private AnimationClip currentAttackClip;
    private string originalStateName;
    private bool isPlayingAttackAnimation = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[EntityAnimator] Animator component missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        entity = GetComponentInParent<Entity>();
        if (entity != null)
        {
            health = entity.Health;
            attacker = entity.Attacker;
            movementController = entity.MovementController;
        }
        else
        {
            Debug.LogWarning($"[EntityAnimator] No Entity component found in parents of {gameObject.name}. Attempting local components.", this);
            health = GetComponent<Health>();
            attacker = GetComponent<Attacker>();
            movementController = GetComponent<MovementController>();
        }

        speedParamId = Animator.StringToHash(speedParameter);
        deathTriggerId = Animator.StringToHash(deathTriggerParameter);
        damageTriggerId = Animator.StringToHash(damageTriggerParameter);

        lastPosition = transform.position;

        // Store the original state name for returning after attack animations
        if (animator.layerCount > 0)
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            // For now, we'll assume the default state is "Idle" or similar
            // You can customize this based on your animator setup
            originalStateName = "Idle";
        }
    }

    private void OnEnable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed += HandleAttackAnimation;
        }

        if (health != null)
        {
            health.OnDied += HandleDeathAnimation;
            health.OnHealthChanged += HandleHealthChangeAnimation;
        }
    }

    private void OnDisable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed -= HandleAttackAnimation;
        }

        if (health != null)
        {
            health.OnDied -= HandleDeathAnimation;
            health.OnHealthChanged -= HandleHealthChangeAnimation;
        }
    }

    private void Update()
    {
        if (movementController != null)
        {
            UpdateMovementAnimation();
        }
    }

    /// <summary>
    /// Sets the animator speed parameter based on the current movement speed.
    /// </summary>
    private void UpdateMovementAnimation()
    {
        float currentSpeed = movementController != null ? movementController.CurrentSpeed : 0f;

        float velocity = (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        float normalized = velocity;
        if (movementController != null && movementController.CurrentSpeed > 0f)
        {
            normalized = velocity / currentSpeed;
        }

        animator.SetFloat(speedParameter, normalized);
        lastPosition = transform.position;
    }

    /// <summary>
    /// Plays the attack animation defined by the provided <see cref="AttackDefinitionSO"/>.
    /// If animationClipName is provided, loads and plays the clip directly from Resources.
    /// Otherwise, uses the traditional animator trigger approach.
    /// </summary>
    /// <param name="attackData">Attack definition containing animation information.</param>
    private void HandleAttackAnimation(AttackDefinitionSO attackData)
    {
        if (attackData == null || isDead)
        {
            Debug.LogError("[EntityAnimator] AttackDefinitionSO is null or entity is dead.", this);
            return;
        }

        // Check if we should use direct animation clip playback
        if (!string.IsNullOrEmpty(attackData.animationClipName))
        {
            PlayAttackAnimationClip(attackData.animationClipName);
        }
        else
        {
            // Fallback to traditional animator trigger approach
            animator.SetTrigger(attackData.animationTriggerName);
        }
    }

    /// <summary>
    /// Triggers the death animation and disables this component afterwards.
    /// </summary>
    /// <param name="deadObject">The object that died.</param>
    private void HandleDeathAnimation(GameObject deadObject)
    {
        if (deadObject.GetComponentInParent<Entity>() != entity || animator == null)
        {
            Debug.LogWarning("[EntityAnimator] Dead object is not the same entity or animator is null.", this);
            return;
        }

        if (isDead)
        {
            Debug.LogWarning("[EntityAnimator] Death animation already triggered, ignoring subsequent calls.", this);
            return;
        }

        isDead = true;
        animator.SetTrigger(deathTriggerId);

        // Disable itself after death animation is triggered
        Debug.Log($"[CharacterAnimator] {entity.name} has died and triggered death animation.");
        enabled = false;
    }

    /// <summary>
    /// Plays the damage animation when the health change was caused by damage.
    /// </summary>
    /// <param name="current">Current health.</param>
    /// <param name="max">Maximum health.</param>
    /// <param name="isDamage">Whether the change was due to taking damage.</param>
    private void HandleHealthChangeAnimation(float current, float max, bool isDamage)
    {
        if (animator == null || health.CurrentHealth <= 0f) return;

        if (!isDamage)
            {
                return; // Only trigger damage animation on actual damage
            }
            else
            {
                animator.SetTrigger(damageTriggerId);
            }
        Debug.Log($"[CharacterAnimator] {gameObject.name} health changed, animation triggered. Current health: {current}/{max}");
    }

    /// <summary>
    /// Loads an animation clip from Resources and plays it directly, then returns to the original state.
    /// </summary>
    /// <param name="clipName">Name of the animation clip in the Resources folder.</param>
    private void PlayAttackAnimationClip(string clipName)
    {
        if (isPlayingAttackAnimation)
        {
            Debug.LogWarning($"[EntityAnimator] Already playing attack animation on {gameObject.name}, ignoring new request.", this);
            return;
        }

        // Load the animation clip from Resources
        AnimationClip clip = Resources.Load<AnimationClip>(clipName);
        if (clip == null)
        {
            Debug.LogError($"[EntityAnimator] Could not load animation clip '{clipName}' from Resources folder.", this);
            return;
        }

        currentAttackClip = clip;
        StartCoroutine(PlayAttackClipCoroutine());
    }

    /// <summary>
    /// Coroutine that handles playing the attack animation and returning to the original state.
    /// </summary>
    private IEnumerator PlayAttackClipCoroutine()
    {
        isPlayingAttackAnimation = true;

        // Alternative approach: Use AnimationClip.SampleAnimation for direct playback
        if (currentAttackClip != null)
        {
            float animationLength = currentAttackClip.length;
            float elapsedTime = 0f;
            
            // Store the current animation state to restore later
            AnimatorStateInfo originalState = animator.GetCurrentAnimatorStateInfo(0);
            
            // Disable the animator temporarily to prevent conflicts
            animator.enabled = false;

            // Sample the animation clip frame by frame
            while (elapsedTime < animationLength)
            {
                float normalizedTime = elapsedTime / animationLength;
                currentAttackClip.SampleAnimation(gameObject, elapsedTime);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Sample the final frame to ensure completion
            currentAttackClip.SampleAnimation(gameObject, animationLength);

            // Re-enable the animator
            animator.enabled = true;
            
            // Return to the previous state
            if (!string.IsNullOrEmpty(originalStateName))
            {
                animator.Play(originalStateName, 0, originalState.normalizedTime);
            }
        }

        isPlayingAttackAnimation = false;
        
        Debug.Log($"[EntityAnimator] Attack animation '{currentAttackClip?.name}' completed on {gameObject.name}.");
        currentAttackClip = null;
    }
}
