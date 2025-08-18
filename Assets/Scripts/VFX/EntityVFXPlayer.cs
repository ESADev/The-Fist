using UnityEngine;

/// <summary>
/// Handles visual effects for an entity by listening to entity events (currently attack events)
/// and triggering VFX through the VFXManager. Mirrors the structure of EntitySFXPlayer for
/// consistency.
/// </summary>
public class EntityVFXPlayer : MonoBehaviour
{
    private Entity entity;
    private Attacker attacker;

    [Header("Damage VFX")]
    [Tooltip("VFX key to play when this entity takes damage (overrides global defaults). Leave empty to use global logic.")]
    public string damageVFXKey;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        if (entity != null)
        {
            attacker = entity.Attacker;
        }
        else
        {
            Debug.LogWarning($"[EntityVFXPlayer] No Entity component found in parents of {gameObject.name}. Attempting local components.", this);
            attacker = GetComponentInParent<Attacker>();
        }
    }

    private void OnEnable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed += HandleAttackVFX;
        }
        if (entity != null && entity.Health != null)
        {
            entity.Health.OnDamaged += HandleUnitDamagedVFX;
        }
    }

    private void OnDisable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed -= HandleAttackVFX;
        }
        if (entity != null && entity.Health != null)
        {
            entity.Health.OnDamaged -= HandleUnitDamagedVFX;
        }
    }

    /// <summary>
    /// Handles playing visual effects when an attack is performed.
    /// </summary>
    /// <param name="attackData">Attack definition containing VFX information.</param>
    private void HandleAttackVFX(AttackDefinitionSO attackData)
    {
        if (attackData == null)
        {
            Debug.LogError("[EntityVFXPlayer] AttackDefinitionSO is null.", this);
            return;
        }

        if (VFXManager.Instance == null)
        {
            Debug.LogWarning("[EntityVFXPlayer] VFXManager instance not found.", this);
            return;
        }

        if (!string.IsNullOrEmpty(attackData.vfxKey))
        {
            VFXManager.Instance.PlayEffect(attackData.vfxKey, transform.position);
        }
        else
        {
            Debug.LogWarning($"[EntityVFXPlayer] Attack '{attackData.attackName}' has no vfxKey defined.", this);
        }
    }

    /// <summary>
    /// Plays damage VFX for this entity when it is the victim of a damage event.
    /// </summary>
    private void HandleUnitDamagedVFX(DamageInfo info)
    {
        if (entity == null) return;
        if (VFXManager.Instance == null) return;
        if (string.IsNullOrEmpty(damageVFXKey)) return;
        VFXManager.Instance.PlayEffect(damageVFXKey, entity.transform.position);
    }
}
