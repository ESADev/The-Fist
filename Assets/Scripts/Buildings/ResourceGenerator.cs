using System.Collections;
using UnityEngine;

/// <summary>
/// Generates resources for the player at regular intervals.
/// </summary>
public class ResourceGenerator : MonoBehaviour
{
    [Header("Generator Profile")]
    [Tooltip("Profile defining resource generation settings.")]
    public ResourceGeneratorProfileSO generatorProfile;

    private Coroutine[] generateRoutines;

    private void OnEnable()
    {
        if (generatorProfile == null)
        {
            Debug.LogError("[ResourceGenerator] Generator profile not assigned.", this);
            enabled = false;
            return;
        }

        generateRoutines = new Coroutine[generatorProfile.resources.Count];
        for (int i = 0; i < generatorProfile.resources.Count; i++)
        {
            ResourceGenerationField resourceProfile = generatorProfile.resources[i];
            if (resourceProfile == null)
                continue;

            if (generateRoutines[i] != null)
                StopCoroutine(generateRoutines[i]);
            generateRoutines[i] = StartCoroutine(GenerateCoroutine(resourceProfile));
        }
    }

    private void OnDisable()
    {
        if (generateRoutines != null)
        {
            foreach (var routine in generateRoutines)
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
            }
            generateRoutines = null;
        }
    }

    private IEnumerator GenerateCoroutine(ResourceGenerationField resourceProfile)
    {
        while (true)
        {
            yield return new WaitForSeconds(resourceProfile.tickRateInSeconds);
            if (ResourceManager.Instance != null)
            {
                int amountPerTick = Mathf.RoundToInt(resourceProfile.tickRateInSeconds * resourceProfile.amountPerSeconds);

                ResourceManager.Instance.AddResource(resourceProfile.resourceType, amountPerTick);

                // SFX
                SFXManager.Instance.PlaySound("gem generation", transform.position);

                Debug.Log($"[ResourceGenerator] Added {amountPerTick} {resourceProfile.resourceType}.", this);
            }
            else
            {
                Debug.LogError("[ResourceGenerator] ResourceManager instance not found.", this);
            }
        }
    }
}
