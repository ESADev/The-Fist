using UnityEngine;

/// <summary>
/// Triggers an assigned effect when this object collides with another collider.
/// Decoupled via the IEffect interface so any effect can be plugged in.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollisionTrigger : MonoBehaviour
{
    [Tooltip("Effect to activate on collision")]
    [SerializeField] private MonoBehaviour effectSource;

    private IEffect _effect;

    private void Awake()
    {
        _effect = effectSource as IEffect;
        if (_effect == null)
        {
            Debug.LogError("[CollisionTrigger] Effect source must implement IEffect.", this);
            enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_effect != null)
        {
            _effect.Apply(collision.gameObject);
        }
    }
}
