using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPaletteManager", menuName = "Game/Color Palette Manager")]
public class ColorPaletteManager : ScriptableObject
{
    private static ColorPaletteManager _instance;

    [Header("Core Colors")]
    [SerializeField] private Color coreColor1; // Core color 1
    [SerializeField] private Color coreColor2; // Core color 2
    [SerializeField] private Color coreColor3; // Core color 3

    [Header("Call-To-Action Colors")]
    [SerializeField] private Color ctaColor1; // CTA color 1
    [SerializeField] private Color ctaColor2; // CTA color 2

    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static ColorPaletteManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ColorPaletteManager>("ColorPaletteManager");
                if (_instance == null)
                {
                    Debug.LogError("ColorPaletteManager asset is missing in Resources folder!");
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Refresh all ColoredElement instances
    /// </summary>
    public void RefreshColors()
    {
        ColoredElement[] coloredElements = FindObjectsByType<ColoredElement>(FindObjectsSortMode.None);
        foreach (var element in coloredElements)
        {
            element.UpdateColor();
        }
    }

    public Color GetColor(PaletteColor color)
    {
        return color switch
        {
            PaletteColor.Core1 => coreColor1,
            PaletteColor.Core2 => coreColor2,
            PaletteColor.Core3 => coreColor3,
            PaletteColor.CTA1 => ctaColor1,
            PaletteColor.CTA2 => ctaColor2,
            _ => Color.white,
        };
    }

    // Automatically refresh colors whenever a color is modified in the Inspector
    private void OnValidate()
    {
        RefreshColors();
    }
}


public enum PaletteColor
{
    Ignore,
    Core1,
    Core2,
    Core3,
    CTA1,
    CTA2
}
