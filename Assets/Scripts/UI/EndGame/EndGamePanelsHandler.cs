using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles the display of endgame panels (win/lose) with smooth fade animations.
/// Uses CanvasGroup components for seamless transitions.
/// </summary>
public class EndGamePanelsHandler : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private Ease fadeInEase = Ease.OutQuart;
    
    private void Awake()
    {
        // Initialize both panels as invisible and non-interactive
        InitializePanel(canvasGroup);
    }
    
    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnVictory += HandleVictory;
        GameEvents.OnDefeat += HandleDefeat;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnVictory -= HandleVictory;
        GameEvents.OnDefeat -= HandleDefeat;
    }
    
    /// <summary>
    /// Displays the appropriate endgame panel based on the game result.
    /// </summary>
    /// <param name="gameResult">The result of the game (Victory or Defeat)</param>
    public void ShowEndGamePanel(GameState gameResult)
    {
        switch (gameResult)
        {
            case GameState.Victory:
                ShowWinPanel();
                break;
            case GameState.Defeat:
                ShowLosePanel();
                break;
            default:
                Debug.LogWarning($"[EndGamePanelsHandler] Unexpected game result: {gameResult}");
                break;
        }
    }
    
    /// <summary>
    /// Shows the victory panel with a smooth fade-in animation.
    /// </summary>
    public void ShowWinPanel()
    {
        Debug.Log("[EndGamePanelsHandler] Showing victory panel");
        winText.SetActive(true);
        loseText.SetActive(false);
        ShowPanel(canvasGroup);
    }
    
    /// <summary>
    /// Shows the defeat panel with a smooth fade-in animation.
    /// </summary>
    public void ShowLosePanel()
    {
        Debug.Log("[EndGamePanelsHandler] Showing defeat panel");
        winText.SetActive(false);
        loseText.SetActive(true);
        ShowPanel(canvasGroup);
    }
    
    /// <summary>
    /// Hides both endgame panels.
    /// </summary>
    public void HideAllPanels()
    {
        Debug.Log("[EndGamePanelsHandler] Hiding all endgame panels");
        HidePanel(canvasGroup);
    }
    
    private void HandleVictory()
    {
        ShowWinPanel();
    }
    
    private void HandleDefeat()
    {
        ShowLosePanel();
    }
    
    private void InitializePanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("[EndGamePanelsHandler] CanvasGroup reference is missing!");
            return;
        }
        
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);
    }
    
    private void ShowPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("[EndGamePanelsHandler] CanvasGroup reference is missing!");
            return;
        }
        
        // Kill any existing tweens on this canvas group
        canvasGroup.DOKill();
        
        // Ensure the panel is active but invisible
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Animate fade in
        canvasGroup.DOFade(1f, fadeInDuration)
            .SetEase(fadeInEase)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }
    
    private void HidePanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;
        
        // Kill any existing tweens on this canvas group
        canvasGroup.DOKill();
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        canvasGroup.DOFade(0f, fadeInDuration * 0.5f)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(false);
            });
    }
    
    private void OnDestroy()
    {
        // Kill any ongoing tweens to prevent errors
        if (canvasGroup != null)
            canvasGroup.DOKill();
    }
}