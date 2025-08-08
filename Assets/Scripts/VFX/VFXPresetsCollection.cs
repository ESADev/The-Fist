using UnityEngine;

/// <summary>
/// Collection of commonly used VFX presets for quick setup.
/// Create an instance of this asset to get pre-configured VFX data.
/// </summary>
[CreateAssetMenu(menuName = "VFX/VFX Presets Collection", fileName = "VFXPresets")]
public class VFXPresetsCollection : ScriptableObject
{
    [Header("Common Particle Effect Presets")]
    [Tooltip("Standard hit effect for basic damage.")]
    public ParticleEffectData basicHitEffect;

    [Tooltip("Critical hit effect for high damage.")]
    public ParticleEffectData criticalHitEffect;

    [Tooltip("Explosion effect for area damage.")]
    public ParticleEffectData explosionEffect;

    [Tooltip("Death effect when units are destroyed.")]
    public ParticleEffectData deathEffect;

    [Header("Common Camera Shake Presets")]
    [Tooltip("Light shake for minor impacts.")]
    public CameraShakeData lightShake;

    [Tooltip("Medium shake for moderate impacts.")]
    public CameraShakeData mediumShake;

    [Tooltip("Heavy shake for major impacts.")]
    public CameraShakeData heavyShake;

    [Tooltip("Explosion shake with unique characteristics.")]
    public CameraShakeData explosionShake;

    [Header("Common Post-Processing Effect Presets")]
    [Tooltip("Low health warning effect.")]
    public PostProcessingEffectData lowHealthEffect;

    [Tooltip("Damage flash effect.")]
    public PostProcessingEffectData damageFlashEffect;

    [Tooltip("Victory celebration effect.")]
    public PostProcessingEffectData victoryEffect;

    [Tooltip("Screen distortion for special attacks.")]
    public PostProcessingEffectData distortionEffect;

    /// <summary>
    /// Initializes all presets with default values.
    /// Call this to set up commonly used configurations.
    /// </summary>
    [ContextMenu("Initialize Default Presets")]
    public void InitializeDefaultPresets()
    {
        InitializeParticleEffectPresets();
        InitializeCameraShakePresets();
        InitializePostProcessingPresets();

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    private void InitializeParticleEffectPresets()
    {
        // Basic Hit Effect
        basicHitEffect = new ParticleEffectData
        {
            key = "basic_hit",
            displayName = "Basic Hit Effect",
            priority = 1,
            autoDestroy = true,
            maxLifetime = 5f,
            playOnAwake = true,
            scale = 1f
        };

        // Critical Hit Effect
        criticalHitEffect = new ParticleEffectData
        {
            key = "critical_hit",
            displayName = "Critical Hit Effect",
            priority = 2,
            autoDestroy = true,
            maxLifetime = 3f,
            playOnAwake = true,
            scale = 1.5f
        };

        // Explosion Effect
        explosionEffect = new ParticleEffectData
        {
            key = "explosion",
            displayName = "Explosion Effect",
            priority = 3,
            autoDestroy = true,
            maxLifetime = 8f,
            playOnAwake = true,
            scale = 2f
        };

        // Death Effect
        deathEffect = new ParticleEffectData
        {
            key = "unit_death",
            displayName = "Unit Death Effect",
            priority = 2,
            autoDestroy = true,
            maxLifetime = 6f,
            playOnAwake = true,
            scale = 1.2f
        };
    }

    private void InitializeCameraShakePresets()
    {
        // Light Shake
        lightShake = new CameraShakeData
        {
            key = "light_shake",
            displayName = "Light Camera Shake",
            priority = 1,
            magnitude = 0.5f,
            duration = 0.2f,
            frequency = 25f,
            shakePosition = true,
            shakeRotation = false,
            xMultiplier = 1f,
            yMultiplier = 1f,
            zMultiplier = 0.5f
        };

        // Medium Shake
        mediumShake = new CameraShakeData
        {
            key = "medium_shake",
            displayName = "Medium Camera Shake",
            priority = 2,
            magnitude = 1f,
            duration = 0.4f,
            frequency = 30f,
            shakePosition = true,
            shakeRotation = true,
            xMultiplier = 1f,
            yMultiplier = 1f,
            zMultiplier = 0.7f
        };

        // Heavy Shake
        heavyShake = new CameraShakeData
        {
            key = "heavy_shake",
            displayName = "Heavy Camera Shake",
            priority = 3,
            magnitude = 2f,
            duration = 0.8f,
            frequency = 35f,
            shakePosition = true,
            shakeRotation = true,
            xMultiplier = 1.2f,
            yMultiplier = 1.2f,
            zMultiplier = 1f
        };

        // Explosion Shake
        explosionShake = new CameraShakeData
        {
            key = "explosion_shake",
            displayName = "Explosion Camera Shake",
            priority = 4,
            magnitude = 3f,
            duration = 1.2f,
            frequency = 40f,
            shakePosition = true,
            shakeRotation = true,
            xMultiplier = 1.5f,
            yMultiplier = 1.5f,
            zMultiplier = 1.2f
        };

        // Set up dampening curves
        AnimationCurve defaultDampening = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        lightShake.dampening = defaultDampening;
        mediumShake.dampening = defaultDampening;
        heavyShake.dampening = defaultDampening;
        explosionShake.dampening = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    }

    private void InitializePostProcessingPresets()
    {
        // Low Health Effect
        lowHealthEffect = new PostProcessingEffectData
        {
            key = "low_health",
            displayName = "Low Health Warning",
            priority = 5,
            fadeInDuration = 0.5f,
            mainDuration = 0f, // Continuous until stopped
            fadeOutDuration = 0.5f,
            maxWeight = 0.8f,
            blendMode = VolumeBlendMode.Override
        };

        // Damage Flash Effect
        damageFlashEffect = new PostProcessingEffectData
        {
            key = "damage_flash",
            displayName = "Damage Flash",
            priority = 3,
            fadeInDuration = 0.1f,
            mainDuration = 0.2f,
            fadeOutDuration = 0.3f,
            maxWeight = 1f,
            blendMode = VolumeBlendMode.Additive
        };

        // Victory Effect
        victoryEffect = new PostProcessingEffectData
        {
            key = "victory",
            displayName = "Victory Celebration",
            priority = 8,
            fadeInDuration = 1f,
            mainDuration = 3f,
            fadeOutDuration = 2f,
            maxWeight = 1f,
            blendMode = VolumeBlendMode.Override
        };

        // Distortion Effect
        distortionEffect = new PostProcessingEffectData
        {
            key = "screen_distortion",
            displayName = "Screen Distortion",
            priority = 4,
            fadeInDuration = 0.2f,
            mainDuration = 0.5f,
            fadeOutDuration = 0.4f,
            maxWeight = 0.6f,
            blendMode = VolumeBlendMode.Multiply
        };
    }

    /// <summary>
    /// Creates a VFXLibrary asset populated with these presets.
    /// </summary>
    [ContextMenu("Create Library from Presets")]
    public void CreateLibraryFromPresets()
    {
        #if UNITY_EDITOR
        var library = CreateInstance<VFXLibrary>();
        
        // Add particle effects
        if (basicHitEffect != null) library.particleEffects.Add(basicHitEffect);
        if (criticalHitEffect != null) library.particleEffects.Add(criticalHitEffect);
        if (explosionEffect != null) library.particleEffects.Add(explosionEffect);
        if (deathEffect != null) library.particleEffects.Add(deathEffect);

        // Add camera shakes
        if (lightShake != null) library.cameraShakes.Add(lightShake);
        if (mediumShake != null) library.cameraShakes.Add(mediumShake);
        if (heavyShake != null) library.cameraShakes.Add(heavyShake);
        if (explosionShake != null) library.cameraShakes.Add(explosionShake);

        // Add post-processing effects
        if (lowHealthEffect != null) library.postProcessingEffects.Add(lowHealthEffect);
        if (damageFlashEffect != null) library.postProcessingEffects.Add(damageFlashEffect);
        if (victoryEffect != null) library.postProcessingEffects.Add(victoryEffect);
        if (distortionEffect != null) library.postProcessingEffects.Add(distortionEffect);

        // Save the asset
        string path = UnityEditor.AssetDatabase.GetAssetPath(this);
        path = path.Replace(".asset", "_GeneratedLibrary.asset");
        UnityEditor.AssetDatabase.CreateAsset(library, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        Debug.Log($"Created VFX Library at: {path}");
        #endif
    }
}
