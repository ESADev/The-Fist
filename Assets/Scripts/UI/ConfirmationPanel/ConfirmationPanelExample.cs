using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example class demonstrating how to use the ConfirmationPanel system
/// </summary>
public class ConfirmationPanelExample : MonoBehaviour
{
    [Header("Example Data")]
    [SerializeField] private Sprite exampleSprite;
    
    private void Start()
    {
        // Example usage of the confirmation panel system
        // This could be called from a button click or any other event
        
        // Note: Uncomment the line below to test the system
        ShowExampleConfirmationPanel();
    }
    
    /// <summary>
    /// Example method showing how to use the confirmation panel system
    /// </summary>
    public void ShowExampleConfirmationPanel()
    {
        // Create cost list
        List<ResourceAmount> cost = new List<ResourceAmount>
        {
            new ResourceAmount { type = ResourceType.Gold, amount = 100 },
            new ResourceAmount { type = ResourceType.Prestige, amount = 5 }
        };

        // Show confirmation panel
        ConfirmationPanelManager.Instance.ShowConfirmationPanel(
            panelName: "Upgrade Boxer Barracks to Lvl 1",
            sprite: exampleSprite,
            itemName: "Lvl 1 Boxer Barracks",
            itemType: "Hybrid_Soldier",
            description: "Increases unit capacity by 5 and unlocks new training options. This upgrade will improve your military capabilities significantly.",
            cost: cost,
            onConfirmed: OnUpgradeConfirmed,
            onCancelled: OnUpgradeCancelled
        );
    }
    
    private void OnUpgradeConfirmed()
    {
        Debug.Log("User confirmed the upgrade! Processing upgrade...");
        
        // Here you would implement the actual upgrade logic
        // For example:
        // - Check if player has enough resources
        // - Deduct resources
        // - Apply the upgrade
        // - Update UI
        
        Debug.Log("Upgrade completed successfully!");
    }
    
    private void OnUpgradeCancelled()
    {
        Debug.Log("User cancelled the upgrade.");
        
        // Here you might want to:
        // - Log the cancellation for analytics
        // - Return to previous UI state
        // - Show alternative options
    }
}
