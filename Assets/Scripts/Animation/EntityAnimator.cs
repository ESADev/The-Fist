using System;
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

    private Animator animator;
    private Health health;
    private Attacker attacker;
    private MovementController movementController;

    private int speedParamId;
    private int deathTriggerId;
    private int damageTriggerId;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[EntityAnimator] Animator component missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        Entity entity = GetComponentInParent<Entity>();
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
        float currentSpeed = movementController != null ? movementController.GetCurrentSpeed() : 0f;
        animator.SetFloat(speedParamId, currentSpeed);
    }

    /// <summary>
    /// Plays the attack animation defined by the provided <see cref="AttackDefinitionSO"/>.
    /// </summary>
    /// <param name="attackData">Attack definition containing animation trigger information.</param>
    private void HandleAttackAnimation(AttackDefinitionSO attackData)
    {
        if (attackData == null)
        {
            Debug.LogError("[EntityAnimator] AttackDefinitionSO is null.", this);
            return;
        }

        animator.SetTrigger(attackData.animationTriggerName);
    }

    /// <summary>
    /// Triggers the death animation and disables this component afterwards.
    /// </summary>
    /// <param name="deadObject">The object that died.</param>
    private void HandleDeathAnimation(GameObject deadObject)
    {
        if (deadObject != gameObject)
        {
            return;
        }

        animator.SetTrigger(deathTriggerId);
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
        if (!isDamage)
        {
            return;
        }

        animator.SetTrigger(damageTriggerId);
    }
}
