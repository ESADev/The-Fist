using UnityEngine;

/// <summary>
/// Handles sound effects for an entity by listening to entity events and playing
/// appropriate audio through the SFXManager. Follows the same pattern as EntityAnimator.
/// </summary>
public class EntitySFXPlayer : MonoBehaviour
{
    private Entity entity;
    private Attacker attacker;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        if (entity != null)
        {
            attacker = entity.Attacker;
        }
        else
        {
            Debug.LogWarning($"[EntitySFXPlayer] No Entity component found in parents of {gameObject.name}. Attempting local components.", this);
            attacker = GetComponentInParent<Attacker>();
        }
    }

    private void OnEnable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed += HandleAttackSFX;
        }
    }

    private void OnDisable()
    {
        if (attacker != null)
        {
            attacker.OnAttackPerformed -= HandleAttackSFX;
        }
    }

    /// <summary>
    /// Handles playing sound effects when an attack is performed.
    /// </summary>
    /// <param name="attackData">Attack definition containing SFX information.</param>
    private void HandleAttackSFX(AttackDefinitionSO attackData)
    {
        if (attackData == null)
        {
            Debug.LogError("[EntitySFXPlayer] AttackDefinitionSO is null.", this);
            return;
        }

        if (SFXManager.Instance == null)
        {
            Debug.LogWarning("[EntitySFXPlayer] SFXManager instance not found.", this);
            return;
        }

        if (!string.IsNullOrEmpty(attackData.sfxKey))
        {
            SFXManager.Instance.PlaySound(attackData.sfxKey, transform.position);
        }
        else
        {
            Debug.LogWarning($"[EntitySFXPlayer] Attack '{attackData.attackName}' has no sfxKey defined.", this);
        }
    }
}