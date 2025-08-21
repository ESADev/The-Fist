using UnityEngine;

/// <summary>
/// Optional helper to trigger a LevelSelectUI center operation after one frame (layout settle) when enabled.
/// If you set autoCenterOnEnable in LevelSelectUI you don't need this; provided as alternative if you want manual sequencing.
/// </summary>
public class AutoCenterOnEnable : MonoBehaviour
{
    public LevelSelectUI levelSelectUI;
    public bool animate = true;

    private void OnEnable()
    {
        if (levelSelectUI == null) levelSelectUI = GetComponent<LevelSelectUI>();
        if (levelSelectUI != null)
        {
            // just call refresh which internally will center. If you want delay, you can implement coroutine.
            levelSelectUI.RefreshAll();
        }
    }
}
