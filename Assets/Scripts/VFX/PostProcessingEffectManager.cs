using UnityEngine;
using MoreMountains.Feedbacks;

/// <summary>
/// Controls global post-processing effects.
/// Requires the Universal Render Pipeline (URP) or Post-Processing Stack package.
/// Integrates with More Mountains FEEL feedback players.
/// </summary>
public class PostProcessingEffectManager : MonoBehaviour
{
    [Header("FEEL Feedbacks")]
    [Tooltip("Feedback player triggered when the low health effect is active.")]
    public MMF_Player lowHealthFeedback;
    /// <summary>
    /// Toggles the visual effect that indicates low health.
    /// </summary>
    /// <param name="isActive">Whether the effect should be active.</param>
    public void ToggleLowHealthEffect(bool isActive)
    {
        if (lowHealthFeedback == null)
        {
            Debug.LogError("[PostProcessingEffectManager] Low health feedback is not assigned.", this);
            return;
        }

        if (isActive)
        {
            lowHealthFeedback.PlayFeedbacks();
        }
        else
        {
            lowHealthFeedback.StopFeedbacks();
        }

        Debug.Log($"[PostProcessingEffectManager] Low health effect active: {isActive}.");
    }
}
