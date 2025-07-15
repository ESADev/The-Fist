using System;
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

    /// <summary>
    /// Fired whenever this attacker performs an attack.
    /// </summary>
    public event Action<AttackDefinitionSO> OnAttackPerformed;

    private void Awake()
    {
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
        Debug.Log($"[Attacker] {gameObject.name} engaging {target.name}");
    }

    /// <summary>
    /// Stops combat engagement.
    /// </summary>
    public void Disengage()
    {
        isEngaging = false;
        currentTarget = null;
        Debug.Log($"[Attacker] {gameObject.name} disengaged");
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
        if (attack.attackType == AttackType.Melee)
        {
            ExecuteMeleeAttack(attack, target);
        }
        else
        {
            ExecuteRangedAttack(attack, target);
        }

        attackCooldowns[attack] = attack.cooldown;

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
    private void ExecuteRangedAttack(AttackDefinitionSO attack, GameObject target)
    {
        Debug.Log($"[Attacker] {gameObject.name} performs ranged attack {attack.attackName} on {target.name}");

        if (attack.projectilePrefab != null)
        {
            GameObject projectileObj = Instantiate(attack.projectilePrefab, transform.position, Quaternion.identity);
            if (projectileObj.TryGetComponent<SimpleProjectile>(out var projectile))
            {
                projectile.Initialize(gameObject, target, attack);
            }
            else
            {
                Debug.LogWarning($"[Attacker] Projectile prefab {attack.projectilePrefab.name} lacks SimpleProjectile component.", projectileObj);
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
