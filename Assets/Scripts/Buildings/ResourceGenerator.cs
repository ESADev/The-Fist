using System.Collections;
using UnityEngine;

/// <summary>
/// Generates resources for the player at regular intervals.
/// </summary>
[RequireComponent(typeof(Building))]
public class ResourceGenerator : MonoBehaviour
{
    [Header("Generator Profile")]
    [Tooltip("Profile defining resource generation settings.")]
    public ResourceGeneratorProfileSO generatorProfile;

    private Coroutine generateRoutine;

    private void OnEnable()
    {
        if (generatorProfile == null)
        {
            Debug.LogError("[ResourceGenerator] Generator profile not assigned.", this);
            enabled = false;
            return;
        }

        generateRoutine = StartCoroutine(GenerateCoroutine());
    }

    private void OnDisable()
    {
        if (generateRoutine != null)
        {
            StopCoroutine(generateRoutine);
            generateRoutine = null;
        }
    }

    private IEnumerator GenerateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(generatorProfile.tickRateInSeconds);
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddResource(generatorProfile.resourceType, generatorProfile.amountPerTick);
                Debug.Log($"[ResourceGenerator] Added {generatorProfile.amountPerTick} {generatorProfile.resourceType}.", this);
            }
            else
            {
                Debug.LogError("[ResourceGenerator] ResourceManager instance not found.", this);
            }
        }
    }
}
