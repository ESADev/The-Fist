using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central coordinator for high level game flow such as loading levels and
/// tracking the current <see cref="GameState"/>.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Current game state.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    [Header("Levels")]
    [Tooltip("Collection of levels available in the game. Order defines progression.")]
    public List<GameLevelDataSO> levels = new List<GameLevelDataSO>();

    [Tooltip("Index of the currently selected / to-be-played level (not necessarily loaded scene).")]
    [SerializeField] private int currentLevelIndex = 1;

    [Tooltip("Highest unlocked level index (inclusive).")]
    [SerializeField] private int highestUnlockedLevelIndex = 1;

    private const string Key_CurrentLevel = "GF_CurrentLevel";
    private const string Key_HighestUnlocked = "GF_HighestUnlocked";

    /// <summary>
    /// Fired when the current level selection changes. Param: new index.
    /// </summary>
    public static event Action<int> OnLevelChanged;

    /// <summary>
    /// Fired when the highest unlocked level changes. Param: new highest unlocked index.
    /// </summary>
    public static event Action<int> OnHighestUnlockedLevelChanged;

    /// <summary>
    /// The index currently selected (may not yet be loaded scene until Play invoked).
    /// </summary>
    public int CurrentLevelIndex => currentLevelIndex;

    /// <summary>
    /// The highest unlocked level index (inclusive).
    /// </summary>
    public int HighestUnlockedLevelIndex => highestUnlockedLevelIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // LoadProgress();
    }

    private void OnEnable()
    {
        SubscribeToGameEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromGameEvents();
    }

    /// <summary>
    /// Subscribes to global events used by the game manager.
    /// </summary>
    private void SubscribeToGameEvents()
    {
        GameEvents.OnVictory += HandleVictory;
        GameEvents.OnDefeat += HandleDefeat;
    }

    /// <summary>
    /// Unsubscribes from global events when the game manager is disabled.
    /// </summary>
    private void UnsubscribeFromGameEvents()
    {
        GameEvents.OnVictory -= HandleVictory;
        GameEvents.OnDefeat -= HandleDefeat;
    }

    /// <summary>
    /// Loads a level by index and initializes gameplay.
    /// </summary>
    /// <param name="levelIndex">Index of the level to load.</param>
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError($"[GameManager] Invalid level index {levelIndex}.", this);
            return;
        }

        GameLevelDataSO levelData = levels[levelIndex];
        if (levelData == null)
        {
            Debug.LogError($"[GameManager] Level data at index {levelIndex} is null.", this);
            return;
        }

        currentLevelIndex = levelIndex;
        CurrentState = GameState.Gameplay;
        Debug.Log($"[GameManager] Loading level {levelData.levelName} ({levelIndex}).");

        SceneManager.LoadScene(levelData.levelIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Called after a scene has loaded to initialize the level.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameLevelDataSO levelData = null;
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
        {
            levelData = levels[currentLevelIndex];
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.InitializeLevel(levelData);
        }
        else
        {
            Debug.LogError("[GameManager] No LevelManager found in loaded scene.", this);
        }
    }

    /// <summary>
    /// Handles logic when the game ends in victory.
    /// </summary>
    private void HandleVictory()
    {
        CurrentState = GameState.Victory;
        Debug.Log("[GameManager] Victory state reached.");

        // Progression: mark completed and unlock next level if any.
        if (currentLevelIndex == highestUnlockedLevelIndex)
        {
            int next = currentLevelIndex + 1;
            if (next < levels.Count)
            {
                highestUnlockedLevelIndex = next;
                PlayerPrefs.SetInt(Key_HighestUnlocked, highestUnlockedLevelIndex);
                PlayerPrefs.Save();
                OnHighestUnlockedLevelChanged?.Invoke(highestUnlockedLevelIndex);
            }
        }
    }

    /// <summary>
    /// Handles logic when the game ends in defeat.
    /// </summary>
    private void HandleDefeat()
    {
        CurrentState = GameState.Defeat;
        Debug.Log("[GameManager] Defeat state reached.");
    }

    #region Progression & Selection

    /// <summary>
    /// Select a level index (does not load). Must be unlocked unless force.
    /// </summary>
    public bool SetCurrentLevel(int index, bool force = false)
    {
        if (index < 0 || index >= levels.Count)
        {
            Debug.LogWarning($"[GameManager] Cannot select level {index}. Out of range.");
            return false;
        }
        if (!force && index > highestUnlockedLevelIndex)
        {
            Debug.LogWarning($"[GameManager] Cannot select locked level {index} (highest unlocked {highestUnlockedLevelIndex}).");
            return false;
        }
        if (index == currentLevelIndex) return true;
        currentLevelIndex = index;
        PlayerPrefs.SetInt(Key_CurrentLevel, currentLevelIndex);
        PlayerPrefs.Save();
        OnLevelChanged?.Invoke(currentLevelIndex);
        return true;
    }

    /// <summary>
    /// Loads the currently selected level.
    /// </summary>
    public void PlayCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    private void LoadProgress()
    {
        highestUnlockedLevelIndex = PlayerPrefs.GetInt(Key_HighestUnlocked, 0);
        currentLevelIndex = PlayerPrefs.GetInt(Key_CurrentLevel, 0);
        // Clamp to valid range.
        highestUnlockedLevelIndex = Mathf.Clamp(highestUnlockedLevelIndex, 0, Mathf.Max(0, levels.Count - 1));
        currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, highestUnlockedLevelIndex);
    }

    #endregion
}
