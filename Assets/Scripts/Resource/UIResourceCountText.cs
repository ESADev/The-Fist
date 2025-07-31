using TMPro;
using UnityEngine;

public class UIResourceCountText : MonoBehaviour
{
    [SerializeField] Resource resourceToDisplay;
    [SerializeField] TextMeshProUGUI textToAssign;

    void Start()
    {
        RefreshText();
    }

    public void RefreshText()
    {
        textToAssign.text = FormattingHelper.FormatNumber(resourceToDisplay.GetResourceAmount(), 0);
    }
}
