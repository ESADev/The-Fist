using UnityEngine;

/// <summary>
/// Automatically destroys the GameObject after its particle system completes.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticle : MonoBehaviour
{
    private void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("[AutoDestroyParticle] ParticleSystem component missing.", this);
            return;
        }

        Destroy(gameObject, particleSystem.main.duration);
    }
}
