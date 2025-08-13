# Entity SFX Override System

## Overview

The Entity SFX Override System allows different entity types (buildings, soldiers, etc.) to have unique sound effects for common events like taking damage, dying, and movement steps. This system provides entity-specific audio customization while maintaining a clean, data-driven architecture.

## Components

### 1. EntitySFXOverrideSO
A ScriptableObject that defines entity-specific sound keys for common events.

**Location**: `Assets/Scripts/Data/SFX/EntitySFXOverrideSO.cs`

**Fields**:
- `takeDamageKey`: Sound key for damage events
- `deathKey`: Sound key for death events  
- `stepKey`: Sound key for movement/step events

**Usage**:
1. Create a new EntitySFXOverrideSO asset: Right-click → Create → Audio → Entity SFX Override
2. Set the specific sound keys for your entity type (e.g., "building_damaged", "soldier_death", etc.)
3. Assign this asset to the `sfxOverrides` field in your CharacterDefinitionSO

### 2. SFXManager Updates
The SFXManager now supports entity-specific overrides and handles additional events.

**New Features**:
- Automatic entity-specific sound resolution
- Death event handling
- Step sound API for movement systems
- Fallback to default keys when no overrides are specified

**New Methods**:
- `PlayStepSound(Entity entity)`: Plays entity-specific step sounds
- `GetEntitySoundKey()`: Internal method for resolving entity-specific keys

### 3. EntityStepSoundTrigger (Optional)
A component that automatically triggers step sounds based on movement.

**Location**: `Assets/Scripts/Audio/EntityStepSoundTrigger.cs`

**Features**:
- Automatic step sound triggering based on movement speed
- Configurable step interval and speed threshold
- Manual trigger support for animation events
- Requires Entity component

## Setup Instructions

### Basic Setup
1. **Create EntitySFXOverrideSO assets** for different entity types:
   ```
   Right-click in Project → Create → Audio → Entity SFX Override
   ```

2. **Configure sound keys** in the override assets:
   - For a soldier: `takeDamageKey = "soldier_hit"`, `deathKey = "soldier_death"`, `stepKey = "soldier_footstep"`
   - For a building: `takeDamageKey = "building_impact"`, `deathKey = "building_collapse"`, `stepKey = ""` (empty for no steps)

3. **Assign to CharacterDefinitionSO**:
   - Open your CharacterDefinitionSO asset
   - Assign the appropriate EntitySFXOverrideSO to the `sfxOverrides` field

4. **Add sound effects to AudioLibrarySO**:
   - Make sure your AudioLibrarySO contains SoundEffect entries for all the keys you're using
   - Default keys: "unit_hit", "unit_death", "unit_step"
   - Custom keys: Whatever you specified in your EntitySFXOverrideSO assets

### Step Sound Setup (Optional)
If you want automatic step sounds for moving entities:

1. **Add EntityStepSoundTrigger component** to your entity prefabs
2. **Configure settings**:
   - `stepInterval`: Time between step sounds (default: 0.5s)
   - `minimumSpeedThreshold`: Minimum speed to trigger steps (default: 0.1)

Alternatively, you can manually call `SFXManager.Instance.PlayStepSound(entity)` from your movement or animation systems.

## Event Flow

### Damage Events
1. Entity takes damage → `GameEvents.OnUnitDamaged` fired
2. SFXManager receives event → Gets entity from damage info
3. SFXManager checks for entity SFX overrides
4. Plays entity-specific sound or falls back to default "unit_hit"

### Death Events  
1. Entity dies → `GameEvents.OnUnitDied` fired
2. SFXManager receives event → Gets entity from GameObject
3. SFXManager checks for entity SFX overrides
4. Plays entity-specific sound or falls back to default "unit_death"

### Step Events
1. EntityStepSoundTrigger detects movement OR manual call to `PlayStepSound()`
2. SFXManager checks for entity SFX overrides
3. Plays entity-specific sound or falls back to default "unit_step"

## Example Usage

### Creating a Building SFX Override
```csharp
// In the Unity Editor:
// 1. Create: Audio → Entity SFX Override
// 2. Name it "BuildingSFXOverride"
// 3. Set:
//    takeDamageKey = "building_impact"
//    deathKey = "building_collapse"
//    stepKey = "" (buildings don't walk)
```

### Creating a Soldier SFX Override
```csharp
// In the Unity Editor:
// 1. Create: Audio → Entity SFX Override  
// 2. Name it "SoldierSFXOverride"
// 3. Set:
//    takeDamageKey = "soldier_hit"
//    deathKey = "soldier_death"
//    stepKey = "metal_footstep"
```

### Manual Step Sound Triggering
```csharp
// From an animation event or movement system:
if (SFXManager.Instance != null && entity != null)
{
    SFXManager.Instance.PlayStepSound(entity);
}
```

## Benefits

1. **Entity-Specific Audio**: Different entity types can have unique sounds for the same events
2. **Fallback System**: If no override is specified, default sounds are used
3. **Data-Driven**: All configuration is done through ScriptableObjects
4. **Modular**: Step sounds are optional and can be added as needed
5. **Performance**: Minimal overhead, only checks overrides when playing sounds
6. **Maintainable**: Clear separation between default and entity-specific sounds

## Migration Notes

- Existing functionality is preserved - entities without SFX overrides will use default sounds
- No changes needed to existing CharacterDefinitionSO assets unless you want entity-specific sounds
- The system is backwards compatible with the existing SFXManager API
