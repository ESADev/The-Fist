using System.Collections;
using UnityEngine;

/// <summary>
/// Periodically finds targets in a radius and applies an effect to each.
/// </summary>
public class AuraTrigger : MonoBehaviour
{
    [Tooltip("Effect to activate each tick")] 
    [SerializeField] private MonoBehaviour effectSource;

    [SerializeField] private float radius = 5f;
    [SerializeField] private float tickRate = 1f;
    [SerializeField] private LayerMask targetMask = ~0; // Defaults to everything

    private IEffect _effect;
    private Coroutine _tickRoutine;

    private void Awake()
    {
        _effect = effectSource as IEffect;
        if (_effect == null)
        {
            Debug.LogError("[AuraTrigger] Effect source must implement IEffect.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (_effect != null)
        {
            _tickRoutine = StartCoroutine(TickRoutine());
        }
    }

    private void OnDisable()
    {
        if (_tickRoutine != null)
        {
            StopCoroutine(_tickRoutine);
        }
    }

    private IEnumerator TickRoutine()
    {
        var wait = new WaitForSeconds(tickRate);
        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetMask);
            foreach (Collider hit in hits)
            {
                _effect.Apply(hit.gameObject);
            }
            yield return wait;
        }
    }
}
