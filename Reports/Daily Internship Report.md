The aim of my project is to develop a mobile game. I am the sole developer, and I receive regular guidance from my mentors. The technology stack is Unity and C#.

From a software engineering perspective, my goal is to maintain a clean, scalable architecture that remains maintainable as the project grows, while still delivering a complete, working product.

### 30 June 2025

At the beginning of the project, a persistent singleton InputManager was designed to unify movement input from multiple sources. It combines keyboard axis input with per-frame joystick vectors and publishes a normalized movement vector through an OnMove event. As a result, movement components remain independent of the input device, supporting keyboard/gamepad during development and a virtual joystick on mobile.

A touch-friendly VirtualJoystick was implemented for mobile control. It constrains the drag vector within a defined background radius, continuously feeds normalized vectors to the input center while pressed, and resets the handle on release to prevent unintended “ghost” input. The VirtualJoystick and InputManager are loosely coupled and communicate via event notifications rather than direct dependencies.

A modular CameraController was also added to follow the player using smooth damp/lerp transitions while preserving an orthographic offset. Camera tuning parameters are stored in ScriptableObjects to support a data-driven workflow, and the controller can subscribe to speed-related events to adjust zoom-in/zoom-out pacing. In its basic configuration, the camera is an isometric orthographic camera.

### 3 July 2025

Polishing mobile controls

The virtual joystick’s pointer-processing flow was hardened by recalculating drag offsets in UI space, constraining handle movement, and continuously writing normalized direction vectors to keep the input stream stable even when the finger moves near or outside the control area.

To prevent touch vectors from accumulating across frames, joystick intent at the input center is reset every frame. When no mobile input is present, only keyboard fallback input is used, which helps avoid “stuck movement” scenarios on touch devices.

### 4 July 2025

Movement tuning

To improve overall movement reliability, all movement calls were guarded with entity state checks. If an actor is dead or disabled, movement is stopped immediately. This prevents unwanted post-defeat behavior such as NavMesh agents continuing to slide or drift, while still supporting both world-space and transform-based target types where appropriate.

Movement initiation was also centralized to reduce duplicated logic and eliminate inconsistent state handling across components. In addition, stat and configuration assets (ScriptableObjects) are validated before runtime use. When a required definition is missing, the system provides explicit diagnostic information rather than failing silently. This helps keep data-driven movement definitions aligned with code expectations and reduces integration errors as the number of entities and movement profiles grows.

During the core-systems sprint, a large refactor was introduced in a single development wave. This change set reflects an earlier, extended planning and diagramming phase where the complete “big picture” of the project was mapped out, including class responsibilities, system boundaries, field/function roles, and the interactions between subsystems. A particular focus was placed on event-triggered workflows and on building a foundation that supports extension without tight coupling. The architecture overview diagram is included as a supporting visual (BigPictureOverview.jpg).

At a high level, the project architecture aims to keep subsystems independent from each other. Communication is handled using an event-driven approach, which allows UI, gameplay logic, and feedback systems to react to state changes without hard references. Configuration is primarily data-driven via ScriptableObjects, enabling new objects and behaviors to be defined through data assets with minimal or no code changes, depending on the feature.

Two core subsystems introduced in this phase are the health system and the movement system. The health system is responsible for health values and health-related rules, including health statistics and the main operations that modify or query health state. The movement system defines what it means for an entity to be movable via the IMoveable interface and separates movement execution into specialized controllers based on actor type. For example, the player can be driven by joystick input, while NPCs are driven by AI decisions; therefore, the system uses two movement controllers that both conform to IMoveable but consume different input sources. Movement parameters and profiles are stored as ScriptableObjects so that movement behavior can be tuned and reused consistently across different entities.

### 7 July 2025

Core systems sprint

In this phase, a project-wide GameEvents event bus was introduced to serve as the primary notification layer for cross-cutting gameplay changes. It publishes signals related to combat, resources, progression, and speed/state changes, enabling loosely coupled systems (such as UI, audio, and VFX) to respond without requiring hard references to gameplay objects. In practice, GameEvents functions as a central “broadcast tower” for announcements that affect the overall game state, helping keep dependencies directional and reducing the need for direct component-to-component communication.

In addition, a singleton ResourceManager was implemented as the central authority for the in-game economy. It loads initial balances, validates and conditions purchase requests (e.g., checks whether a transaction is affordable and applies the cost), and broadcasts balance changes so that other systems can react consistently. By maintaining a unified ledger and supporting multiple resource types under a single interface, the ResourceManager acts as a “bank” layer for the project and strengthens a data-driven economy loop used by buildings, drops, and other reward sources.

### 10 July 2025

Core systems sprint

To make entity initialization consistent and scalable, an Entity center was introduced. Each gameplay prefab is initialized through a single CharacterDefinitionSO that acts as the entity’s “identity” and source of truth. This definition aggregates core aspects such as faction/alignment, health configuration, movement configuration, attack capabilities, targeting rules, and interaction-related components. By relying on a unified definition object, the project avoids scattered setup logic and ensures that prefabs start in a reliable, repeatable way.

This CharacterDefinitionSO-based “character identity” approach is used to define and spawn characters both in the editor and at runtime. In practical terms, it functions as an ID card that contains (or references) the character’s prefab, team/faction data, movement profile, health statistics, and attack profile, so that a character can be created and configured without manually wiring a large number of separate fields each time.

Field decision-making was also decomposed into dedicated modules to give AI-driven units a more maintainable and extensible combat behavior. In this design, AutoInteractor is responsible for tactical selection of nearby or immediate objectives, while AIMovementBrain manages strategic movement decisions (i.e., where the agent should go next). Combat execution is handled by the Attacker component, which tracks cooldowns and triggers profile-driven attack sequences/combos, resulting in a more predictable and configurable combat loop.

To support multiple interaction categories in a type-safe way, interaction capabilities are expressed through interfaces such as IUpgradable, IDestructible, ICollectible, IHealable, IInteractable, and IUnlockable. This allows the AI and other systems to reason about what an object can do (or what can be done to it) without requiring knowledge of the object’s concrete class.

In the combat pipeline, the attacking system encapsulates knowledge about attack types and attack “identity” assets and executes the correct behavior when an attack command is issued. Target acquisition is supported by TargetScanner components attached to characters; these act as the “eyes” of an entity by scanning the environment according to the entity’s capabilities (defined in a data-driven way) and maintaining a candidate list of detected targets.

An AIController orchestrates NPC behavior so that units can progress toward a primary objective while still reacting to opportunities or threats encountered along the path. For example, when an NPC encounters a closer, higher-priority interaction target, it can temporarily engage with that target and then continue toward its main destination. Additionally, because interaction and combat decisions are largely AI-driven, the player character also uses an Interactor component to automate non-movement actions such as attacking nearby enemies or triggering interactions when in range.

Finally, the high-level application flow was structured around a singleton GameManager and a scene-specific LevelManager. Together, these managers coordinate level selection, scene loading, and win/lose trigger conditions, and they integrate these transitions with the event system introduced earlier. This establishes a clear entry point for game state changes and reduces fragmentation of level-flow logic across unrelated components.

### 11 July 2025

Presentation and interaction improvements

The character definition layer was expanded to support richer presentation and interaction requirements in a consistent, data-driven way. Character definitions were extended with additional fields and hooks such as unit cost, animation configuration, targeting configuration, and override points. With these additions, a single entity definition can manage per-unit prefab selection as well as presentation assets such as UI portraits, audio selections, and VFX selections. This increases the amount of gameplay and presentation behavior that can be configured through data assets rather than hard-coded values, which improves scalability as the number of units and variations grows.

To keep animation state aligned with gameplay state changes, an EntityAnimator component was introduced. It listens to runtime signals such as movement speed updates and key gameplay events (attack triggers, damage events, upgrade events, and unlock events) and uses these notifications to drive animation transitions. This event-driven approach ensures that the animation graph remains synchronized with the entity’s current state without requiring frequent direct calls from unrelated systems.

In addition, the animation workflow was structured around a reusable base Animator Controller that defines the shared state machine for entities. Per-entity differences are provided via overridden animation clips (i.e., swapping only the clips that differ while keeping the same controller structure). This allows multiple entities to share the same animation logic while still supporting unique visuals and timing per unit.

### 14 July 2025

Presentation and interaction improvements. Upgradeable BuildingSlot components were implemented to enable interactive construction/upgrade points in the base. Each BuildingSlot provides a confirmation UI, spends the required resources, and instantiates the resulting building prefab. The BuildingSlot workflow is integrated with the project’s event-driven feedback layer by emitting or subscribing to SFX/VFX events, so that upgrades are accompanied by consistent audio-visual feedback. In effect, this establishes a resource-driven building-upgrade feature with a clear player-facing interaction flow (confirm → pay cost → spawn/upgrade → play feedback).

Combat reliability improvements. To make AI movement more robust across interruptions and state changes, a Continue command and explicit “death gates” were added along the movement-controller path. With these safeguards, AI agents can resume their navigation after transient interruptions, but they do not continue acting when they are inactive or dead unless they are explicitly revived or re-enabled. This prevents undesirable edge cases where defeated entities keep moving or behaving as if they were still alive (i.e., “zombie” behavior).

In addition, the Attacker component was refined to improve timing predictability during combat. It now updates cooldown timers consistently, re-acquires targets when necessary, and exposes clear attack start/impact events. These explicit timing hooks make it easier for dependent systems (such as animation, VFX, and SFX) to synchronize with combat actions and provide more reliable moment-to-moment feedback.

### 17 July 2025

Interaction architecture and camera feel

The AutoInteractor module was refined to make interaction behavior more deterministic and easier to extend. It was updated to publish explicit acquisition/loss events (e.g., when an interactable target enters or leaves the active set), which improves observability and allows dependent systems to react in a loosely coupled manner. The selection logic was also adjusted to prioritize hostile interactions when appropriate, so that survival-critical actions (such as engaging an enemy) are handled before lower-priority interactions.

In addition, the interaction workflow was structured to progress in stages depending on profile flags, enabling a controlled sequence for actions such as interacting, upgrading, or unlocking. Instead of treating all interactions as equivalent, profile-driven rules determine which interaction path is valid and what the next step should be, reducing ambiguity during runtime decision-making. As part of this refinement, multiple edge cases and bugs were addressed to prevent inconsistent states and to ensure that interaction transitions remain stable.

The CameraController was also integrated with a new speed-change event to improve the overall “feel” of the camera. When player movement becomes faster, the camera can automatically widen its zoom to increase situational awareness; when the player is idle for a period, the camera smoothly recenters at a cinematic pace. This event-driven camera behavior results in a smoother, more dynamic, and more consistent camera response compared to a purely fixed-follow implementation.

### 18 July 2025

Health bar and animation consistency

A DOTween-based HealthBarAnimationHandler was introduced to improve HUD readability and to keep health feedback consistent across different entity types. The handler automatically locates the relevant health data sources, animates both the fill bar and the delayed/impact bar, applies a “shake” response on health changes, and automatically hides the health bar when the entity is at full health. This results in a cleaner UI presentation that surfaces damage and recovery events clearly without permanently occupying screen space.

The HealthBarAnimationHandler was designed to be reusable for all characters rather than tailored to a single prefab. It listens to health-related event notifications instead of relying on hard references, and it accounts for common edge cases such as interrupted tweens, rapid consecutive damage/heal updates, and state transitions where the health source may be temporarily unavailable. Handling these cases explicitly prevents visual desynchronization between the displayed bar state and the underlying health value.

EntityAnimator was further developed to keep animation playback aligned with gameplay changes across locomotion and combat. The implementation uses Playables-based attack blending to combine attack motions with movement when appropriate, and it relies on event subscriptions to react to gameplay triggers rather than polling. Normalized speed sampling is used so that locomotion, hit reactions, and death clips remain consistent across different entities even when their underlying movement speeds or animation clip lengths differ.

Combat and building modularization. The building-slot pipeline was rebuilt to support a more reliable multi-stage building lifecycle. The system spends resources as part of the build/upgrade flow, plays destruction and upgrade animations, and attaches listeners to newly spawned health components so that downstream systems (UI, VFX, SFX, and game logic) remain correctly wired after a build state change. The rebuilt workflow also supports multi-tier structures and clean downgrades, enabling a building to transition to a previous stage without breaking references or leaving stale listeners behind.

### 21 July 2025

Combat and building modularization

To improve extensibility, ranged attack definitions were added alongside reusable building animation clips and dedicated tower handlers. The goal of this change was to allow new prefabs (especially buildings and towers) to reuse the shared attacker pipeline rather than requiring custom scripts for each new unit type. By treating attacks as configurable definitions and by reusing standardized animation assets, new content can be integrated with a predictable setup process and consistent feedback behavior.

In parallel, a set of data-driven producers, spawners, and tower controllers was implemented to broaden base-defense behavior and the passive economy loop. Examples include ResourceGenerator for periodic resource production, ArcherTowerHandler for ranged defensive behavior, and configurable spawn-rate managers that control unit production tempo. These components are designed to be configured through profiles (e.g., ScriptableObjects), so that the same code can support multiple building variants by changing parameters such as production rate, attack cadence, or spawn composition.

As a practical outcome, additional building types were introduced to cover common base-defense and economy needs. This included wall-type buildings that provide structure without extra mechanics, defensive towers that engage enemies automatically, resource mines that provide passive income, and unit producer buildings that spawn soldiers. Because these buildings are built on the same shared pipelines (attack execution, animation playback, and data-driven configuration), they can be extended with new variants with minimal engineering overhead.

### 24 July 2025

Asset import

In-game model prefabs were imported into the project, and both character and building definitions were updated to reference production art assets directly. This ensured that the existing data-driven profiles remained consistent with the newly added visual library and that gameplay definitions (e.g., character/building identity assets) could select the correct production-ready prefabs.

For the majority of models, an AI-assisted 3D content workflow was used. The pipeline was as follows: first, concept images were generated with ChatGPT, including multiple viewpoints of the same asset. These images were then converted into a 3D model using Hunyuan image-to-3D, and textures were produced using StableProjectorz. For character models that required a skeleton, Mixamo was used for automatic rigging. After generation, the resulting models were imported into Unity, added to the project as prefabs, and their scale/size was adjusted to match the in-game environment. At this stage, not all final models were produced; this section documents the workflow and integration approach.

Prefab production

Environment prop prefabs were created and categorized, and accompanying category/configuration data was authored so that world-generation recipes could place selected decoration variations around the combat area. This enables controlled variation (different prop sets and densities) while keeping placement rules and asset selection data-driven.

Environment dressing and world generation

To speed up level dressing, a custom editor tool named LineObjectPlacerWindow was developed. The tool reads a placement “recipe”, samples placement points along line segments, and applies a Perlin-based filtering approach to achieve natural-looking distribution rather than uniform spacing. Placed props are grouped to support fast rollback and cleanup when regenerating the environment.

The tool is intended to reduce manual workload for developers by generating a level layout using rule-based randomness. Parameters such as how far from the main path props may appear, the probability of spawning at different distances, and the overall placement amount are defined through numeric parameters and curves. This supports rapid iteration and serial level generation while keeping the output consistent with designer-defined constraints.

### 25 July 2025

Environment dressing and world generation

To support environment dressing and procedural world generation, I added supporting editor tooling for the placement pipeline. These tools make it easier to define and iterate on the same inputs used by the placer, such as density controls, prop variants, and distribution curves. In practice, this includes utilities like randomized transform managers (to produce controlled variation in rotation/scale) and prop profile assets that encapsulate spawn rules and variant sets. With these helpers, placement “recipes” can be tuned in a structured way, reducing manual scene tweaking and improving repeatability across generation runs.

In parallel, the feedback layer for the economy UI was upgraded to make resource changes clearer and more satisfying for the player, without relying on intrusive effects. I implemented ResourceCounter widgets that subscribe to resource/balance events and update the displayed totals using tweens rather than abrupt number jumps. When balances change, the widgets can also spawn delta popups (positive or negative) and apply short, controlled scale pulses (and optional shake) to emphasize the change. The result is a robust, event-driven UI feedback system that remains loosely coupled: the widgets do not need direct references to gameplay systems, and they react purely by listening to published events, which helps keep the UI maintainable as the economy and resource types expand.

### 28 July 2025

Feedback and economy UI upgrade

The resource drop pipeline was upgraded by improving the ResourceDropper component to better support large payouts while keeping the moment-to-moment feedback performant and readable. Instead of spawning a single oversized drop (or producing an excessive number of tiny items), large rewards are split into meaningful “coupons” (chunks) that represent the total amount more clearly. The drop behavior also randomizes the physical spawn parameters (such as spread and impulse) to avoid repetitive patterns and to create a more natural “shower” effect, while still remaining deterministic enough to tune. In addition, the dropper triggers appropriate collection feedback effects, so that the reward loop communicates value both visually and through interaction, without causing unnecessary overhead.

For combat readability and cleanup, a dissolve-style death presentation was introduced for defeated entities. When an entity is eliminated, the death pipeline now coordinates VFX and SFX emission while also ensuring that cleanup is planned and executed through a dedicated death handler rather than being scattered across unrelated systems. Visually, units appear to fade out and disappear instead of remaining as persistent clutter on the battlefield. This reduces on-screen noise during dense fights and can also reduce rendering cost by removing dead objects and their materials/effects from the scene in a controlled manner.

Finally, a data-cleanup pass was performed to remove ambiguity and reduce friction during level loading and progression management. Level-related ScriptableObject assets were renamed and reorganized to better reflect their responsibilities and to make it easier to locate and maintain the correct data definitions. Progression assets were also aligned with the expectations of the GameManager, which reduces mismatches between UI selection and runtime scene loading. As a result, selecting a level through the UI leads to clearer, more consistent behavior, and the overall project structure remains easier to scale as additional levels and progression rules are added.

### 31 July 2025

Data cleanup

As part of a data cleanup pass, the responsibility for faction/alignment was removed from character definition assets. Previously, faction was “fixed” inside the CharacterDefinition layer, which reduced prefab reuse because the same unit concept might need to exist as both an ally and an enemy. In that design, producing two separate but otherwise identical definition assets (differing only by team) became a recurring maintenance cost and increased the chance of configuration drift.

To resolve this, alignment was delegated to dedicated Faction components attached to prefabs at runtime (or at authoring time on the prefab), instead of being baked into the definition asset itself. With this change, a single character definition can describe the unit’s capabilities, visuals, and gameplay parameters, while the faction component determines “who it belongs to” in a clean and explicit way. This improves prefab and data reuse, keeps team logic closer to the entity instance, and reduces duplicated data assets.

In the same cleanup phase, building-system stability was also improved by strengthening the unit spawning pipeline. The producer logic was hardened with configurable spawn-rate controllers and more explicit diagnostics to handle heavy late-game load more reliably. By treating spawn tempo as a tunable system (rather than a set of scattered per-building rules) and by adding clearer error reporting, the project can maintain consistent wave pacing while still allowing difficulty to be adjusted through data and configuration.

### 1 August 2025

Building system stability

To improve building-system stability, the BuildingSlot upgrade workflow was patched to prevent incorrect health/event behavior during rapid rebuild or upgrade cycles. In the previous implementation, upgrading a building inside a slot could unintentionally trigger the building’s death logic, which produced incorrect outcomes (for example, premature destruction flow, duplicated cleanup, or inconsistent state). The slot now detaches or rebinds health listeners appropriately during the upgrade transition and fades the interaction UI while the operation is in progress, so that repeated upgrades do not accumulate duplicate listeners or repeated “death” processing.

In addition, mid-run economy and defense behavior was strengthened by introducing ResourceGenerator routines. These routines tick one or more resource channels on a schedule, convert configured rates into per-second yields, and emit the required audio/visual feedback so that passive income changes remain visible to the player. The generator identity was designed in a data-driven way so that a single generator definition can produce one resource type or multiple resource types, depending on configuration, which supports scalable balancing and content variation.

Finally, defensive tower behaviors and spawn-rate control were made more consistent across passive buildings by ensuring that attack cadence and similar time-based behaviors can be scaled and tuned via profiles. Defensive structures and unit producer buildings were also integrated with the central resource ledger, ensuring that costs, income, and upgrades remain coherent under the same economy rules. This makes passive structures behave predictably during longer sessions and reduces the risk of economy-related edge cases as the base-defense loop becomes more complex.

### 4 August 2025

Audio/visual consistency and end-of-game flows

To improve overall consistency and maintainability, audio playback was centralized through a dedicated singleton, SFXManager. Instead of triggering audio directly from many gameplay scripts, global and gameplay events are mapped to library keys, and the manager resolves and plays the appropriate sound assets. This structure makes sound effects reusable across common situations such as unit damage, deaths, upgrades, and win/lose outcomes, while keeping dependencies directional: gameplay systems publish events, and the audio layer reacts. As a result, adding or changing sounds becomes primarily a configuration task, and the project avoids duplicated audio logic scattered across unrelated components.

The same architectural approach was applied to visual feedback through a VFXManager facade. Particle effects, camera shake, and post-processing responses are routed via data-driven mappings that listen to the same event bus used by other cross-cutting systems. This keeps the feedback layer aligned with the event-driven architecture and ensures that audio and visual responses remain synchronized without introducing hard references between gameplay logic and presentation. In practice, it enables consistent, scalable feedback for both combat and progression events, while also making it easier to extend the library of effects as the game’s content grows.

### 7 August 2025

Audio/visual consistency and end-of-game flows

To complete the end-of-game flow, the win/lose UI panels were connected directly to victory and defeat events. This allows the base-defense gameplay loop to transition into dedicated outcome screens as a first-class part of the event-driven architecture, rather than relying on ad-hoc checks or scene-specific references. By driving the end state through events, the same victory/defeat signal can also coordinate presentation elements (such as SFX/VFX reinforcement and UI animation) in a consistent way, which results in a cleaner and more polished finish to each run.

In the same phase, I introduced a scalable difficulty approach together with support for expanding game content. Instead of authoring and maintaining hand-crafted wave tables for each level, enemy production tempo is controlled by spawn-rate scalers that increase over unscaled time and reference level data curves for configuration. This makes difficulty tuning a data-driven process: each level can define its own scaling profile, and the runtime can apply those curves to grow challenge gradually.

This change was motivated by the core design observation that the primary driver of difficulty in this game is the enemy’s unit production rate. Therefore, the scaling system focuses on adjusting spawn cadence (and related production parameters) in a controlled, explainable manner. As a result, difficulty can ramp predictably without requiring frequent manual edits to wave schedules, and new levels can be created more efficiently by adjusting level-specific scaling data rather than rewriting large sets of per-wave definitions.

### 8 August 2025

Gradual difficulty and new content

To support gradual difficulty progression and to make it easier to add new content, tower and prestige-producer data definitions were expanded. With this update, newly introduced structures can share centralized SFX/VFX override settings that are defined at the profile level (i.e., in character/building identity assets). In other words, instead of duplicating presentation configuration across many prefabs, a group of related units or buildings can reference common audio/visual mappings through their identity cards. This improves consistency across similar content, reduces repeated configuration work, and makes it safer to iterate on feedback style because changes can be applied at the shared profile level.

In parallel, a movement-related bug was fixed to improve stability during runtime state transitions. The AIMovementBrain module was updated to exit early when the owning entity is inactive (for example, disabled, dead, or otherwise not in a valid simulation state) and to validate movement controllers before issuing movement commands. This prevents commands from being executed against missing or invalid movement components and reduces the likelihood of null-reference errors during scene transitions or initialization/teardown windows. As a result, AI-driven movement becomes more robust in edge cases where game objects are being enabled/disabled while navigation decisions are still being evaluated.

### 11 August 2025

SFX system migration

As part of the sound-effects pipeline cleanup, unit-specific SFX keys were finalized in a way that gracefully falls back to defaults when an override is missing or intentionally not provided. This allows designers to customize per-unit sounds (such as footsteps and impact/hit sounds) without requiring code changes, while still ensuring that the game always has a valid sound to play. In practice, this reduces content-integration friction: adding a new unit no longer forces a complete, perfectly authored SFX key set on day one, because the system can safely resolve to shared default mappings.

In the same phase, durability and level-flow stability were improved by enabling automatic revive-style regeneration behavior in the Health component. After an entity takes damage, regeneration timers are reset to avoid “instant heal” edge cases, and health is restored only after a configurable delay and according to configurable parameters. This supports longer play sessions by reducing hard-stall failure states, while keeping the behavior deterministic and tunable through data rather than scattered ad-hoc healing rules. Overall, the change ensures that health can recover in a controlled way, improving the pacing of extended runs and making survivability balancing more systematic.

### 14 August 2025

Durability and level content

Level 1 content was integrated with the LevelManager so that the level flow is driven by explicit win/lose conditions rather than scattered checks. In this setup, destroying the enemy base triggers a victory event, while losing the hero triggers a defeat event. These outcome signals are then propagated to higher-level layers (such as the UI and the persistence/progression systems) through the existing event-driven architecture, ensuring that end-of-level feedback and state saving are coordinated consistently.

In addition, the main menu and level selection flow was connected through a scrollable LevelSelectUI. This UI listens to GameManager-level events to stay synchronized with the current game state, keeps the user interface aligned with the active objective/selection, and drives the “Play” action to load the currently selected level. As a result, level selection is handled in a centralized and observable way: the UI reflects selection changes deterministically, and scene loading is initiated through a clear, single entry point based on the selected level.

### 15 August 2025

Main menu and level selection

The level selection interface was strengthened by introducing dedicated `LevelSelectButton` behaviors. Each button now reflects the level’s progression state in a user-facing and deterministic way by switching its visuals between locked and completed states when appropriate. The same component also highlights the currently selected level so that the player can clearly see which level will be launched. On user interaction, the button communicates with the central progression singleton to resolve whether the level can be selected and to persist the selection consistently, instead of relying on scene-local variables.

For the main menu flow, a set of small but important usability and safety mechanisms was added to ensure reliable navigation and consistent game-state reset when returning to the menu. In particular, a `LoadScene0Button` routine was introduced to reset the time scale before transitioning back to the menu scene, which prevents unintended slow-motion or paused-state carryover from gameplay sessions. In addition, the “front door” navigation loop was completed so that moving from menu → selection → gameplay → menu follows a stable, repeatable path without leaving the application in a partially initialized state.

Finally, to support a presentable demo build, five levels were authored with gradually increasing difficulty. These levels were completed end-to-end and prepared for loading through the same selection pipeline, so that the demo experience can showcase progression in a controlled manner. Having multiple ready-to-load levels also serves as a practical validation of the level-management and UI-selection integration, because it exercises the selection state, lock/completion visuals, and scene-loading transitions across several distinct configurations.

### Discussions and Conclusion

During review sessions, my mentors highlighted that the AI-generated 3D models currently used in the project contain significantly more polygons (and therefore many more faces) than would normally be acceptable for a typical mobile-game performance target. They emphasized that this may negatively affect runtime performance (GPU load, memory bandwidth, and battery consumption), especially in dense combat scenarios where multiple characters and structures can be visible at the same time. As a corrective action, they recommended migrating to more optimized assets by reducing polygon counts, using mobile-friendly LODs, and preferring production-ready models that are authored and exported with mobile constraints in mind.

In addition to the asset-optimization topic, the mentors stated that the current build requires further stabilization work and a dedicated polish pass. This feedback focused on addressing remaining bugs, improving reliability in edge cases, and ensuring that the overall gameplay loop feels complete rather than purely functional. Concretely, this means prioritizing bug fixing, tightening interaction flows, removing rough transitions, and validating that UI, camera, audio/visual feedback, and level-flow logic behave consistently across repeated sessions.

Finally, they suggested using established tower-defense references such as Kingdom Rush as a benchmark for overall structure and player experience. The intent of this guidance was not to copy a specific game, but to adopt proven patterns for progression, readability, pacing, and feedback loops (e.g., clarity of upgrades, satisfying reward cadence, and clear communication of threats and objectives). Using such references helps evaluate whether the current design decisions and implementations translate into a cohesive and engaging mobile gameplay experience.

Even though the scope of the project was ambitious for a single-developer internship, focusing on scalable and modular systems improved my ability to build features without accumulating severe technical debt. I believe the project architecture remained relatively independent and extensible, meaning that adding new content does not immediately turn the codebase into “spaghetti” or a fragile chain of dependencies. At the same time, this phase made it clear that technical correctness alone is not sufficient to transform a software system into a finished game product: performance constraints, content quality, bug fixing, and overall polish are equally important to deliver a stable and enjoyable mobile-game experience.