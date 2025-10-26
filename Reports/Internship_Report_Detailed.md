# Project Oba Internship Report (Detailed)

## July 3, 2025 – Kickoff
- Established a persistent `InputManager` singleton that merges keyboard axes with per-frame joystick data and broadcasts normalized movement vectors through the `OnMove` event so any movement component can stay decoupled from specific input devices.【F:Assets/Scripts/Input/InputManager.cs†L4-L83】
- Delivered a touch-friendly `VirtualJoystick` that clamps drag motion to its background radius, continuously feeds normalized vectors into the input hub while pressed, and clears the handle when released to avoid ghost input on mobile.【F:Assets/Scripts/UI/VirtualJoystick.cs†L4-L80】
- Added a modular `CameraController` that follows the player with smooth damp/lerp transitions, keeps an orthographic offset, and subscribes to player speed events for adaptive zoom pacing via ScriptableObject tuning.【F:Assets/Scripts/Camera/CameraController.cs†L3-L160】

## July 5, 2025 – Mobile controls polish
- Hardened the virtual joystick’s pointer handling by recalculating drag offsets in UI space, clamping the handle travel, and writing normalized vectors so the input stream remains stable even when the finger leaves the pad edge.【F:Assets/Scripts/UI/VirtualJoystick.cs†L35-L68】
- Reset joystick intent every frame inside the input hub so stale touch vectors cannot accumulate; keyboard fallbacks only fire when mobile input is absent, preventing “stuck walk” scenarios on touch devices.【F:Assets/Scripts/Input/InputManager.cs†L36-L83】

## July 7, 2025 – Movement tuning
- Guarded all movement calls with entity state checks so dead or disabled actors immediately stop, preventing navmesh agents from drifting after defeat while still exposing world- and transform-based destinations.【F:Assets/Scripts/Movement/MovementController.cs†L63-L154】
- Centralized mover initialization to validate stats assets before runtime and surface detailed diagnostics when a definition is missing, ensuring data-driven movement stays in sync with ScriptableObjects.【F:Assets/Scripts/Movement/MovementController.cs†L35-L61】

## July 10, 2025 – Core systems sprint
- Introduced a project-wide `GameEvents` bus that raises combat, resource, progression, and speed notifications, letting loosely coupled systems (UI, audio, VFX) react without hard references.【F:Assets/Scripts/Core/GameEvents.cs†L4-L153】
- Stood up the `ResourceManager` singleton to load starting balances, gate purchases, and broadcast balance changes, reinforcing the data-driven economy loop for buildings and pickups.【F:Assets/Scripts/Core/ResourceManager.cs†L1-L120】
- Added the `Entity` hub to assemble factions, health, movement, attacking, targeting, and interaction components from a single `CharacterDefinitionSO`, yielding reliable initialization for any prefab.【F:Assets/Scripts/Entity/Entity.cs†L4-L137】【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L4-L115】
- Split battlefield decision making so `AutoInteractor` selects tactical targets, `AIMovementBrain` handles strategic marches, and `Attacker` executes profile-driven combos with cooldown tracking, giving AI units modular combat intelligence.【F:Assets/Scripts/Interaction/AutoInteractor.cs†L6-L197】【F:Assets/Scripts/AI/AIMovementBrain.cs†L4-L195】【F:Assets/Scripts/Combat/Attacker.cs†L1-L139】
- Bootstrapped overarching flow with a singleton `GameManager` and scene-bound `LevelManager`, wiring level selection, scene loading, and win/lose triggers around the new event system.【F:Assets/Scripts/Core/GameManager.cs†L7-L121】【F:Assets/Scripts/Core/LevelManager.cs†L4-L111】

## July 11, 2025 – Presentation and interaction upgrades
- Expanded character definitions with cost, animation, targeting, and override hooks so a single asset can drive prefabs, UI portraits, audio, and VFX selections per unit.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L11-L115】
- Delivered the `EntityAnimator` that listens to movement speed, attack, damage, upgrade, and unlock events to keep animation graphs synchronized with gameplay state changes.【F:Assets/Scripts/Animation/EntityAnimator.cs†L8-L198】
- Implemented upgradeable `BuildingSlot`s that expose confirmation UI, spend resources, spawn prefabs, and hook into SFX/VFX events, enabling interactive build sites in the base.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L6-L181】

## July 12, 2025 – Pipeline keep-alive
- Pushed an empty commit to keep CI automation running while larger refactors were in review, preserving build freshness for follow-up merges.【d147ea†L60-L63】

## July 14, 2025 – Combat reliability
- Added a `Continue` command and death gating across the movement controller so AI can resume paths after interruptions without reviving inactive entities.【F:Assets/Scripts/Movement/MovementController.cs†L88-L154】
- Ensured the attacker updates cooldown timers, reacquires targets, and exposes attack start/impact events, giving combatants predictable timing hooks for later systems.【F:Assets/Scripts/Combat/Attacker.cs†L40-L139】

## July 15, 2025 – Interaction architecture and camera feel
- Refined the automatic interactor to broadcast acquisition/loss events, prioritize hostile engagements, and cascade through interact/upgrade/unlock workflows based on profile flags.【F:Assets/Scripts/Interaction/AutoInteractor.cs†L122-L197】
- Hooked the camera controller into the new speed-change event so fast player motion automatically widens the zoom, while idle time recenters smoothly for cinematic pacing.【F:Assets/Scripts/Camera/CameraController.cs†L21-L160】【F:Assets/Scripts/Core/GameEvents.cs†L37-L120】

## July 17, 2025 – Health bar & animation cohesion
- Added DOTween-powered `HealthBarAnimationHandler` logic to auto-find health sources, animate fill/effect bars, shake on resource deltas, and auto-hide at full health for cleaner HUD feedback.【F:Assets/Scripts/UI/HealthBarAnimationHandler.cs†L5-L171】
- Enhanced the `EntityAnimator` with Playables-based attack blending, event subscriptions, and normalized speed sampling so locomotion, damage, and death clips stay synchronized across entities.【F:Assets/Scripts/Animation/EntityAnimator.cs†L31-L206】

## July 21, 2025 – Combat & building modularization
- Rebuilt the building slot pipeline to spend resources, animate teardown/upgrades, and attach listeners to spawned health components, supporting multistage structures and downgrades gracefully.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L123-L200】
- Added ranged attack definitions, reusable building animation clips, and tower handlers so new prefabs can leverage the shared attacker pipeline without bespoke scripts.【F:Assets/Scripts/Combat/Attacker.cs†L140-L200】
- Implemented data-driven generators, spawners, and tower controllers (e.g., `ResourceGenerator`, `ArcherTowerHandler`, spawn rate managers) to diversify base defenses and passive income loops.【F:Assets/Scripts/Buildings/ResourceGenerator.cs†L4-L78】【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】

## July 25, 2025 – Asset import
- Onboarded in-game model prefabs so character and structure definitions reference production art assets, aligning data-driven profiles with the new visual library.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L35-L105】

## July 27, 2025 – Prefab generation
- Authored environment prop prefabs and category data so the world-generation recipes can place curated scenery variations around the battlefield.【F:Assets/Scripts/Editor/World Generation/WorldGenerationRecipeSO.cs†L1-L120】

## July 28, 2025 – Environment dressing & world generation
- Built the `LineObjectPlacerWindow` editor that reads world recipes, samples Perlin-filtered placement along line segments, and groups generated props for easy undo, accelerating level dressing workflows.【F:Assets/Scripts/Editor/World Generation/LineObjectPlacerWindow.cs†L6-L170】
- Added supporting editor utilities (random transform managers, prop profiles) to define densities, variants, and distribution curves used by the placer.【F:Assets/Scripts/Editor/World Generation/PropProfile.cs†L1-L160】

## July 31, 2025 – Feedback & economy UI upgrade
- Implemented `ResourceCounter` widgets that listen for resource events, tween displayed totals, spawn delta popups, and shake UI elements with magnitude scaling to celebrate economy beats.【F:Assets/Scripts/UI/InGame/ResourceCounter.cs†L5-L155】
- Upgraded the `ResourceDropper` to split large payouts into smart denominations, randomize drop physics, and trigger collection effects, keeping resource showers performant and satisfying.【F:Assets/Scripts/Resource/ResourceDropper.cs†L5-L190】
- Added dissolve-on-death handling inside the entity pipeline so defeated units spawn VFX and SFX while scheduling cleanup via the death handler, enhancing battlefield readability.【F:Assets/Scripts/Entity/Entity.cs†L100-L137】

## August 1, 2025 – Data cleanup
- Renamed and reorganized level data ScriptableObjects so progression assets align with `GameManager` expectations, reducing confusion when loading scenes from UI selection.【F:Assets/Scripts/Core/GameManager.cs†L21-L121】
- Removed faction hardcoding from character definitions, delegating alignment to attached `Faction` components for better prefab reuse across sides.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L40-L58】

## August 4, 2025 – Building system stability
- Hardened unit spawner logic with configurable spawn-rate managers and failure diagnostics, ensuring late-game waves remain paced even under heavy load.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】
- Patched building slots to detach health listeners when upgrading and fade interaction UI, avoiding duplicate death handling during rapid rebuilds.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L140-L200】

## August 7, 2025 – Mid-run economy & defense
- Introduced `ResourceGenerator` routines that tick multiple resource channels, convert rates to per-second yields, and trigger corresponding audio/visual effects for consistent passive income feedback.【F:Assets/Scripts/Buildings/ResourceGenerator.cs†L4-L78】
- Delivered defense tower behaviours and spawn rate controllers so passive structures can scale firing rates and integrate with the central resource ledger.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】【F:Assets/Scripts/Combat/Attacker.cs†L140-L200】

## August 8, 2025 – Audio/visual unification & endgame flows
- Centralized sound playback through the singleton `SFXManager`, wiring global events to library keys and instantiating reusable audio sources for unit damage, deaths, upgrades, and victory/defeat.【F:Assets/Scripts/Audio/SFXManager.cs†L4-L186】
- Mirrored the approach for visuals with a `VFXManager` facade that routes particle, camera shake, and post-processing effects via data-driven mappings reacting to the same event bus.【F:Assets/Scripts/VFX/VFXManager.cs†L4-L180】
- Hooked endgame UI panels into victory/defeat events so the base-defense loop now transitions into polished win/lose screens with audio-visual reinforcement.【F:Assets/Scripts/Core/LevelManager.cs†L48-L111】【F:Assets/Scripts/UI/EndGame/EndGamePanelsHandler.cs†L1-L160】

## August 13, 2025 – Progressive difficulty & new content
- Added spawn-rate scalers that grow over unscaled time and reference level data slopes, enabling in-run difficulty ramps without hand-authored wave tables.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】
- Expanded tower and prestige generator data so new structures share the centralized audio/VFX overrides defined in character profiles.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L81-L115】【F:Assets/Scripts/Audio/SFXManager.cs†L184-L200】

## August 14, 2025 – Movement bug fix
- Ensured AI brains bail early when their entity state isn’t active and verify movement controllers before issuing commands, preventing null references during scene transitions.【F:Assets/Scripts/AI/AIMovementBrain.cs†L107-L138】

## August 15, 2025 – SFX system setup pass
- Finalized sound override resolution so entity-specific SFX keys fall back gracefully to defaults, letting designers swap footstep/hit audio per unit without code changes.【F:Assets/Scripts/Audio/SFXManager.cs†L184-L200】

## August 18, 2025 – Resilience & level content
- Enabled auto-revive regeneration on the `Health` component, resetting timers on damage and restoring health after configurable delays to keep long missions flowing.【F:Assets/Scripts/Combat/Health.cs†L39-L176】
- Wired Level 1 content into the `LevelManager` so destroying the enemy base or losing the hero fires victory/defeat events that bubble through UI and persistence systems.【F:Assets/Scripts/Core/LevelManager.cs†L48-L111】

## August 21, 2025 – Main menu & level select
- Built a scrollable `LevelSelectUI` that listens to `GameManager` events, centers on the active mission, and drives the Play button to load the selected level.【F:Assets/Scripts/UI/LevelSelect/LevelSelectUI.cs†L7-L135】
- Added `LevelSelectButton` behaviours to toggle lock/completion visuals, highlight the current choice, and call into the progression singleton when clicked.【F:Assets/Scripts/UI/LevelSelect/LevelSelectButton.cs†L5-L58】
- Provided main-menu affordances like `LoadScene0Button` to reset time scale and return to the menu scene, completing the front-door navigation loop.【F:Assets/Scripts/UI/LoadScene0Button.cs†L4-L29】
