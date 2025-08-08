using UnityEngine;
using MoreMountains.Feedbacks;

/// <summary>
/// Handles camera shake effects using More Mountains FEEL feedback players.
/// </summary>
public class CameraShakeManager : MonoBehaviour
{
    [Header("FEEL Feedbacks")]
    [Tooltip("Feedback player used for light camera shakes.")]
    public MMF_Player lightShakeFeedback;

    [Tooltip("Feedback player used for medium camera shakes.")]
    public MMF_Player mediumShakeFeedback;

    [Tooltip("Feedback player used for heavy camera shakes.")]
    public MMF_Player heavyShakeFeedback;

    /// <summary>
    /// Intensity levels for camera shake.
    /// </summary>
    public enum ShakeIntensity
    {
        Light,
        Medium,
        Heavy
    }

    /// <summary>
    /// Shakes the camera with the specified intensity.
    /// </summary>
    /// <param name="intensity">Intensity level of the shake.</param>
    public void Shake(ShakeIntensity intensity)
    {
        MMF_Player player = null;

        switch (intensity)
        {
            case ShakeIntensity.Light:
                player = lightShakeFeedback;
                break;
            case ShakeIntensity.Medium:
                player = mediumShakeFeedback;
                break;
            case ShakeIntensity.Heavy:
                player = heavyShakeFeedback;
                break;
        }

        if (player == null)
        {
            Debug.LogError($"[CameraShakeManager] MMF_Player for intensity {intensity} is not assigned.", this);
            return;
        }

        player.PlayFeedbacks();
        Debug.Log($"[CameraShakeManager] FEEL shake triggered with intensity {intensity}.");
    }
}
