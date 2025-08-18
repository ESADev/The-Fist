#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Renames BountySO assets in the selected Project window folder(s) based on their resource list.
/// Example: 10 Prestige -> "10 Prestige"; 8 Prestige + 3 Gold -> "8 Prestige 3 Gold".
/// Order: Prestige, Gold (enum declaration order). Resources with amount <= 0 are skipped.
/// </summary>
public static class BountyRenamer
{
    private const string MenuRoot = "Tools/Bounty/";

    [MenuItem(MenuRoot + "Rename BountySO In Current Folder", priority = 201)]
    private static void RenameInCurrentFolder()
    {
        var folderPaths = GetSelectedFolderAssetPaths();
        if (folderPaths.Count == 0)
        {
            Debug.LogWarning("[BountyRenamer] Select a folder (or assets inside a folder) in the Project window first.");
            return;
        }

        int total = 0; int renamed = 0;
        foreach (var folder in folderPaths)
        {
            string[] guids = AssetDatabase.FindAssets("t:BountySO", new[] { folder });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<BountySO>(path);
                if (asset == null) continue;
                total++;

                string desiredName = BuildName(asset);
                if (string.IsNullOrEmpty(desiredName))
                {
                    desiredName = "Empty Bounty";
                }

                string currentName = Path.GetFileNameWithoutExtension(path);
                if (currentName == desiredName) continue;

                string folderDir = Path.GetDirectoryName(path).Replace('\\', '/');
                string uniqueName = MakeUnique(folderDir, desiredName);
                string error = AssetDatabase.RenameAsset(path, uniqueName);
                if (string.IsNullOrEmpty(error)) renamed++; else Debug.LogError($"[BountyRenamer] Failed to rename {currentName} -> {uniqueName}: {error}");
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[BountyRenamer] Processed {total} BountySO assets. Renamed {renamed}.");
    }

    [MenuItem(MenuRoot + "Rename BountySO In Current Folder", validate = true)]
    private static bool ValidateRename() => Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0;

    private static string BuildName(BountySO bounty)
    {
        if (bounty.resourcesToDrop == null || bounty.resourcesToDrop.Count == 0) return string.Empty;

        // Group by resource type (in case duplicates) and sum amounts > 0
        var grouped = bounty.resourcesToDrop
            .Where(r => r.amount > 0)
            .GroupBy(r => r.type)
            .Select(g => new { Type = g.Key, Amount = g.Sum(x => x.amount) })
            .OrderBy(g => g.Type); // enum order ensures Prestige before Gold

        var parts = new List<string>();
        foreach (var entry in grouped)
        {
            parts.Add($"{entry.Amount} {entry.Type}");
        }
        return string.Join(" ", parts);
    }

    private static string MakeUnique(string folderDir, string desiredName)
    {
        string unique = desiredName;
        int attempt = 1;
        while (AssetExistsInFolder(folderDir, unique) && attempt < 100)
        {
            unique = desiredName + "_" + attempt;
            attempt++;
        }
        return unique;
    }

    private static bool AssetExistsInFolder(string folderPath, string nameWithoutExtension)
    {
        string[] assets = AssetDatabase.FindAssets(nameWithoutExtension, new[] { folderPath });
        foreach (var guid in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetDirectoryName(path).Replace('\\', '/') == folderPath && Path.GetFileNameWithoutExtension(path) == nameWithoutExtension)
                return true;
        }
        return false;
    }

    private static List<string> GetSelectedFolderAssetPaths()
    {
        var result = new HashSet<string>();
        foreach (var guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;
            if (AssetDatabase.IsValidFolder(path)) result.Add(path); else {
                string dir = Path.GetDirectoryName(path).Replace('\\', '/');
                if (!string.IsNullOrEmpty(dir)) result.Add(dir);
            }
        }
        return new List<string>(result);
    }
}
#endif
