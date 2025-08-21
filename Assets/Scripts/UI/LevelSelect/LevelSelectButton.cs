using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single level entry button in the level select UI.
/// Status visuals are updated based on current / locked / completed state provided by the manager.
/// </summary>
[DisallowMultipleComponent]
public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Index of the level this button represents (matches GameManager levels ordering).")]
    public int levelIndex;

    [Header("Visuals")]
    public Image iconImage;
    [Tooltip("Sprite for locked level.")]
    public Sprite lockedSprite;
    [Tooltip("Sprite for unlocked but not yet completed (empty) level.")]
    public Sprite emptySprite;
    [Tooltip("Sprite for completed level.")]
    public Sprite completedSprite;

    [Tooltip("Optional highlight GameObject (e.g., outline) to show when this is the currently selected level.")]
    public GameObject selectionHighlight;

    /// <summary>
    /// Called by the container to refresh state.
    /// </summary>
    public void Refresh(int currentIndex, int highestUnlockedIndex)
    {
        if (iconImage == null) return;
        bool isLocked = levelIndex > highestUnlockedIndex;
        bool isCurrent = levelIndex == currentIndex;
        bool isCompleted = levelIndex < highestUnlockedIndex; // completed if below highest unlocked

        if (isLocked && lockedSprite != null)
            iconImage.sprite = lockedSprite;
        else if (isCompleted && completedSprite != null)
            iconImage.sprite = completedSprite;
        else if (emptySprite != null)
            iconImage.sprite = emptySprite;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(isCurrent);

        // Optional: disable interaction when locked.
        var selectable = GetComponent<Selectable>();
        if (selectable != null)
            selectable.interactable = !isLocked;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.SetCurrentLevel(levelIndex);
    }
}
