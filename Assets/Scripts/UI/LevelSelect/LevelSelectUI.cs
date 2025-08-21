using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the level selection map UI. Expose a list to manually assign buttons placed arbitrarily on the ScrollRect content.
/// Smoothly recenters on the currently selected level at start.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("References")] 
    [Tooltip("ScrollRect containing the level buttons.")] public ScrollRect scrollRect;
    [Tooltip("All level buttons (manually assign in the order you want, positions arbitrary)")] public List<LevelSelectButton> levelButtons = new();
    [Tooltip("UI text component showing the current level's display name.")] public TMP_Text levelNameText;
    [Tooltip("Seconds for smooth centering animation on open.")] public float centerDuration = 0.5f;
    [Tooltip("Easing curve for centering.")] public AnimationCurve easing = AnimationCurve.EaseInOut(0,0,1,1);
    [Tooltip("When true, will auto-center on open.")] public bool autoCenterOnEnable = true;

    private Coroutine _centerRoutine;

    private void Start()
    {
        GameManager.OnLevelChanged += HandleLevelChanged;
        GameManager.OnHighestUnlockedLevelChanged += HandleHighestUnlockedChanged;
        if (autoCenterOnEnable)
            CenterOnCurrentLevelImmediateOrAnimated(true);
        RefreshAll();
    }

    private void OnDisable()
    {
        GameManager.OnLevelChanged -= HandleLevelChanged;
        GameManager.OnHighestUnlockedLevelChanged -= HandleHighestUnlockedChanged;
    }

    private void HandleLevelChanged(int idx)
    {
        RefreshAll();
        CenterOnCurrentLevelImmediateOrAnimated(false);
    }

    private void HandleHighestUnlockedChanged(int idx)
    {
        RefreshAll();
    }

    /// <summary>
    /// Refresh button visuals according to current/unlocked state.
    /// </summary>
    public void RefreshAll()
    {
        if (GameManager.Instance == null) return;
        int current = GameManager.Instance.CurrentLevelIndex;
        int highest = GameManager.Instance.HighestUnlockedLevelIndex;
        foreach (var btn in levelButtons)
        {
            if (btn != null)
                btn.Refresh(current, highest);
        }

        // Update level name text.
        if (levelNameText != null)
        {
            string displayName = "";
            if (current >= 0 && GameManager.Instance.levels != null && current < GameManager.Instance.levels.Count && GameManager.Instance.levels[current] != null)
            {
                displayName = GameManager.Instance.levels[current].levelName;
            }
            levelNameText.text = displayName;
        }
    }

    /// <summary>
    /// Called by master Play button.
    /// </summary>
    public void PlayCurrentLevel()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.PlayCurrentLevel();
    }

    private void CenterOnCurrentLevelImmediateOrAnimated(bool animate)
    {
        if (scrollRect == null || GameManager.Instance == null) return;
        int current = GameManager.Instance.CurrentLevelIndex;
        if (current < 0 || current >= levelButtons.Count) return;
        var target = levelButtons[current];
        if (target == null) return;

        // Convert target position inside content into normalized scroll position.
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform targetRT = target.transform as RectTransform;
        if (content == null || viewport == null || targetRT == null) return;

        // Calculate required normalized position for horizontal/vertical separately.
        Vector2 contentSize = content.rect.size;
        Vector2 viewportSize = viewport.rect.size;
        Vector2 targetLocalPos = (Vector2)content.InverseTransformPoint(targetRT.position);
        Vector2 contentPivotOffset = new Vector2(content.pivot.x * contentSize.x, content.pivot.y * contentSize.y);
        Vector2 targetPosInContentSpace = targetLocalPos + contentPivotOffset;

        float normalizedX = scrollRect.horizontal ? Mathf.Clamp01((targetPosInContentSpace.x - viewportSize.x * 0.5f) / Mathf.Max(1f, contentSize.x - viewportSize.x)) : scrollRect.horizontalNormalizedPosition;
        float normalizedY = scrollRect.vertical ? Mathf.Clamp01((targetPosInContentSpace.y - viewportSize.y * 0.5f) / Mathf.Max(1f, contentSize.y - viewportSize.y)) : scrollRect.verticalNormalizedPosition;
        // Unity vertical scroll normalized 0=bottom 1=top, we used top-left assumption above so invert if needed
        if (scrollRect.vertical) normalizedY = 1f - normalizedY;

        if (_centerRoutine != null) StopCoroutine(_centerRoutine);
        if (animate && centerDuration > 0f)
            _centerRoutine = StartCoroutine(AnimateScroll(normalizedX, normalizedY));
        else
        {
            if (scrollRect.horizontal) scrollRect.horizontalNormalizedPosition = normalizedX;
            if (scrollRect.vertical) scrollRect.verticalNormalizedPosition = normalizedY;
        }
    }

    private IEnumerator AnimateScroll(float targetX, float targetY)
    {
        float startX = scrollRect.horizontalNormalizedPosition;
        float startY = scrollRect.verticalNormalizedPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, centerDuration);
            float e = easing.Evaluate(Mathf.Clamp01(t));
            if (scrollRect.horizontal)
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startX, targetX, e);
            if (scrollRect.vertical)
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(startY, targetY, e);
            yield return null;
        }
    }
}
