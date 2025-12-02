using UnityEditor;
using UnityEngine;
using System.IO;

public class ImageQuantizerWindow : EditorWindow
{
    // Root folder for searching images
    string rootFolder = "Assets";
    int quantizationLevels = 8;
    Vector2Int pixelationSize = new Vector2Int(32, 32);
    ComputeShader quantizeComputeShader;

    [MenuItem("Tools/Image Quantizer")]
    public static void ShowWindow()
    {
        GetWindow<ImageQuantizerWindow>("Image Quantizer");
    }

    void OnGUI()
    {
        GUILayout.Label("Image Quantization Settings", EditorStyles.boldLabel);
        rootFolder = EditorGUILayout.TextField("Root Folder", rootFolder);
        quantizationLevels = EditorGUILayout.IntSlider("Quantization Levels", quantizationLevels, 2, 256);
        pixelationSize = EditorGUILayout.Vector2IntField("Pixelation Size", pixelationSize);
        quantizeComputeShader = (ComputeShader)EditorGUILayout.ObjectField(
            "Compute Shader (optional)", quantizeComputeShader, typeof(ComputeShader), false);

        if (GUILayout.Button("Quantize Images"))
        {
            if (EditorUtility.DisplayDialog(
                    "Quantize Images",
                    $"This will create backups (if missing) and quantize originals from backups under '{rootFolder}'. Continue?",
                    "Yes", "No"))
            {
                QuantizeImages(quantizationLevels, rootFolder, quantizeComputeShader, pixelationSize);
            }
        }

        if (GUILayout.Button("Rollback Images"))
        {
            if (EditorUtility.DisplayDialog(
                    "Rollback Images",
                    $"This will restore original images from backups under '{rootFolder}'. Continue?",
                    "Yes", "No"))
            {
                RollbackImages(rootFolder);
            }
        }
    }

    static void QuantizeImages(int levels, string rootFolder, ComputeShader shader, Vector2Int pixelationSize)
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { rootFolder });
        int total = guids.Length;
        int processed = 0;

        for (int i = 0; i < total; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string ext = Path.GetExtension(assetPath).ToLower();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
                continue;

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (fileName.EndsWith("_backup"))
                continue; // skip backup files

            string directory = Path.GetDirectoryName(assetPath).Replace("\\", "/");
            string backupAssetPath = $"{directory}/{fileName}_backup{ext}";
            string relBackup = backupAssetPath.Substring("Assets/".Length);
            string fullBackup = Path.Combine(Application.dataPath, relBackup);

            // Ensure backup exists
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(backupAssetPath) == null)
                AssetDatabase.CopyAsset(assetPath, backupAssetPath);

            if (!File.Exists(fullBackup))
                continue;

            // Load original from backup
            byte[] srcBytes = File.ReadAllBytes(fullBackup);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(srcBytes);

            var pixelatedTex = ApplyPixelation(tex, pixelationSize);
            Object.DestroyImmediate(tex);

            Texture2D quantizedTex = null;
            if (shader != null)
            {
                // GPU path
                int w = pixelatedTex.width, h = pixelatedTex.height;
                var rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32)
                {
                    enableRandomWrite = true
                };
                rt.Create();
                Graphics.Blit(pixelatedTex, rt);

                int kernel = shader.FindKernel("CSMain");
                shader.SetTexture(kernel, "Result", rt);
                shader.SetInt("Levels", levels);
                shader.Dispatch(kernel, Mathf.CeilToInt(w / 8f), Mathf.CeilToInt(h / 8f), 1);

                quantizedTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                RenderTexture.active = rt;
                quantizedTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                quantizedTex.Apply();
                RenderTexture.active = null;
                rt.Release();
            }
            else
            {
                // CPU path
                quantizedTex = new Texture2D(pixelatedTex.width, pixelatedTex.height, TextureFormat.RGBA32, false);
                var pixels = pixelatedTex.GetPixels();
                for (int p = 0; p < pixels.Length; p++)
                {
                    Color c = pixels[p];
                    c.r = Mathf.Round(c.r * (levels - 1)) / (levels - 1);
                    c.g = Mathf.Round(c.g * (levels - 1)) / (levels - 1);
                    c.b = Mathf.Round(c.b * (levels - 1)) / (levels - 1);
                    pixels[p] = c;
                }
                quantizedTex.SetPixels(pixels);
                quantizedTex.Apply();
            }

            Object.DestroyImmediate(pixelatedTex);

            // Write quantized image back to original
            string relOriginal = assetPath.Substring("Assets/".Length);
            string fullOriginal = Path.Combine(Application.dataPath, relOriginal);
            byte[] outBytes = assetPath.EndsWith(".png") ? quantizedTex.EncodeToPNG() : quantizedTex.EncodeToJPG(100);
            File.WriteAllBytes(fullOriginal, outBytes);
            Object.DestroyImmediate(quantizedTex);

            processed++;
            EditorUtility.DisplayProgressBar(
                "Quantizing Images",
                $"Quantizing {fileName}{ext} ({processed})",
                (float)processed / total
            );
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", $"Quantized {processed} images using levels={levels}.", "OK");
    }

    static Texture2D ApplyPixelation(Texture2D source, Vector2Int targetSize)
    {
        int width = source.width;
        int height = source.height;
        int targetWidth = Mathf.Clamp(targetSize.x, 1, width);
        int targetHeight = Mathf.Clamp(targetSize.y, 1, height);

        float cellWidth = (float)width / targetWidth;
        float cellHeight = (float)height / targetHeight;

        var sourcePixels = source.GetPixels();
        var blockColors = new Color[targetWidth * targetHeight];

        for (int y = 0; y < targetHeight; y++)
        {
            int srcY = Mathf.Clamp(Mathf.FloorToInt(y * cellHeight + cellHeight * 0.5f), 0, height - 1);
            for (int x = 0; x < targetWidth; x++)
            {
                int srcX = Mathf.Clamp(Mathf.FloorToInt(x * cellWidth + cellWidth * 0.5f), 0, width - 1);
                blockColors[y * targetWidth + x] = sourcePixels[srcY * width + srcX];
            }
        }

        var resultPixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            int blockY = Mathf.Min(Mathf.FloorToInt(y / cellHeight), targetHeight - 1);
            for (int x = 0; x < width; x++)
            {
                int blockX = Mathf.Min(Mathf.FloorToInt(x / cellWidth), targetWidth - 1);
                resultPixels[y * width + x] = blockColors[blockY * targetWidth + blockX];
            }
        }

        var pixelated = new Texture2D(width, height, TextureFormat.RGBA32, false);
        pixelated.SetPixels(resultPixels);
        pixelated.Apply();
        return pixelated;
    }

    static void RollbackImages(string rootFolder)
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { rootFolder });
        int total = guids.Length;
        int restored = 0;

        for (int i = 0; i < total; i++)
        {
            string backupAssetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string fileName = Path.GetFileNameWithoutExtension(backupAssetPath);
            if (!fileName.EndsWith("_backup"))
                continue;

            string ext = Path.GetExtension(backupAssetPath);
            string directory = Path.GetDirectoryName(backupAssetPath).Replace("\\", "/");
            string originalAssetPath = $"{directory}/{fileName.Substring(0, fileName.Length - 7)}{ext}";

            string relBackup = backupAssetPath.Substring("Assets/".Length);
            string fullBackup = Path.Combine(Application.dataPath, relBackup);
            string relOriginal = originalAssetPath.Substring("Assets/".Length);
            string fullOriginal = Path.Combine(Application.dataPath, relOriginal);

            if (File.Exists(fullBackup))
            {
                File.Copy(fullBackup, fullOriginal, true);
                restored++;
            }

            EditorUtility.DisplayProgressBar(
                "Rolling Back Images",
                $"Restoring {Path.GetFileName(originalAssetPath)} ({restored})",
                (float)i / total
            );
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", $"Restored {restored} images from backups.", "OK");
    }
}

// Note: Your ComputeShader asset must define a kernel "CSMain" with signature:
// RWTexture2D<float4> Result;
// int Levels;
// [numthreads(8,8,1)]
// void CSMain(uint3 id : SV_DispatchThreadID) {
//     float4 c = Result[id.xy];
//     c.rgb = round(c.rgb * (Levels - 1)) / (Levels - 1);
//     Result[id.xy] = c;
// }
