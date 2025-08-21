using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to a GameObject (e.g., a Canvas button). Assign the Load() method to the Button's OnClick.
/// Loads scene build index 0. Optionally resets Time.timeScale.
/// </summary>
public class LoadScene0Button : MonoBehaviour
{
    [Header("Options")] 
    [Tooltip("Reset Time.timeScale to 1 before loading.")] 
    [SerializeField] private bool resetTimeScale = true;

    [Tooltip("If true, use Single mode (replace current scene). If false, loads Additively.")] 
    [SerializeField] private bool singleLoad = true;

    /// <summary>
    /// Called by UI Button OnClick. Loads scene index 0.
    /// </summary>
    public void Load()
    {
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }

        var mode = singleLoad ? LoadSceneMode.Single : LoadSceneMode.Additive;
        SceneManager.LoadScene(0, mode);
    }
}
