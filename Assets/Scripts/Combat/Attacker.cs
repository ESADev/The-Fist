using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles combat behaviour for an entity by executing attacks defined in an
/// <see cref="AttackerProfileSO"/>.
/// </summary>
// [RequireComponent(typeof(Collider))]
public class Attacker : MonoBehaviour
{
    [Header("Profile")]
    [Tooltip("Profile defining the attacks available to this attacker.")]
    [HideInInspector] public AttackerProfileSO attackerProfile;

    [Space]
    [SerializeField]
    [Tooltip("Collider used for attack interactions. If not set, will be auto-assigned from children.")]
    private new Collider collider;

    [Space]
    [SerializeField]
    [Tooltip("Spawn point for projectiles. If not set, will use the transform position.")]
    private Transform projectileSpawnPoint;

    /// <summary>
    /// Tracks remaining cooldown time for each attack.
    /// </summary>
    private readonly Dictionary<AttackDefinitionSO, float> attackCooldowns = new Dictionary<AttackDefinitionSO, float>();

    /// <summary>
    /// Current engaged target. Null when not engaging anything.
    /// </summary>
    private GameObject currentTarget;

    /// <summary>
    /// Indicates whether the attacker is currently engaging a target.
    /// </summary>
    private bool isEngaging;

    private Entity entity;

    /// <summary>
    /// Fired immediately when an attack action starts (windup). Consumers (e.g. animations) should use this to begin visuals.
    /// </summary>
    public event Action<AttackDefinitionSO> OnAttackStarted;

    /// <summary>
    /// Fired when the attack actually "impacts" (damage applied / projectile spawned). SFX/VFX should prefer this for timing.
    /// </summary>
    public event Action<AttackDefinitionSO> OnAttackPerformed;

    private void Awake()
    {
        entity = GetComponent<Entity>();

        if (entity == null)
        {
            Debug.LogError($"[Attacker] Missing Entity on {gameObject.name}", this);
            enabled = false;
        }

        if (collider == null)
        {
            collider = GetComponentInChildren<Collider>();
            if (collider != null)
            {
                Debug.Log($"[Attacker] No collider assigned on {gameObject.name}. Automatically assigned collider: {collider.gameObject.name}", this);
            }
            else
            {
                Debug.LogWarning($"[Attacker] No collider found on {gameObject.name} or its children.", this);
            }
        }

        // Don't check for attackerProfile here - it may be assigned by Entity during initialization
    }

    /// <summary>
    /// Initializes the attacker with the specified profile.
    /// </summary>
    public void Initialize(AttackerProfileSO profile)
    {
        attackerProfile = profile;

        if (attackerProfile == null)
        {
            Debug.LogError($"[Attacker] AttackerProfileSO is not assigned on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        InitializeAttacks();
    }

    private void InitializeAttacks()
    {
        foreach (AttackDefinitionSO attack in attackerProfile.attacks)
        {
            if (attack != null && !attackCooldowns.ContainsKey(attack))
            {
                attackCooldowns.Add(attack, 0f);
            }
        }
    }

    private void Update()
    {
        if (entity != null && entity.CurrentState != EntityState.Active)
        {
            return;
        }

        UpdateCooldowns();

        if (isEngaging)
        {
            HandleCombat();
        }
    }

    /// <summary>
    /// Reduces cooldown timers every frame.
    /// </summary>
    private void UpdateCooldowns()
    {
        List<AttackDefinitionSO> keys = new List<AttackDefinitionSO>(attackCooldowns.Keys);
        foreach (AttackDefinitionSO attack in keys)
        {
            if (attackCooldowns[attack] > 0f)
            {
                attackCooldowns[attack] -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Begins combat engagement with a target.
    /// </summary>
    /// <param name="target">Target to attack.</param>
    public void Engage(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("[Attacker] Engage called with null target.", this);
            return;
        }

        currentTarget = target;
        isEngaging = true;
        //Debug.Log($"[Attacker] {gameObject.name} engaging {target.name}");
    }

    /// <summary>
    /// Stops combat engagement.
    /// </summary>
    public void Disengage()
    {
        isEngaging = false;
        currentTarget = null;
        //Debug.Log($"[Attacker] {gameObject.name} disengaged");
    }

    /// <summary>
    /// Selects the most suitable attack for the specified target.
    /// </summary>
    /// <param name="target">Potential attack target.</param>
    /// <returns>Chosen attack definition or null if none available.</returns>
    private AttackDefinitionSO SelectBestAttackForTarget(GameObject target)
    {
        if (target == null)
        {
            return null;
        }

        // Calculate distance to the closest point on the target's collider
        Collider[] targetColliders = target.GetComponentsInChildren<Collider>();
        float distance;
        
        if (targetColliders.Length > 0)
        {
            float minDistance = float.MaxValue;
            foreach (Collider col in targetColliders)
            {
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            float currentDistance = Vector3.Distance(transform.position, closestPoint);
            minDistance = Mathf.Min(minDistance, currentDistance);
            }
            distance = minDistance;
        }
        else
        {
            // Fallback to center-to-center if no collider found
            distance = Vector3.Distance(transform.position, target.transform.position);
            Debug.LogWarning($"[Attacker] Target {target.name} has no Collider components. Using center-to-center distance.", this);
        }
        
        AttackDefinitionSO bestAttack = null;
        float bestScore = float.MinValue;

        foreach (AttackDefinitionSO attack in attackerProfile.attacks)
        {
            if (attack == null || !IsAttackReady(attack))
            {
                continue;
            }

            if (distance > attack.range)
            {
                continue;
            }

            float score = attack.damage / Mathf.Max(attack.cooldown, 0.01f);
            if (score > bestScore)
            {
                bestScore = score;
                bestAttack = attack;
            }
        }

        return bestAttack;
    }

    /// <summary>
    /// Determines the range of the best attack currently available for the specified target.
    /// </summary>
    /// <param name="target">Potential attack target.</param>
    /// <returns>Range of the chosen attack or 0 if none are in range.</returns>
    public float GetBestAttackRange(GameObject target)
    {
        AttackDefinitionSO attack = SelectBestAttackForTarget(target);
        return attack != null ? attack.range : 0f;
    }

    /// <summary>
    /// Handles combat logic executed each frame while engaging a target.
    /// </summary>
    private void HandleCombat()
    {
        if (currentTarget == null)
        {
            Disengage();
            return;
        }

        AttackDefinitionSO attack = SelectBestAttackForTarget(currentTarget);
        if (attack != null)
        {
            PerformAttack(attack, currentTarget);
        }
    }

    /// <summary>
    /// Determines if an attack is off cooldown and ready to use.
    /// </summary>
    /// <param name="attack">Attack definition.</param>
    /// <returns>True if the attack can be executed.</returns>
    private bool IsAttackReady(AttackDefinitionSO attack)
    {
        if (attack == null)
        {
            return false;
        }

        return !attackCooldowns.TryGetValue(attack, out float timeLeft) || timeLeft <= 0f;
    }

    /// <summary>
    /// Executes the specified attack against a target and starts its cooldown.
    /// </summary>
    /// <param name="attack">Attack data.</param>
    /// <param name="target">Target to attack.</param>
    private void PerformAttack(AttackDefinitionSO attack, GameObject target)
    {
        // Launch coroutine handling windup -> impact sequence
        StartCoroutine(PerformAttackRoutine(attack, target));
    }

    /// <summary>
    /// Coroutine that separates attack start (windup) from impact using AttackDefinitionSO.impactDelay.
    /// Keeps system decoupled from specific animation clips; consumers can subscribe to start vs impact.
    /// </summary>
    private IEnumerator PerformAttackRoutine(AttackDefinitionSO attack, GameObject initialTarget)
    {
        if (attack == null)
            yield break;

        // Begin cooldown immediately so attack won't be picked again during windup.
        attackCooldowns[attack] = attack.cooldown;

        // Fire start event so animator (and optional anticipation SFX) can begin.
        OnAttackStarted?.Invoke(attack);

        float delay = Mathf.Max(0f, attack.impactDelay);
        float elapsed = 0f;
        while (elapsed < delay)
        {
            // Early abort conditions: attacker disabled/destroyed or entity not active anymore
            if (!this || !enabled || entity == null || entity.CurrentState != EntityState.Active)
            {
                yield break; // Do not apply impact
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Re-validate target at impact time
        GameObject current = currentTarget != null ? currentTarget : initialTarget;
        if (current == null)
        {
            yield break;
        }

        // Optional: ensure still in range (use collider distance logic if needed; reuse SelectBestAttackForTarget distance logic simplified)
        float distance = Vector3.Distance(transform.position, current.transform.position);
        if (distance > attack.range * 1.1f) // small grace
        {
            // Out of range now; treat as a miss (could add event later)
            yield break;
        }

        // Execute the actual gameplay effect now (impact)
        if (attack.AttackType == AttackType.Melee)
        {
            ExecuteMeleeAttack(attack, current);
        }
        else
        {
            ExecuteRangedAttack(attack, current, FactionHelper.GetEnemyOf(entity.Faction.CurrentFaction));
        }

        // Fire impact event after damage/projectile spawn so listeners (SFX/VFX) sync to real effect
        OnAttackPerformed?.Invoke(attack);
    }

    /// <summary>
    /// Handles melee attack execution logic.
    /// </summary>
    /// <param name="attack">Attack being executed.</param>
    /// <param name="target">Target receiving the attack.</param>
    private void ExecuteMeleeAttack(AttackDefinitionSO attack, GameObject target)
    {
        Debug.Log($"[Attacker] {gameObject.name} performs melee attack {attack.attackName} on {target.name}");

        if (target.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(attack.damage, gameObject, attack);
        }
        else
        {
            Debug.LogWarning($"[Attacker] Target {target.name} has no Health component.", this);
        }
    }

    /// <summary>
    /// Handles ranged attack execution logic.
    /// </summary>
    /// <param name="attack">Attack being executed.</param>
    /// <param name="target">Target receiving the attack.</param>
    private void ExecuteRangedAttack(AttackDefinitionSO attack, GameObject target, FactionType targetFaction)
    {
        Debug.Log($"[Attacker] {gameObject.name} performs ranged attack {attack.attackName} on {target.name}");

        RangedAttackDefinitionSO ranged = attack as RangedAttackDefinitionSO;

        if (ranged != null && ranged.projectilePrefab != null)
        {
            Vector3 spawnPoint = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
            GameObject projectileObj = Instantiate(ranged.projectilePrefab, spawnPoint, Quaternion.identity);
            if (projectileObj.TryGetComponent<SimpleProjectile>(out var projectile))
            {
                projectile.Initialize(gameObject, target, ranged, targetFaction);
            }
            else
            {
                Debug.LogWarning($"[Attacker] Projectile prefab {ranged.projectilePrefab.name} lacks SimpleProjectile component.", projectileObj);
                if (target.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(attack.damage, gameObject, attack);
                }
            }
        }
        else if (target.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(attack.damage, gameObject, attack);
        }
        else
        {
            Debug.LogWarning($"[Attacker] Target {target.name} has no Health component.", this);
        }
    }
}
