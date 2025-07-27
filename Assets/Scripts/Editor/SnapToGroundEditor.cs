using UnityEditor;
using UnityEngine;

public static class SnapToGroundEditor
{
    // Binds Ctrl+Q (Cmd+Q on macOS) to this menu command
    [MenuItem("Tools/Snap Bottom To Ground %q", false, 0)]
    private static void SnapSelectedToGround()
    {
        var sel = Selection.gameObjects;
        if (sel.Length == 0)
        {
            Debug.LogWarning("Nothing selected to snap.");
            return;
        }

        foreach (var obj in sel)
        {
            if (!TryGetWorldBounds(obj, out Bounds bounds))
            {
                Debug.LogWarning($"No Renderer found under '{obj.name}'.");
                continue;
            }

            float bottomY = bounds.min.y;
            float offsetY = -bottomY;
            Undo.RecordObject(obj.transform, "Snap Bottom To Ground");
            obj.transform.position += new Vector3(0, offsetY, 0);
        }
    }

    // Optional validator: greys out the menu item if nothing selected
    [MenuItem("Tools/Snap Bottom To Ground %q", true)]
    private static bool SnapSelectedToGround_Validate()
    {
        return Selection.activeGameObject != null;
    }

    // Collects/combines all child Renderers’ bounds into world-space Bounds
    private static bool TryGetWorldBounds(GameObject obj, out Bounds totalBounds)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        totalBounds = new Bounds();

        if (renderers == null || renderers.Length == 0)
            return false;

        totalBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            totalBounds.Encapsulate(renderers[i].bounds);

        return true;
    }
}
