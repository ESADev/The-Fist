\# AGENTS.md - Project Oba Development Guide



\## 1. About This Project



\*\*Project Name:\*\* Project Oba

\*\*Concept:\*\* A top-down, rogue-like base-building and defense game for mobile. The player manages a nomadic tribe, defending their camp (Oba) from rival attacks, building up their forces, and ultimately launching a climactic assault to defeat the enemy and progress.

\*\*Core Pillars:\*\*

\* \*\*Strategic Choices:\*\* Meaningful decisions before and during gameplay.

\* \*\*Modular Design:\*\* A highly flexible and scalable codebase.

\* \*\*Satisfying Progression:\*\* A clear sense of growing power both within a level and across the entire game.



---



\## 2. Codebase Structure



The project follows a standard Unity folder structure. Key directories for development are:



\* `Assets/Scripts/`: Contains all C# source code.

&nbsp;   \* `Core/`: For foundational, game-wide systems like `GameEvents.cs`, `ResourceManager.cs`.

&nbsp;   \* `Gameplay/`: For components directly related to the main game loop (`Health.cs`, `Attacker.cs`, `AIController.cs`, etc.).

&nbsp;   \* `Data/`: For all C# files defining `ScriptableObject` classes and `enum`s.

&nbsp;   \* `UI/`: For scripts related to user interface elements.

\* `Assets/Data/`: Contains all `ScriptableObject` assets created from the classes in `Assets/Scripts/Data/`.

&nbsp;   \* `CharacterDefinitions/`

&nbsp;   \* `AttackDefinitions/`

&nbsp;   \* `LevelData/`

\* `Assets/Prefabs/`: Contains all game object prefabs.

&nbsp;   \* `Characters/`: Player, Enemy Soldiers, Jokers.

&nbsp;   \* `Buildings/`: Barracks, Towers, Main Tents.

&nbsp;   \* `Projectiles/`

\* `Assets/Scenes/`: Contains all game scenes.

&nbsp;   \* `Test/`: For scenes dedicated to testing a single system.



---



\## 3. Architectural Principles \& Best Practices (The Golden Rules)



All code contributed to this project \*\*must\*\* adhere to these principles.



\### 3.1. Data-Driven Design with ScriptableObjects

\*\*Rule:\*\* All configuration data, stats, and settings must be stored in ScriptableObject assets. Do not use hard-coded "magic numbers" in `MonoBehaviour` scripts.

\*\*Example:\*\* A unit's health and armor are defined in a `HealthStatsSO`, not as public floats in `Health.cs`. The `Health.cs` component holds a \*reference\* to its `HealthStatsSO`.



\### 3.2. Component-Based \& Single Responsibility Principle (SRP)

\*\*Rule:\*\* Entities are built by composing small, single-purpose components. Each class must have one, and only one, reason to change. Avoid monolithic "god classes."

\*\*Example:\*\* An AI soldier is a `GameObject` with `Health`, `MovementController`, `NavMeshMover`, `Attacker`, and `AIController` components. Each component handles only its own logic.



\### 3.3. Decoupled Communication via Global Event System

\*\*Rule:\*\* Communication between separate, high-level systems (e.g., a unit's death informing the UI and the ResourceManager) \*\*must\*\* be done through the static `GameEvents.cs` class. A component firing an event should not know who is listening.

\*\*Example:\*\* `Health.cs` should not have a reference to `UIManager`. Instead, when a unit dies, `Health.cs` calls `GameEvents.TriggerOnUnitDied()`. `UIManager` subscribes to `GameEvents.OnUnitDied` and reacts accordingly.



\### 3.4. Naming Conventions

\* \*\*Interfaces:\*\* Start with `I` (e.g., `IMoveable`, `IDestructible`).

\* \*\*ScriptableObjects:\*\* End with `SO` (e.g., `HealthStatsSO`, `LevelDataSO`).

\* \*\*Enums:\*\* `PascalCase` (e.g., `Faction`, `ResourceType`).

\* \*\*Classes \& Public Members:\*\* `PascalCase` (e.g., `Health`, `TakeDamage()`).

\* \*\*Private Fields:\*\* `\_camelCase` or `camelCase` (be consistent).



\### 3.5. Inspector-Friendly Code

\*\*Rule:\*\* All public fields must be organized and documented for clarity in the Unity Inspector.

\* Use `\[Header("...")]` to group related fields.

\* Use `\[Tooltip("...")]` for every public field to explain its purpose.



---



\## 4. How to Add a New Feature (Standard Workflow)



Follow these steps when implementing a new feature:



1\.  \*\*Define the Data (`ScriptableObjects`):\*\* First, identify all configuration data the feature needs. Create the necessary `...SO` class(es) in `Assets/Scripts/Data/`.

2\.  \*\*Define the Contracts (`Interfaces`):\*\* If the feature introduces a new, reusable capability (e.g., the ability to be "stunned"), define its contract by creating an `I...` interface.

3\.  \*\*Implement the Logic (`MonoBehaviours`):\*\* Write the core component script(s) in `Assets/Scripts/Gameplay/` or `Core/`. Ensure the script is modular and adheres to SRP.

4\.  \*\*Handle Communication (`GameEvents`):\*\* If the feature needs to broadcast information to the rest of the game, add a new `event Action` to `GameEvents.cs` and a corresponding `Trigger...()` method.

5\.  \*\*Create Prefabs \& Assets:\*\* Assemble the components on a new or existing Prefab and create the necessary `...SO` assets in the `Assets/Data/` folder.



---



\## 5. Testing Procedures



This project does not use a command-line test runner. All testing is done within the Unity Editor.



\* \*\*Primary Method:\*\* Play Mode testing. Always create a test case in a scene to validate functionality.

\* \*\*Debugging:\*\* Use comprehensive and context-aware logging.

&nbsp;   \* `Debug.Log()`: For key actions and state changes (e.g., "AIController engaging new target: EnemySwordsman").

&nbsp;   \* `Debug.LogWarning()`: For recoverable issues or unexpected states (e.g., "Target found, but it has no IDestructible component.").

&nbsp;   \* `Debug.LogError()`: For critical errors that break functionality (e.g., "HealthStatsSO is not assigned in the Inspector!").

\* \*\*Test Scenes:\*\* For complex systems, creating a dedicated test scene in `Assets/Scenes/Test/` is highly encouraged. This allows for testing a system in isolation.

