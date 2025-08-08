#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Custom editor for VFXLibrary to provide better organization and validation tools.
/// </summary>
[CustomEditor(typeof(VFXLibrary))]
public class VFXLibraryEditor : Editor
{
    private VFXLibrary library;
    private bool showParticleEffects = true;
    private bool showPostProcessingEffects = true;
    private bool showCameraShakes = true;
    private bool showValidation = false;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        library = (VFXLibrary)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("VFX Library", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Validation section
        showValidation = EditorGUILayout.Foldout(showValidation, "Validation & Tools");
        if (showValidation)
        {
            EditorGUILayout.BeginVertical("box");
            
            if (GUILayout.Button("Validate All VFX"))
            {
                ValidateLibrary();
            }

            if (GUILayout.Button("Initialize Library"))
            {
                library.Initialize();
                EditorUtility.SetDirty(library);
            }

            if (GUILayout.Button("Show All Keys"))
            {
                ShowAllKeys();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // Scrollable area for large libraries
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Particle Effects section
        showParticleEffects = EditorGUILayout.Foldout(showParticleEffects, $"Particle Effects ({library.particleEffects.Count})");
        if (showParticleEffects)
        {
            EditorGUILayout.BeginVertical("box");
            DrawParticleEffectsSection();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // Post-Processing Effects section
        showPostProcessingEffects = EditorGUILayout.Foldout(showPostProcessingEffects, $"Post-Processing Effects ({library.postProcessingEffects.Count})");
        if (showPostProcessingEffects)
        {
            EditorGUILayout.BeginVertical("box");
            DrawPostProcessingEffectsSection();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // Camera Shakes section
        showCameraShakes = EditorGUILayout.Foldout(showCameraShakes, $"Camera Shakes ({library.cameraShakes.Count})");
        if (showCameraShakes)
        {
            EditorGUILayout.BeginVertical("box");
            DrawCameraShakesSection();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawParticleEffectsSection()
    {
        SerializedProperty particleEffectsProp = serializedObject.FindProperty("particleEffects");
        
        EditorGUILayout.PropertyField(particleEffectsProp, true);

        if (GUILayout.Button("Add New Particle Effect"))
        {
            library.particleEffects.Add(new ParticleEffectData());
            EditorUtility.SetDirty(library);
        }
    }

    private void DrawPostProcessingEffectsSection()
    {
        SerializedProperty postProcessingEffectsProp = serializedObject.FindProperty("postProcessingEffects");
        
        EditorGUILayout.PropertyField(postProcessingEffectsProp, true);

        if (GUILayout.Button("Add New Post-Processing Effect"))
        {
            library.postProcessingEffects.Add(new PostProcessingEffectData());
            EditorUtility.SetDirty(library);
        }
    }

    private void DrawCameraShakesSection()
    {
        SerializedProperty cameraShakesProp = serializedObject.FindProperty("cameraShakes");
        
        EditorGUILayout.PropertyField(cameraShakesProp, true);

        if (GUILayout.Button("Add New Camera Shake"))
        {
            library.cameraShakes.Add(new CameraShakeData());
            EditorUtility.SetDirty(library);
        }
    }

    private void ValidateLibrary()
    {
        library.Initialize();
        bool isValid = library.ValidateLibrary();
        
        if (isValid)
        {
            EditorUtility.DisplayDialog("Validation Result", "All VFX configurations are valid!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Validation Result", "Some VFX configurations have errors. Check the console for details.", "OK");
        }
    }

    private void ShowAllKeys()
    {
        library.Initialize();
        var allKeys = library.GetAllVFXKeys();
        
        string message = "VFX Library Keys:\n\n";
        
        foreach (var kvp in allKeys)
        {
            message += $"{kvp.Key}:\n";
            foreach (string key in kvp.Value)
            {
                message += $"  - {key}\n";
            }
            message += "\n";
        }

        EditorUtility.DisplayDialog("All VFX Keys", message, "OK");
    }
}

/// <summary>
/// Custom property drawer for VFX data base classes to provide better visualization.
/// </summary>
[CustomPropertyDrawer(typeof(VFXDataBase), true)]
public class VFXDataBasePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the default property field
        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
