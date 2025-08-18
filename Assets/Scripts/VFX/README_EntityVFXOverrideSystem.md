## Entity VFX Override System

### Overview
Provides per-entity visual effect overrides (damage, death, footsteps) similar to the existing Entity SFX Override System. Lets buildings, soldiers, etc. have unique particles / VFX.

### Components
1. EntityVFXOverrideSO (ScriptableObject)
   - takeDamageVFXKey
   - deathVFXKey
   - stepVFXKey
2. CharacterDefinitionSO
   - New field `vfxOverrides`
3. VFXManager
   - Default keys: unitDamagedVFXKey, unitDeathVFXKey, unitStepVFXKey
   - Resolves per-entity override on damage & death events before generic mappings

### Setup
1. Create asset: Create → VFX → Entity VFX Override
2. Fill keys (e.g. building_damage, building_collapse)
3. Assign to CharacterDefinitionSO.vfxOverrides
4. Ensure keys exist in VFXLibrary (particle, shake, or post-processing entries)

### Flow (Damage Example)
GameEvents.OnUnitDamaged → VFXManager.HandleUnitDamaged → Resolve entity override key → Play effect → Execute configured eventMappings.

### Extending
Add new event types by adding fields to EntityVFXOverrideSO, default keys to VFXManager, and updating GetEntityVFXKey & relevant handlers.
