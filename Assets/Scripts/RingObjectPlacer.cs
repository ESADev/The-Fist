using UnityEngine;
using UnityEditor;

// Editor-only component that lets level designers place a prefab in a ring around this GameObject.
// Because it lives in an Editor folder, it will not be included in builds.
[ExecuteAlways]
public class RingObjectPlacer : MonoBehaviour
{
    [Header("Prefab Source")] public GameObject prefab;

    [Header("Ring Layout")] [Min(1)] public int count = 8; // number of objects
    [Min(0f)] public float radius = 5f;                      // ring radius
    [Tooltip("Degrees of arc to span. 360 = full ring.")] [Range(0f, 360f)] public float arcDegrees = 360f;
    [Tooltip("Rotation offset in degrees applied before placing first object.")] public float startAngle = 0f;

    [Header("Orientation")] public bool lookAtCenter = false;
    public bool randomizeYaw = false;

    [Header("Offsets / Fine Tuning")] public Vector3 perInstanceLocalOffset = Vector3.zero;

    [Header("Generation Options")] public bool clearPreviousOnGenerate = true;
    [Tooltip("Automatically regenerate when a serialized value changes.")] public bool autoRegenerate = false;
    [Tooltip("Optional name for (auto) container child. Leave empty to place directly under this transform.")] public string containerName = "_Ring";

    // Track last values to detect changes for autoRegenerate without spamming when unrelated inspector events fire.
    private int _lastCount;
    private float _lastRadius;
    private float _lastArc;
    private float _lastStart;
    private GameObject _lastPrefab;
    private bool _lastLookAtCenter;
    private bool _lastRandomYaw;
    private Vector3 _lastOffset;

    private void OnValidate()
    {
        if (!autoRegenerate) return;

        if (HasLayoutChanged())
        {
            Generate();
            SnapshotValues();
        }
    }

    private bool HasLayoutChanged()
    {
        return _lastCount != count ||
               !Mathf.Approximately(_lastRadius, radius) ||
               !Mathf.Approximately(_lastArc, arcDegrees) ||
               !Mathf.Approximately(_lastStart, startAngle) ||
               _lastPrefab != prefab ||
               _lastLookAtCenter != lookAtCenter ||
               _lastRandomYaw != randomizeYaw ||
               _lastOffset != perInstanceLocalOffset;
    }

    private void SnapshotValues()
    {
        _lastCount = count;
        _lastRadius = radius;
        _lastArc = arcDegrees;
        _lastStart = startAngle;
        _lastPrefab = prefab;
        _lastLookAtCenter = lookAtCenter;
        _lastRandomYaw = randomizeYaw;
        _lastOffset = perInstanceLocalOffset;
    }

    [ContextMenu("Generate Ring")] public void Generate()
    {
        if (prefab == null)
        {
            Debug.LogWarning("RingObjectPlacer: Prefab is null.");
            return;
        }
        if (count < 1)
        {
            Debug.LogWarning("RingObjectPlacer: Count must be >= 1.");
            return;
        }
        if (radius < 0f) radius = 0f;

        // Find / create container (optional)
        Transform parent = transform;
        if (!string.IsNullOrEmpty(containerName))
        {
            var found = transform.Find(containerName);
            if (found == null)
            {
                GameObject containerGO = new GameObject(containerName);
                Undo.RegisterCreatedObjectUndo(containerGO, "Create Ring Container");
                containerGO.transform.SetParent(transform, false);
                parent = containerGO.transform;
            }
            else
            {
                parent = found;
            }
        }

        if (clearPreviousOnGenerate)
        {
            ClearChildren(parent);
        }

        bool fullCircle = arcDegrees >= 360f - 0.0001f; // tolerance
        float step;
        if (fullCircle)
        {
            step = 360f / count;
        }
        else
        {
            step = (count > 1) ? arcDegrees / (count - 1) : 0f; // include endpoints
        }

        for (int i = 0; i < count; i++)
        {
            float angleDeg = startAngle + step * i;
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 localPos = new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
            localPos += perInstanceLocalOffset; // uniform offset

            // Instantiate - preserve prefab connection where possible
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            Undo.RegisterCreatedObjectUndo(instance, "Ring Instance");
            instance.transform.localPosition = localPos;

            // Rotation
            Quaternion rot = prefab.transform.rotation; // default use prefab's rotation
            if (lookAtCenter)
            {
                Vector3 dirToCenter = (transform.position - instance.transform.position).normalized;
                if (dirToCenter.sqrMagnitude > 0.0001f)
                    rot = Quaternion.LookRotation(dirToCenter, Vector3.up);
            }
            if (randomizeYaw)
            {
                rot = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * rot;
            }
            instance.transform.rotation = rot;
        }
    }

    private void ClearChildren(Transform parent)
    {
        // Destroy existing children (only direct children to avoid accidental unrelated deletions)
        // Iterate backwards to be safe
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RingObjectPlacer))]
public class RingObjectPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var placer = (RingObjectPlacer)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Ring"))
        {
            placer.Generate();
        }
        if (GUILayout.Button("Clear Generated"))
        {
            if (!string.IsNullOrEmpty(placer.containerName))
            {
                var c = placer.transform.Find(placer.containerName);
                if (c != null)
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
            }
        }
        EditorGUILayout.HelpBox("Editor-only tool. Uses PrefabUtility to maintain prefab links. Works in edit mode only.", MessageType.Info);
    }
}
#endif
