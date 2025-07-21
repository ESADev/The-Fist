using System;
using UnityEngine;

/// <summary>
/// Controls character animations by reacting to movement, attacks and death events.
/// Requires <see cref="Animator"/> component on the same GameObject and <see cref="Entity"/> component on the parent GameObject.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Animator float parameter used to represent movement speed.")]
    [SerializeField] private string speedParameter = "Speed";

    [Tooltip("Animator trigger parameter fired when the character dies.")]
    [SerializeField] private string deathTriggerParameter = "Die";

    [Tooltip("Animator trigger parameter fired when the character takes damage.")]
    [SerializeField] private string damageTriggerParameter = "TakeDamage";

    // Component references
    private Animator animator;
    private Entity entity;
    private Health health;
    private MovementController movementController;
    private Attacker attacker;

    // Animator parameter hash IDs
    private int speedParamID;
    private int deathTriggerID;
    private int damageTriggerID;

    private Vector3 lastPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        entity = GetComponentInParent<Entity>();
        if (entity == null)
        {
            Debug.LogError($"[CharacterAnimator] Failed to find Entity component in parent of {gameObject.name}.", this);
        }

        if (animator == null)
        {
            Debug.LogError($"[CharacterAnimator] Animator component missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        if (entity == null)
        {
            Debug.LogError($"[CharacterAnimator] Entity component missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        // Cache components from entity
        health = entity.Health;
        movementController = entity.MovementController;
        attacker = entity.Attacker;

        speedParamID = Animator.StringToHash(speedParameter);
        deathTriggerID = Animator.StringToHash(deathTriggerParameter);
        damageTriggerID = Animator.StringToHash(damageTriggerParameter);

        lastPosition = transform.position;
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDeathAnimation;
            health.OnHealthChanged += HandleHealthChangeAnimation;
        }
        else
        {
            Debug.LogWarning($"[CharacterAnimator] {gameObject.name} has no Health component to animate death.", this);
        }

        if (attacker != null)
        {
            attacker.OnAttackPerformed += HandleAttackAnimation;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeathAnimation;
            health.OnHealthChanged -= HandleHealthChangeAnimation;
        }

        if (attacker != null)
        {
            attacker.OnAttackPerformed -= HandleAttackAnimation;
        }
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    /// <summary>
    /// Updates the movement animation by setting the speed parameter based on the character's velocity.
    /// </summary>
    private void UpdateMovementAnimation()
    {
        if (animator == null) return;

        float velocity = (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        float normalized = velocity;
        if (movementController != null && movementController.CurrentSpeed > 0f)
        {
            normalized = velocity / movementController.CurrentSpeed;
        }

        animator.SetFloat(speedParamID, normalized);
        lastPosition = transform.position;
    }

    /// <summary>
    /// Triggers an attack animation using the trigger defined on the <see cref="AttackDefinitionSO"/>.
    /// </summary>
    /// <param name="attackData">Attack definition containing animation info.</param>
    private void HandleAttackAnimation(AttackDefinitionSO attackData)
    {
        if (animator == null || attackData == null)
        {
            Debug.LogError("[CharacterAnimator] Cannot handle attack animation due to missing components or data.", this);
            return;
        }

        //string triggerName = attackData.animationTriggerName;
        //int triggerId = Animator.StringToHash(triggerName);
        //animator.SetTrigger(triggerId);
    }

    /// <summary>
    /// Triggers the death animation when the attached <see cref="Health"/> component reports death.
    /// </summary>
    /// <param name="deadObject">The object that died.</param>
    private void HandleDeathAnimation(GameObject deadObject)
    {
        if (deadObject.GetComponentInParent<Entity>() != entity || animator == null)
        {
            return;
        }

        animator.SetTrigger(deathTriggerID);

        // Disable itself after death animation is triggered
        enabled = false;
        Debug.Log($"[CharacterAnimator] {gameObject.name} has died and triggered death animation.");
    }

    /// <summary>
    /// Handles health change animation by triggering the damage animation parameter.
    /// </summary>
    private void HandleHealthChangeAnimation(float currentHealth, float maxHealth, bool isDamaged)
    {
        if (animator == null || health.CurrentHealth <= 0f) return;

        if (!isDamaged)
            {
                return; // Only trigger damage animation on actual damage
            }
            else
            {
                animator.SetTrigger(damageTriggerID);
            }
        Debug.Log($"[CharacterAnimator] {gameObject.name} health changed, animation triggered. Current health: {currentHealth}/{maxHealth}");
    }
}
