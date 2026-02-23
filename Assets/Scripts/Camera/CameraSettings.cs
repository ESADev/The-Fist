
using UnityEngine;

/// <summary>
/// A ScriptableObject to hold the settings for the camera controller.
/// This allows for a data-driven approach, where camera behavior can be tweaked by designers
/// without changing any code. You can create multiple instances of these settings for different
/// situations (e.g., gameplay, cutscenes).
/// </summary>
[CreateAssetMenu(fileName = "CameraSettings", menuName = "Settings/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [Header("Following")]
    [Tooltip("How smoothly the camera follows the target. Lower values are smoother.")]
    //[Range(0.1f, 10f)]
    public float followSpeed = 5f;

    [Tooltip("The time to wait before the camera starts recentering on the target.")]
    [Range(0f, 5f)]
    public float centerDelay = 1f;

    [Header("Zoom")]
    [Tooltip("How smoothly the camera zooms. Lower values are smoother.")]
    [Range(0.1f, 10f)]
    public float zoomSpeed = 5f;

    [Tooltip("The minimum perspective angle for the camera.")]
    [Range(0.1f, 180f)]
    public float minZoom = 25f;

    [Tooltip("The maximum perspective angle for the camera.")]
    [Range(0.1f, 180f)]
    public float maxZoom = 40f;
}
