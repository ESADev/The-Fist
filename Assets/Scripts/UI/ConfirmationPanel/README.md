# Confirmation Panel System

A clean, flexible confirmation panel system for Unity that allows any system to request user confirmation for actions.

## Components

### ConfirmationPanel.cs
The UI component that handles display and user interaction.

**Features:**
- Displays confirmation details (name, sprite, description, cost)
- Smooth fade in/out animations using DOTween and CanvasGroup
- Event-based confirmation/cancellation callbacks
- Auto-cleanup after user interaction

**Inspector Fields:**
- `confirmationPanelNameText` - Main title text (e.g., "Upgrade Boxer Barracks to Lvl 1")
- `itemSprite` - Image component for the sprite
- `itemNameText` - Item name text (e.g., "Lvl 1 Boxer Barracks")
- `descriptionText` - Description text component
- `costContainer` - Transform where cost items will be displayed
- `confirmButton` - Confirm action button
- `cancelButton` - Cancel action button
- `canvasGroup` - For fade animations (assign in inspector)
- `animationDuration` - Duration of fade animations

### ConfirmationPanelManager.cs
Singleton manager that handles panel instantiation and lifecycle.

**Features:**
- Singleton pattern for global access
- Prefab-based panel instantiation
- Multiple panel support (though typically one at a time)
- Auto-cleanup and memory management
- Fallback Canvas detection

**Inspector Fields:**
- `confirmationPanelPrefab` - Reference to the ConfirmationPanel prefab
- `panelParent` - Transform to parent panels to (usually Canvas)

## Setup Instructions

1. **Create the Prefab:**
   - Create a UI prefab with the required components
   - Add CanvasGroup component for animations
   - Assign all UI references in the ConfirmationPanel script
   - Save as prefab

2. **Setup Manager:**
   - Add ConfirmationPanelManager to a persistent GameObject
   - Assign the prefab reference
   - Assign parent transform (Canvas)

3. **Usage:**
   ```csharp
   // Create cost list
   List<ResourceAmount> cost = new List<ResourceAmount>
   {
       new ResourceAmount { type = ResourceType.Gold, amount = 100 },
       new ResourceAmount { type = ResourceType.Prestige, amount = 5 }
   };
   
   // Show confirmation panel
   ConfirmationPanelManager.Instance.ShowConfirmationPanel(
       panelName: "Upgrade Boxer Barracks to Lvl 1",
       sprite: buildingSprite,
       itemName: "Lvl 1 Boxer Barracks",
       description: "Increases unit capacity by 5 and unlocks new training options.",
       cost: cost,
       onConfirmed: () => Debug.Log("Confirmed!"),
       onCancelled: () => Debug.Log("Cancelled!")
   );
   ```

## Dependencies

- Unity UI (TextMeshPro, Image, Button)
- DOTween (for animations)
- ResourceAmount struct (existing in project)
- ResourceType enum (existing in project)

## Notes

- Panels are prefab-based, not code-generated
- Uses CanvasGroup for smooth fade animations
- Supports multiple simultaneous panels (though rarely needed)
- Auto-cleanup prevents memory leaks
- Clean separation of concerns between manager and panel logic
