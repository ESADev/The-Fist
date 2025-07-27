using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GlbMaterialGenerator
{
    [MenuItem("Tools/Generate GLB Materials", false, 100)]
    public static void GenerateMaterialsForCurrentFolder()
    {
        // 1. Get the folder you have open in the Project window
        string folder = GetActiveProjectWindowFolder();
        if (string.IsNullOrEmpty(folder))
        {
            Debug.LogWarning("Couldn’t detect an open folder in the Project window.");
            return;
        }

        // 2. Compute its full system path
        string projectRoot = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        string relativeSub = folder.StartsWith("Assets/") 
            ? folder.Substring("Assets/".Length) 
            : folder;
        string folderFull = Path.Combine(projectRoot, "Assets", relativeSub);

        if (!Directory.Exists(folderFull))
        {
            Debug.LogError($"Folder path not found on disk:\n{folderFull}");
            return;
        }

        // 3. Find all .glb files under that folder (recursive)
        string[] glbFiles = Directory.GetFiles(folderFull, "*.glb", SearchOption.AllDirectories);
        if (glbFiles.Length == 0)
        {
            Debug.Log($"No .glb files found in '{folder}'.");
            return;
        }

        // 4. For each .glb, create a URP Lit material if it doesn't exist
        int created = 0;
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null)
        {
            Debug.LogError("URP Lit shader not found. Make sure URP is installed.");
            return;
        }

        foreach (string filePath in glbFiles)
        {
            // Convert to "Assets/..." style path
            string assetPath = filePath.Replace(projectRoot, "").Replace("\\", "/");
            string name = Path.GetFileNameWithoutExtension(assetPath);
            string matPath = Path.GetDirectoryName(assetPath) + "/" + name + ".mat";

            if (AssetDatabase.LoadAssetAtPath<Material>(matPath) != null)
                continue;

            var mat = new Material(urpLit);
            mat.SetFloat("_Smoothness", 0.1f);
            AssetDatabase.CreateAsset(mat, matPath);
            Debug.Log($"[GLB→MAT] Created: {matPath}");
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generate GLB Materials: {created} new material(s) created in '{folder}'.");
    }

    // Reflection hack: UnityEditor.ProjectWindowUtil.GetActiveFolderPath()
    private static string GetActiveProjectWindowFolder()
    {
        var utilType = typeof(ProjectWindowUtil);
        var mi = utilType.GetMethod(
            "GetActiveFolderPath",
            BindingFlags.Static | BindingFlags.NonPublic
        );
        return mi != null 
            ? (string)mi.Invoke(null, null) 
            : null;
    }
}
