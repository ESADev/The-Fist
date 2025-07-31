using UnityEngine;
using System.Collections;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 1f;
    [SerializeField] private float delayAfterDissolve = 0f;

    private IEnumerator Start()
    {
        // Find all renderers in the entity hierarchy
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning("DeathHandler: No Renderers found.");
            yield break;
        }

        // Load the dissolve template from Resources
        Material dissolveTemplate = Resources.Load<Material>("Art/VFX/DissolveTemplate");
        if (dissolveTemplate == null)
        {
            Debug.LogError("DissolveTemplate material not found in Resources!");
            yield break;
        }

        // Create dissolve materials for each renderer
        Material[] dissolveMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            
            // Grab original texture from renderer's material
            Texture mainTex = renderer.material.mainTexture;
            
            // Clone material and inject main texture
            Material runtimeMat = new Material(dissolveTemplate);
            runtimeMat.SetTexture("_MainTex", mainTex);
            
            // Assign it to the renderer
            renderer.material = runtimeMat;
            dissolveMaterials[i] = runtimeMat;
        }

        // Add dissolve logic and start it with multiple materials
        var dissolve = gameObject.AddComponent<DissolveOnDeath>();
        dissolve.Setup(dissolveMaterials, dissolveDuration);
        dissolve.Kill();

        yield return new WaitForSeconds(dissolveDuration + delayAfterDissolve);

        Destroy(gameObject);
    }
}
