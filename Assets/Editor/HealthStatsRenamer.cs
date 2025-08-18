#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Renames every HealthStatsSO asset in the currently selected Project window folder(s)
/// to match its maxHealth value (cast to int). Example: maxHealth = 100f => asset name "100".
/// </summary>
public static class HealthStatsRenamer
{
    private const string MenuRoot = "Tools/Health Stats/";

    [MenuItem(MenuRoot + "Rename HealthStatsSO In Current Folder", priority = 200)]
    private static void RenameInCurrentFolder()
    {
        // Determine target folders based on current selection.
        var folderPaths = GetSelectedFolderAssetPaths();
        if (folderPaths.Count == 0)
        {
            Debug.LogWarning("[HealthStatsRenamer] Select a folder (or assets inside a folder) in the Project window first.");
            return;
        }

        int total = 0;
        int renamed = 0;
        foreach (var folder in folderPaths)
        {
            string[] guids = AssetDatabase.FindAssets("t:HealthStatsSO", new[] { folder });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<HealthStatsSO>(path);
                if (asset == null) continue;
                total++;

                // Compute desired new name from maxHealth (cast to int).
                int healthInt = Mathf.RoundToInt(asset.maxHealth); // rounding for nicer naming (100.4->100, 100.5->101)
                string desiredName = healthInt.ToString();

                // Skip if already correct.
                string currentName = Path.GetFileNameWithoutExtension(path);
                if (currentName == desiredName) continue;

                // Handle name collisions inside same folder.
                string folderDir = Path.GetDirectoryName(path).Replace('\\', '/');
                string uniqueName = desiredName;
                int attempt = 1;
                while (AssetExistsInFolder(folderDir, uniqueName) && attempt < 100)
                {
                    uniqueName = desiredName + "_" + attempt;
                    attempt++;
                }

                string error = AssetDatabase.RenameAsset(path, uniqueName);
                if (string.IsNullOrEmpty(error))
                {
                    renamed++;
                }
                else
                {
                    Debug.LogError($"[HealthStatsRenamer] Failed to rename {currentName} -> {uniqueName}: {error}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[HealthStatsRenamer] Processed {total} HealthStatsSO assets. Renamed {renamed}.");
    }

    // Validate menu (only enabled if something is selected)
    [MenuItem(MenuRoot + "Rename HealthStatsSO In Current Folder", validate = true)]
    private static bool ValidateRename()
    {
        return Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0;
    }

    /// <summary>
    /// Returns a deduplicated list of folder asset paths implied by the current selection.
    /// </summary>
    private static List<string> GetSelectedFolderAssetPaths()
    {
        var result = new HashSet<string>();
        foreach (var guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;
            if (AssetDatabase.IsValidFolder(path))
            {
                result.Add(path);
            }
            else
            {
                string dir = Path.GetDirectoryName(path).Replace('\\', '/');
                if (!string.IsNullOrEmpty(dir))
                    result.Add(dir);
            }
        }
        return new List<string>(result);
    }

    private static bool AssetExistsInFolder(string folderPath, string nameWithoutExtension)
    {
        string[] assets = AssetDatabase.FindAssets(nameWithoutExtension, new[] { folderPath });
        foreach (var guid in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetDirectoryName(path).Replace('\\', '/') == folderPath
                && Path.GetFileNameWithoutExtension(path) == nameWithoutExtension)
            {
                return true;
            }
        }
        return false;
    }
}
#endif
