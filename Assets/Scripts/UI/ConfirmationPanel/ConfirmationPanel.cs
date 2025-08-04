using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class ConfirmationPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI confirmationPanelNameText;
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    
    // Events
    public event Action OnConfirmed;
    public event Action OnCancelled;
    
    private void Awake()
    {
        // Set initial state
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Setup button listeners
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }
    
    public void Setup(string panelName, Sprite sprite, string itemName, string itemType, string description, List<ResourceAmount> cost, bool canBeConfirmed)
    {
        confirmationPanelNameText.text = panelName;
        itemSprite.sprite = sprite;
        itemNameText.text = itemName;
        itemTypeText.text = itemType;
        descriptionText.text = description;

        confirmButton.interactable = canBeConfirmed;
        
        // Setup cost display
        SetupCostDisplay(cost);
    }

    private void SetupCostDisplay(List<ResourceAmount> cost)
    {
        costText.text = "";

        // Create cost display for each resource
        foreach (var resourceAmount in cost)
        {
            costText.text += $"{resourceAmount.type}: {resourceAmount.amount},";
        }

        // Remove the last comma if there is any cost
        if (costText.text.Length > 0)
        {
            costText.text = costText.text.Substring(0, costText.text.Length - 1);
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        canvasGroup.DOFade(1f, animationDuration)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }
    
    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        canvasGroup.DOFade(0f, animationDuration)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
    
    private void OnConfirmClicked()
    {
        OnConfirmed?.Invoke();
        Hide();
    }
    
    private void OnCancelClicked()
    {
        OnCancelled?.Invoke();
        Hide();
    }
    
    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(OnConfirmClicked);
        cancelButton.onClick.RemoveListener(OnCancelClicked);
        
        // Kill any ongoing tweens
        canvasGroup.DOKill();
    }
}