using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomTransformManager : MonoBehaviour
{
    [Header("Random Rotation (degrees)")]
    public Vector3 maxRotation = new Vector3(2.5f, 360f, 2.5f);

    [Header("Random Scale Multiplier")]
    public float minScale = 0.75f;
    public float maxScale = 1.25f;

    private static bool initialized = false;
    private static HashSet<GameObject> alreadyProcessed = new HashSet<GameObject>();

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !initialized)
        {
            initialized = true;
            UnityEditor.EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        initialized = false;
#endif
    }

    private void ApplyRandomTransform()
    {
        if (alreadyProcessed.Contains(gameObject)) return;

        alreadyProcessed.Add(gameObject);

        Vector3 randomEuler = new Vector3(
            Random.Range(-maxRotation.x, maxRotation.x),
            Random.Range(-maxRotation.y, maxRotation.y),
            Random.Range(-maxRotation.z, maxRotation.z)
        );
        transform.rotation = Quaternion.Euler(randomEuler);

        float scaleFactor = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * scaleFactor;
    }

    private void OnHierarchyChanged()
    {
        if (!gameObject.scene.IsValid()) return; // avoid assets
        ApplyRandomTransform();
    }
}
