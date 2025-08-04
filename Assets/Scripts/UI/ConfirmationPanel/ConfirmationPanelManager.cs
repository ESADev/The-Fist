using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPanelManager : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private ConfirmationPanel confirmationPanelPrefab;
    
    [Header("Parent Transform")]
    [SerializeField] private Transform panelParent; // Usually the Canvas or UI root
    
    // Singleton instance
    private static ConfirmationPanelManager _instance;
    public static ConfirmationPanelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ConfirmationPanelManager>();
                if (_instance == null)
                {
                    Debug.LogError("ConfirmationPanelManager not found in scene!");
                }
            }
            return _instance;
        }
    }
    
    // Active panels list (in case multiple panels are needed)
    private List<ConfirmationPanel> activePanels = new List<ConfirmationPanel>();
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // If panelParent is not assigned, try to find Canvas
        if (panelParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                panelParent = canvas.transform;
            }
            else
            {
                Debug.LogWarning("No Canvas found for ConfirmationPanelManager. Please assign panelParent manually.");
            }
        }
    }
    
    /// <summary>
    /// Shows a confirmation panel with the specified parameters
    /// </summary>
    /// <param name="panelName">The main title of the confirmation panel</param>
    /// <param name="sprite">The sprite to display</param>
    /// <param name="itemName">The name of the item</param>
    /// <param name="description">Description text</param>
    /// <param name="cost">List of resource costs</param>
    /// <param name="onConfirmed">Callback when confirmed</param>
    /// <param name="onCancelled">Callback when cancelled</param>
    /// <returns>The created ConfirmationPanel instance</returns>
    public ConfirmationPanel ShowConfirmationPanel(
        string panelName,
        Sprite sprite,
        string itemName,
        string itemType,
        string description,
        List<ResourceAmount> cost,
        Action onConfirmed = null,
        Action onCancelled = null,
        bool canBeConfirmed = true)
    {
        if (confirmationPanelPrefab == null)
        {
            Debug.LogError("ConfirmationPanel prefab is not assigned!");
            return null;
        }
        
        if (panelParent == null)
        {
            Debug.LogError("Panel parent is not assigned!");
            return null;
        }
        
        // Instantiate the panel
        ConfirmationPanel panel = Instantiate(confirmationPanelPrefab, panelParent);
        
        // Setup the panel
        panel.Setup(panelName, sprite, itemName, itemType, description, cost, canBeConfirmed);
        
        // Subscribe to events
        if (onConfirmed != null)
        {
            panel.OnConfirmed += onConfirmed;
        }
        
        if (onCancelled != null)
        {
            panel.OnCancelled += onCancelled;
        }
        
        // Add cleanup listeners
        panel.OnConfirmed += () => RemovePanel(panel);
        panel.OnCancelled += () => RemovePanel(panel);
        
        // Add to active panels list
        activePanels.Add(panel);
        
        // Show the panel
        panel.Show();
        
        return panel;
    }
    
    /// <summary>
    /// Closes all active confirmation panels
    /// </summary>
    public void CloseAllPanels()
    {
        for (int i = activePanels.Count - 1; i >= 0; i--)
        {
            if (activePanels[i] != null)
            {
                activePanels[i].Hide();
            }
        }
    }
    
    /// <summary>
    /// Removes a panel from the active list and destroys it
    /// </summary>
    /// <param name="panel">The panel to remove</param>
    private void RemovePanel(ConfirmationPanel panel)
    {
        if (activePanels.Contains(panel))
        {
            activePanels.Remove(panel);
        }
        
        // Destroy the panel GameObject after a short delay to allow animation to complete
        if (panel != null)
        {
            Destroy(panel.gameObject, 0.5f);
        }
    }
    
    /// <summary>
    /// Gets the number of active confirmation panels
    /// </summary>
    public int ActivePanelCount => activePanels.Count;
    
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}