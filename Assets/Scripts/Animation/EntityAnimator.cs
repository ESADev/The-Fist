using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

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
    private IUpgradable upgradeable;
    private IUnlockable unlockable;

    private int speedParamId;
    private int deathTriggerId;
    private int damageTriggerId;
    private int upgradeTriggerId;
    private int unlockTriggerId;
    private Vector3 lastPosition;
    private bool isDead = false;

    // Attack crossfade (Playables) fields
    [Header("Attack Crossfade Settings")]
    [Tooltip("Fade-in duration (seconds) when blending in an attack clip.")]
    private float attackFadeInDuration = 0.12f;
    [Tooltip("Fade-out duration (seconds) when returning to the base animator.")]
    private float attackFadeOutDuration = 0.10f;
    [Tooltip("If true, the attack clip is added on top (does not lower base layer weight).")]
    private bool attackAdditive = false;
    [Tooltip("If > 0, will clamp fade durations so their sum does not exceed (clipLength - this buffer). Helps guarantee some fully-held portion of the attack.")]
    private float attackHoldBuffer = 0.02f;

    private bool isPlayingAttackAnimation = false;
    private PlayableGraph attackGraph;
    private AnimationMixerPlayable mixerPlayable;          // 2 inputs: 0 = controller, 1 = attack clip
    private AnimatorControllerPlayable controllerPlayable; // Wraps the original runtime controller
    private AnimationClipPlayable attackClipPlayable;      // Created per attack
    private bool graphInitialized = false;

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

        // Get upgrade component if it exists (modular approach)
        upgradeable = GetComponentInParent<IUpgradable>() ?? GetComponent<IUpgradable>();

        // Get unlockable component if it exists
        unlockable = GetComponentInParent<IUnlockable>() ?? GetComponent<IUnlockable>();

        speedParamId = Animator.StringToHash(speedParameter);
        deathTriggerId = Animator.StringToHash(deathTriggerParameter);
        damageTriggerId = Animator.StringToHash(damageTriggerParameter);
        upgradeTriggerId = Animator.StringToHash(upgradeTriggerParameter);
        unlockTriggerId = Animator.StringToHash(unlockTriggerParameter);

        lastPosition = transform.position;

    InitializePlayableGraph();
    }

    private void OnEnable()
    {
        if (attacker != null)
        {
            // Subscribe to attack start (windup) for animation playback. Impact (damage) happens later.
            attacker.OnAttackStarted += HandleAttackAnimation;
        }

        if (health != null)
        {
            health.OnDied += HandleDeathAnimation;
            health.OnHealthChanged += HandleHealthChangeAnimation;
        }

        // Subscribe to upgrade events if the component exists (modular approach)
        if (upgradeable != null)
        {
            upgradeable.OnUpgraded += HandleUpgradeAnimation;
        }

        // Subscribe to unlock events if the component exists
        if (unlockable != null)
        {
            unlockable.OnUnlocked += HandleUnlockAnimation;
        }
    }

    private void OnDisable()
    {
        if (attacker != null)
        {
            attacker.OnAttackStarted -= HandleAttackAnimation;
        }

        if (health != null)
        {
            health.OnDied -= HandleDeathAnimation;
            health.OnHealthChanged -= HandleHealthChangeAnimation;
        }

        // Unsubscribe from upgrade events if the component exists (modular approach)
        if (upgradeable != null)
        {
            upgradeable.OnUpgraded -= HandleUpgradeAnimation;
        }

        // Unsubscribe from unlock events if the component exists
        if (unlockable != null)
        {
            unlockable.OnUnlocked -= HandleUnlockAnimation;
        }
    }

    private void OnDestroy()
    {
        if (graphInitialized && attackGraph.IsValid())
        {
            attackGraph.Destroy();
            graphInitialized = false;
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

        normalized /= movementController.entity.characterDefinition.movementStats.moveSpeed;

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
    /// Loads an attack clip from Resources and blends it in/out over the base controller using Playables.
    /// </summary>
    /// <param name="clipName">Clip name inside Resources/Animations/Attack</param>
    private void PlayAttackAnimationClip(string clipName)
    {
        if (isPlayingAttackAnimation)
        {
            // Optionally you could queue attacks here.
            Debug.LogWarning($"[EntityAnimator] Attack already playing on {gameObject.name}. Ignoring new request '{clipName}'.", this);
            return;
        }

        if (!graphInitialized)
        {
            InitializePlayableGraph();
            if (!graphInitialized)
            {
                Debug.LogError("[EntityAnimator] Failed to initialize PlayableGraph.", this);
                return;
            }
        }

        AnimationClip clip = Resources.Load<AnimationClip>("Animations/Attack/" + clipName);
        if (clip == null)
        {
            Debug.LogError($"[EntityAnimator] Could not load attack clip '{clipName}' from Resources/Animations/Attack.", this);
            return;
        }

        StartCoroutine(PlayAttackClipCoroutine(clip));
    }

    /// <summary>
    /// Coroutine that performs timed crossfade of an arbitrary attack clip using a 2-input mixer.
    /// </summary>
    private IEnumerator PlayAttackClipCoroutine(AnimationClip clip)
    {
        isPlayingAttackAnimation = true;

        // Clean previous attack playable if still valid
        if (attackClipPlayable.IsValid())
        {
            attackClipPlayable.Destroy();
        }

        attackClipPlayable = AnimationClipPlayable.Create(attackGraph, clip);
        attackClipPlayable.SetApplyFootIK(true);

        // (Re)connect clip playable to mixer input 1
        if (mixerPlayable.GetInputCount() < 2)
        {
            // Shouldn't happen; mixer was created with 2 inputs.
            while (mixerPlayable.GetInputCount() < 2)
            {
                mixerPlayable.AddInput(Playable.Null, 0);
            }
        }
        attackGraph.Connect(attackClipPlayable, 0, mixerPlayable, 1);
        mixerPlayable.SetInputWeight(1, 0f);

        float clipLength = clip.length;
        float fadeIn = Mathf.Max(0.0001f, attackFadeInDuration);
        float fadeOut = Mathf.Max(0.0001f, attackFadeOutDuration);

        // Ensure fades do not exceed clip length
        float maxFades = fadeIn + fadeOut + attackHoldBuffer;
        if (maxFades > clipLength)
        {
            float scale = clipLength / (fadeIn + fadeOut + 0.00001f);
            fadeIn *= scale;
            fadeOut *= scale;
        }
        float holdTime = Mathf.Max(0f, clipLength - fadeIn - fadeOut);

        // Fade in
        float t = 0f;
        while (t < fadeIn)
        {
            float w = t / fadeIn;
            if (!attackAdditive)
            {
                mixerPlayable.SetInputWeight(0, 1f - w);
            }
            mixerPlayable.SetInputWeight(1, w);
            t += Time.deltaTime;
            yield return null;
        }
        mixerPlayable.SetInputWeight(1, 1f);
        if (!attackAdditive) mixerPlayable.SetInputWeight(0, 0f);

        // Hold
        float elapsed = 0f;
        while (elapsed < holdTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < fadeOut)
        {
            float w = 1f - (t / fadeOut);
            mixerPlayable.SetInputWeight(1, w);
            if (!attackAdditive)
            {
                mixerPlayable.SetInputWeight(0, 1f - w);
            }
            t += Time.deltaTime;
            yield return null;
        }
        mixerPlayable.SetInputWeight(1, 0f);
        if (!attackAdditive) mixerPlayable.SetInputWeight(0, 1f);

        // Cleanup attack playable to free resources (optional; can reuse)
        if (attackClipPlayable.IsValid())
        {
            attackClipPlayable.Destroy();
        }

        isPlayingAttackAnimation = false;
        Debug.Log($"[EntityAnimator] Attack animation '{clip.name}' completed on {gameObject.name}.");
    }

    /// <summary>
    /// Handles upgrade animation when the entity is upgraded.
    /// </summary>
    /// <param name="upgradedObject">The GameObject that was upgraded.</param>
    /// <param name="newLevel">The new upgrade level.</param>
    private void HandleUpgradeAnimation(GameObject upgradedObject, int newLevel)
    {
        if (upgradedObject != gameObject && !upgradedObject.transform.IsChildOf(transform) && !transform.IsChildOf(upgradedObject.transform))
        {
            return; // Not our object
        }

        if (animator == null || isDead)
        {
            Debug.LogWarning($"[EntityAnimator] Cannot play upgrade animation: animator is null or entity is dead on {gameObject.name}.", this);
            return;
        }

        animator.SetTrigger(upgradeTriggerId);
        Debug.Log($"[EntityAnimator] {gameObject.name} upgrade animation triggered. New level: {newLevel}");
    }

    /// <summary>
    /// Handles unlock animation when the entity is unlocked.
    /// </summary>
    /// <param name="unlockedObject">The GameObject that was unlocked.</param>
    private void HandleUnlockAnimation(GameObject unlockedObject)
    {
        if (unlockedObject != gameObject && !unlockedObject.transform.IsChildOf(transform) && !transform.IsChildOf(unlockedObject.transform))
        {
            return; // Not our object
        }

        if (animator == null || isDead)
        {
            Debug.LogWarning($"[EntityAnimator] Cannot play unlock animation: animator is null or entity is dead on {gameObject.name}.", this);
            return;
        }

        animator.SetTrigger(unlockTriggerId);
        Debug.Log($"[EntityAnimator] {gameObject.name} unlock animation triggered.");
    }

    /// <summary>
    /// Builds the PlayableGraph used to blend attack clips without requiring extra animator states.
    /// </summary>
    private void InitializePlayableGraph()
    {
        if (graphInitialized || animator == null || animator.runtimeAnimatorController == null)
            return;

        attackGraph = PlayableGraph.Create($"EntityAnimatorGraph_{gameObject.name}");
        attackGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        controllerPlayable = AnimatorControllerPlayable.Create(attackGraph, animator.runtimeAnimatorController);
    // Create mixer with 2 inputs (0 = base controller, 1 = attack clip)
    mixerPlayable = AnimationMixerPlayable.Create(attackGraph, 2);

        attackGraph.Connect(controllerPlayable, 0, mixerPlayable, 0);
        mixerPlayable.SetInputWeight(0, 1f); // Base layer initially full weight
        mixerPlayable.SetInputWeight(1, 0f); // Attack layer hidden

        var output = AnimationPlayableOutput.Create(attackGraph, "Animation", animator);
        output.SetSourcePlayable(mixerPlayable);

        attackGraph.Play();
        graphInitialized = true;
    }
}
