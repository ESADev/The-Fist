using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ColoredElement : MonoBehaviour
{
    [SerializeField] private PaletteColor color; // Serialized to detect changes in the Inspector

    private int _componentIndex;
    private TextMeshProUGUI _textMeshProUGUI;
    private TextMeshPro _textMeshPro;
    private Image _image;
    private SpriteRenderer _spriteRenderer;

    private ColorPaletteManager Palette => ColorPaletteManager.Instance;

    private void OnEnable()
    {
        // Determine which component this GameObject has
        if (TryGetComponent(out _textMeshProUGUI))
        {
            _componentIndex = 1;
        }
        else if (TryGetComponent(out _image))
        {
            _componentIndex = 2;
        }
        else if (TryGetComponent(out _spriteRenderer))
        {
            _componentIndex = 3;
        }
        else if (TryGetComponent(out _textMeshPro))
        {
            _componentIndex = 4;
        }
        else
        {
            //Debug.LogError("ColoredElement: No TextMeshProUGUI, TextMeshPro, Image or SpriteRenderer found on this GameObject.");
            //Destroy(this);
            return;
        }

        UpdateColor();
    }

    public void UpdateColor()
    {
        if(color != PaletteColor.Ignore)
        {
            if (Palette == null) return;

            Color selectedColor = Palette.GetColor(color);

            switch (_componentIndex)
            {
                case 1:
                    _textMeshProUGUI.color = new(selectedColor.r, selectedColor.g, selectedColor.b, _textMeshProUGUI.color.a);
                    break;
                case 2:
                    _image.color = new(selectedColor.r, selectedColor.g, selectedColor.b, _image.color.a);
                    break;
                case 3:
                    _spriteRenderer.color = new(selectedColor.r, selectedColor.g, selectedColor.b, _spriteRenderer.color.a);
                    break;
                case 4:
                    _textMeshPro.color = new(selectedColor.r, selectedColor.g, selectedColor.b, _textMeshPro.color.a);
                    break;
                default:
                    //Debug.LogError("ColoredElement: No valid component found.");
                    //DestroyImmediate(this);
                    break;
            }
        }
    }

    private void OnValidate()
    {
        // Automatically update the color when the selected color changes in the Inspector
        UpdateColor();
    }
}
