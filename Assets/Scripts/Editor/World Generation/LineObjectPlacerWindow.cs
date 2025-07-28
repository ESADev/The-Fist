using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LineObjectPlacerWindow : EditorWindow
{
    private Transform startPoint;
    private Transform endPoint;
    private WorldGenerationRecipeSO activeRecipe;

    private bool parentToHolder = true;
    private static System.Random random = new System.Random();
    private const string HolderName = "__GeneratedObjectsHolder__";

    [MenuItem("Tools/World Recipe Placer")]
    public static void ShowWindow()
    {
        GetWindow<LineObjectPlacerWindow>("Recipe Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("World Recipe Placer", EditorStyles.boldLabel);
        activeRecipe = (WorldGenerationRecipeSO)EditorGUILayout.ObjectField("World Recipe", activeRecipe, typeof(WorldGenerationRecipeSO), false);
        EditorGUILayout.Space();
        startPoint = (Transform)EditorGUILayout.ObjectField("Start Point", startPoint, typeof(Transform), true);
        endPoint = (Transform)EditorGUILayout.ObjectField("End Point", endPoint, typeof(Transform), true);
        parentToHolder = EditorGUILayout.Toggle("Parent to Holder Object", parentToHolder);
        EditorGUILayout.Space();

        if (GUILayout.Button("(Re)generate Objects"))
        {
            if (activeRecipe == null || startPoint == null || endPoint == null)
            {
                Debug.LogError("Error: Assign a World Recipe, a Start Point, and an End Point.");
                return;
            }
            if(activeRecipe.prefabProfiles == null || activeRecipe.prefabProfiles.Count == 0)
            {
                Debug.LogError("Error: The assigned World Recipe has no Prefab Profiles.");
                return;
            }
            GenerateObjects();
        }
        if (GUILayout.Button("Undo Last Generation"))
        {
            GameObject existingHolder = GameObject.Find(HolderName);
            if(existingHolder != null)
            {
                Undo.DestroyObjectImmediate(existingHolder);
            }
        }
    }
    
    private void GenerateObjects()
    {
        Undo.SetCurrentGroupName("Generate From Recipe");

        GameObject existingHolder = GameObject.Find(HolderName);
        if (existingHolder != null) Undo.DestroyObjectImmediate(existingHolder);
        GameObject holder = new GameObject(HolderName);
        Undo.RegisterCreatedObjectUndo(holder, "Create Generated Objects");

        List<Bounds> placedObjectBounds = new List<Bounds>();
        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;
        startPos.y = 0; endPos.y = 0;

        float maxRadius = activeRecipe.pathRadius;
        
        float lineLength = Vector3.Distance(startPos, endPos);
        Vector3 lineDir = (lineLength > 0) ? (endPos - startPos).normalized : Vector3.right;
        float rectangleArea = lineLength * (2 * maxRadius);
        float circleArea = Mathf.PI * maxRadius * maxRadius;
        float totalArea = rectangleArea + circleArea;

        var attemptDeck = new List<PropProfile>();
        foreach (var profile in activeRecipe.prefabProfiles)
        {
            if(profile.prefabVariants.Count == 0) continue;
            int numAttempts = Mathf.RoundToInt(profile.density * totalArea);
            for (int i = 0; i < numAttempts; i++)
            {
                attemptDeck.Add(profile);
            }
        }

        for (int i = 0; i < attemptDeck.Count - 1; i++)
        {
            int j = random.Next(i, attemptDeck.Count);
            var temp = attemptDeck[i];
            attemptDeck[i] = attemptDeck[j];
            attemptDeck[j] = temp;
        }
        
        // --- THIS IS THE FIX ---
        // The noise offset must be generated only ONCE for the entire pass.
        float noiseOffsetX = (float)random.NextDouble() * 10000f;
        float noiseOffsetY = (float)random.NextDouble() * 10000f;
        
        foreach(var profile in attemptDeck)
        {
            // The offsets are no longer generated inside this loop.

            float rectProbability = (totalArea > 0) ? rectangleArea / totalArea : 0;
            Vector3 potentialPosition;
            float normalizedDistanceForCurve;

            if (random.NextDouble() < rectProbability)
            {
                float distAlongLine = (float)random.NextDouble() * lineLength;
                float perpDist = ((float)random.NextDouble() * 2f - 1f) * maxRadius;
                Vector3 perpDir = new Vector3(-lineDir.z, 0, lineDir.x);
                potentialPosition = startPos + (lineDir * distAlongLine) + (perpDir * perpDist);
                normalizedDistanceForCurve = Mathf.Abs(perpDist) / maxRadius;
            }
            else
            {
                Vector3 circleCenter = (random.NextDouble() < 0.5) ? startPos : endPos;
                Vector3 capDirection = (circleCenter == startPos) ? -lineDir : lineDir;
                float baseAngle = Mathf.Atan2(capDirection.z, capDirection.x);
                float randomAngleInArc = ((float)random.NextDouble() - 0.5f) * Mathf.PI;
                float finalAngle = baseAngle + randomAngleInArc;
                float randomRadius = Mathf.Sqrt((float)random.NextDouble()) * maxRadius;
                potentialPosition = circleCenter + new Vector3(Mathf.Cos(finalAngle) * randomRadius, 0, Mathf.Sin(finalAngle) * randomRadius);
                normalizedDistanceForCurve = randomRadius / maxRadius;
            }

            if (profile.distanceDistribution == null || (float)random.NextDouble() > profile.distanceDistribution.Evaluate(normalizedDistanceForCurve)) continue;
            
            if (profile.usePerlinFilter)
            {
                // This logic is now correct because it uses the consistent offsets from outside the loop.
                float noiseX = (potentialPosition.x + noiseOffsetX) * profile.perlinScale;
                float noiseY = (potentialPosition.z + noiseOffsetY) * profile.perlinScale;
                if (Mathf.PerlinNoise(noiseX, noiseY) < profile.perlinThreshold) continue;
            }

            GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(profile.prefabVariants[Random.Range(0, profile.prefabVariants.Count)]);
            tempInstance.transform.position = potentialPosition;
            
            if (profile.checkForIntersections)
            {
                Bounds newBounds = GetBoundsOfInstance(tempInstance);
                bool intersects = false;
                foreach (var existingBound in placedObjectBounds)
                {
                    if (newBounds.Intersects(existingBound)) { intersects = true; break; }
                }
                if (intersects) { DestroyImmediate(tempInstance); continue; }
                placedObjectBounds.Add(newBounds);
            }
            
            if (parentToHolder) tempInstance.transform.SetParent(holder.transform);
        }
        
        Debug.Log($"Generation complete. Processed {attemptDeck.Count} attempts and placed {holder.transform.childCount} objects.");
    }
    
    private Bounds GetBoundsOfInstance(GameObject instance)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(instance.transform.position, Vector3.one);
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }
}