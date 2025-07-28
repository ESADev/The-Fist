// Place this script in a folder named "Editor" in your Unity project's Assets folder.
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GaussianObjectPlacerWindow : EditorWindow
{
    private Transform startPoint;
    private Transform endPoint;

    private GameObject prefabToPlace;
    private float density = 0.5f;
    private float maxRadius = 10.0f;
    private bool parentToCenter = true;

    private AnimationCurve distributionCurve;

    private bool usePerlinFilter = true;
    private float perlinScale = 0.1f;
    private float perlinThreshold = 0.5f;

    private static List<GameObject> lastGeneratedObjects = new List<GameObject>();
    private static System.Random random = new System.Random();

    private void OnEnable()
    {
        if (distributionCurve == null || distributionCurve.keys.Length == 0)
        {
            distributionCurve = new AnimationCurve(
                new Keyframe(0f, 1f, 0f, 0f),
                new Keyframe(1f, 1f, 0f, 0f)
            );
        }
    }

    [MenuItem("Tools/Gaussian Object Placer")]
    public static void ShowWindow()
    {
        GetWindow<GaussianObjectPlacerWindow>("Object Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Distribution Settings", EditorStyles.boldLabel);
        prefabToPlace = (GameObject)EditorGUILayout.ObjectField("Prefab to Place", prefabToPlace, typeof(GameObject), false);

        startPoint = (Transform)EditorGUILayout.ObjectField("Start Point", startPoint, typeof(Transform), true);
        endPoint = (Transform)EditorGUILayout.ObjectField("End Point", endPoint, typeof(Transform), true);

        density = EditorGUILayout.FloatField("Density (per sq. unit)", density);
        maxRadius = EditorGUILayout.FloatField("Capsule Radius", maxRadius);

        distributionCurve = EditorGUILayout.CurveField("Distribution Curve", distributionCurve);
        parentToCenter = EditorGUILayout.Toggle("Parent to Start Point", parentToCenter);

        EditorGUILayout.Space();

        usePerlinFilter = EditorGUILayout.BeginToggleGroup("Use Perlin Noise Filter", usePerlinFilter);
        perlinScale = EditorGUILayout.FloatField("Noise Scale", perlinScale);
        perlinThreshold = EditorGUILayout.Slider("Noise Threshold", perlinThreshold, 0f, 1f);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("(Re)generate Objects"))
        {
            if (prefabToPlace == null || startPoint == null || endPoint == null)
            {
                Debug.LogError("Error: Assign a Prefab, a Start Point, and an End Point.");
                return;
            }
            UndoLastGeneration();
            GenerateObjects();
        }
        if (GUILayout.Button("Undo Last Generation"))
        {
            UndoLastGeneration();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void UndoLastGeneration()
    {
        if (lastGeneratedObjects.Count == 0) return;
        Undo.SetCurrentGroupName("Undo Last Placed Objects");
        foreach (var obj in lastGeneratedObjects)
        {
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }
        lastGeneratedObjects.Clear();
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    private void GenerateObjects()
    {
        Undo.SetCurrentGroupName("Generate In Capsule");
        lastGeneratedObjects.Clear();

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;
        
        startPos.y = 0; // Work on the XZ plane
        endPos.y = 0;

        float lineLength = Vector3.Distance(startPos, endPos);
        Vector3 lineDir = (lineLength > 0) ? (endPos - startPos).normalized : Vector3.right;

        float rectangleArea = lineLength * (2 * maxRadius);
        float circleArea = Mathf.PI * maxRadius * maxRadius;
        float totalArea = rectangleArea + circleArea;
        float rectProbability = (totalArea > 0) ? rectangleArea / totalArea : 0;

        int totalAttempts = Mathf.RoundToInt(density * totalArea);

        float noiseOffsetX = (float)random.NextDouble() * 10000f;
        float noiseOffsetY = (float)random.NextDouble() * 10000f;

        for (int i = 0; i < totalAttempts; i++)
        {
            Vector3 potentialPosition;
            float normalizedDistanceForCurve;

            if (random.NextDouble() < rectProbability)
            {
                // Spawn in the rectangle
                float distAlongLine = (float)random.NextDouble() * lineLength;
                float perpDist = ((float)random.NextDouble() * 2f - 1f) * maxRadius;
                
                Vector3 perpDir = new Vector3(-lineDir.z, 0, lineDir.x);

                potentialPosition = startPos + (lineDir * distAlongLine) + (perpDir * perpDist);
                normalizedDistanceForCurve = Mathf.Abs(perpDist) / maxRadius;
            }
            else
            {
                // --- CORRECTED LOGIC: Spawn in one of the two SEMICIRCLES ---
                Vector3 circleCenter;
                Vector3 capDirection;

                if (random.NextDouble() < 0.5)
                {
                    circleCenter = startPos;
                    capDirection = -lineDir;
                }
                else
                {
                    circleCenter = endPos;
                    capDirection = lineDir;
                }

                // Get the angle for the semicircle's outward direction
                float baseAngle = Mathf.Atan2(capDirection.z, capDirection.x);
                // Get a random angle within the 180-degree arc of the semicircle
                float randomAngleInArc = ((float)random.NextDouble() - 0.5f) * Mathf.PI;
                float finalAngle = baseAngle + randomAngleInArc;
                
                float randomRadius = Mathf.Sqrt((float)random.NextDouble()) * maxRadius;

                potentialPosition = circleCenter + new Vector3(Mathf.Cos(finalAngle) * randomRadius, 0, Mathf.Sin(finalAngle) * randomRadius);
                normalizedDistanceForCurve = randomRadius / maxRadius;
            }

            float probability = distributionCurve.Evaluate(normalizedDistanceForCurve);
            if ((float)random.NextDouble() > probability)
            {
                continue;
            }

            if (usePerlinFilter)
            {
                float noiseX = (potentialPosition.x + noiseOffsetX) * perlinScale;
                float noiseY = (potentialPosition.z + noiseOffsetY) * perlinScale;
                if (Mathf.PerlinNoise(noiseX, noiseY) < perlinThreshold)
                {
                    continue;
                }
            }

            GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
            newInstance.transform.position = potentialPosition;
            if (parentToCenter) newInstance.transform.SetParent(startPoint);

            lastGeneratedObjects.Add(newInstance);
        }

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        Debug.Log($"Made {totalAttempts} attempts and placed {lastGeneratedObjects.Count} objects.");
    }
}