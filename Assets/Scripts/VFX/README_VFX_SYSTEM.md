# VFX System - Data-Driven Visual Effects

## Overview

The VFX System provides a comprehensive, data-driven approach to managing visual effects in Unity. It supports three types of VFX:

1. **Particle Effects** - GameObject-based particle systems with automatic lifecycle management
2. **Post-Processing Effects** - Volume-based effects with timing and blending control
3. **Camera Shakes** - Procedural camera movement with customizable characteristics

## Key Components

### Core Data Structures

- `VFXDataBase` - Abstract base class for all VFX data
- `ParticleEffectData` - Configuration for particle effects
- `PostProcessingEffectData` - Configuration for post-processing effects
- `CameraShakeData` - Configuration for camera shake effects

### Management Classes

- `VFXLibrary` - Central ScriptableObject containing all VFX configurations
- `VFXManager` - Main facade manager for triggering VFX
- `ParticleEffectManager` - Specialized particle effect management
- `PostProcessingEffectManager` - Specialized post-processing management
- `CameraShakeManager` - Specialized camera shake management

### Utility Classes

- `AutoDestroyParticleEffect` - Component for automatic particle cleanup
- `VFXPresetsCollection` - Predefined VFX configurations for common use cases

## Setup Guide

### 1. Create a VFX Library

```csharp
// Create via menu: Assets > Create > VFX > VFX Library
VFXLibrary library = CreateInstance<VFXLibrary>();
```

### 2. Configure VFX Data

#### Particle Effects
```csharp
var hitEffect = new ParticleEffectData
{
    key = "basic_hit",
    displayName = "Basic Hit Effect",
    particlePrefab = yourParticlePrefab,
    autoDestroy = true,
    maxLifetime = 5f,
    scale = 1f
};
```

#### Camera Shakes
```csharp
var mediumShake = new CameraShakeData
{
    key = "medium_shake",
    magnitude = 1f,
    duration = 0.4f,
    frequency = 30f,
    shakePosition = true,
    xMultiplier = 1f,
    yMultiplier = 1f,
    zMultiplier = 0.7f
};
```

#### Post-Processing Effects
```csharp
var damageFlash = new PostProcessingEffectData
{
    key = "damage_flash",
    postProcessingProfile = yourVolumeProfile,
    fadeInDuration = 0.1f,
    mainDuration = 0.2f,
    fadeOutDuration = 0.3f,
    maxWeight = 1f
};
```

### 3. Setup Managers

1. Create a GameObject with `VFXManager`
2. Assign the VFX Library
3. Add and configure specialist managers:
   - `ParticleEffectManager`
   - `CameraShakeManager` 
   - `PostProcessingEffectManager`

### 4. Configure Event Mappings

```csharp
// In VFXManager inspector, setup event mappings
var damageMapping = new VFXEventMapping
{
    eventType = VFXEventType.UnitDamaged,
    damageThreshold = 10f,
    particleEffectKeys = new[] { "basic_hit" },
    cameraShakeConfigs = new[] 
    { 
        new CameraShakeConfig 
        { 
            key = "light_shake",
            useMagnitudeMultiplier = true,
            damageThreshold = 50f
        }
    },
    postProcessingEffectKeys = new[] { "damage_flash" }
};
```

## Usage Examples

### Basic VFX Triggering

```csharp
// Trigger individual effects
VFXManager.Instance.PlayParticleEffect("explosion", transform.position);
VFXManager.Instance.TriggerCameraShake("heavy_shake");
VFXManager.Instance.PlayPostProcessingEffect("low_health");

// Trigger multiple effects at once
string[] effects = { "hit_particle", "damage_flash", "light_shake" };
VFXManager.Instance.PlayMultipleEffects(effects, hitPosition);
```

### Advanced Usage

```csharp
// Particle effect with custom scale
VFXManager.Instance.particleManager.PlayParticleWithScale(
    "explosion", 
    transform.position, 
    Quaternion.identity, 
    2.5f // Scale multiplier
);

// Camera shake with magnitude override
VFXManager.Instance.TriggerCameraShakeWithMagnitude("explosion_shake", 1.5f);

// Check effect status
if (VFXManager.Instance.postProcessingManager.IsEffectActive("low_health"))
{
    Debug.Log("Low health effect is currently active");
}
```

### Event-Based Triggering

The system automatically responds to game events when properly configured:

```csharp
// This will automatically trigger configured VFX
GameEvents.OnUnitDamaged?.Invoke(new DamageInfo
{
    attacker = attackerObject,
    victim = victimObject,
    damageAmount = 75f
});
```

## Best Practices

### 1. Particle Effect Prefabs

- Add `AutoDestroyParticleEffect` component to particle prefabs
- Set appropriate `maxLifetime` values as safety fallbacks
- Use `Play On Awake` for immediate effects

### 2. VFX Library Organization

- Use clear, consistent naming conventions for keys
- Group related effects logically
- Validate the library regularly using the editor tools

### 3. Performance Considerations

- Limit concurrent post-processing effects
- Use object pooling for frequently spawned particle effects
- Set reasonable `maxLifetime` values to prevent memory leaks

### 4. Camera Shake Design

- Use different frequencies for different effect types
- Apply appropriate dampening curves
- Consider directional multipliers for varied feel

## Migration from Legacy System

### Old Approach
```csharp
// Hardcoded in VFXManager
if (info.damageAmount > heavyShakeThreshold)
    cameraShakeManager.Shake(ShakeIntensity.Heavy);
else
    cameraShakeManager.Shake(ShakeIntensity.Light);
```

### New Approach
```csharp
// Data-driven configuration in VFXLibrary
// Automatic triggering via event mappings
// Or manual triggering:
string shakeKey = damageAmount > 50f ? "heavy_shake" : "light_shake";
VFXManager.Instance.TriggerCameraShake(shakeKey);
```

## Editor Tools

- **VFX Library Editor**: Enhanced inspector for `VFXLibrary`
- **Validation Tools**: Built-in validation for all VFX configurations
- **Preset Collections**: Pre-configured common VFX setups
- **Key Visualization**: View all available VFX keys organized by type

## Troubleshooting

### Common Issues

1. **VFX not triggering**: Check key spelling and library initialization
2. **Particle effects not destroying**: Verify `AutoDestroyParticleEffect` component
3. **Post-processing not visible**: Ensure Volume component priority and profile assignment
4. **Camera shake not working**: Verify camera transform assignment

### Debug Methods

```csharp
// Check system status
var status = VFXManager.Instance.GetSystemStatus();
Debug.Log($"Active shakes: {status.activeCameraShakes}");

// Validate library
if (vfxLibrary.ValidateLibrary())
    Debug.Log("Library is valid");

// View all available keys
var allKeys = vfxLibrary.GetAllVFXKeys();
```

## Performance Tips

1. **Batch VFX calls** when possible using `PlayMultipleEffects()`
2. **Use event mappings** instead of manual triggering for better performance
3. **Pool particle objects** for frequently used effects
4. **Limit concurrent post-processing effects** to maintain frame rate
5. **Use appropriate culling** for particle systems

## Future Enhancements

- Audio integration for synchronized audio-visual effects
- Timeline support for cinematic sequences
- Network synchronization for multiplayer effects
- Advanced pooling system for particle effects
- Conditional VFX based on graphics settings
