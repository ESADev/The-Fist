using UnityEngine;

/// <summary>
/// Controls runtime behaviour for an individual level such as spawning
/// gameplay elements and checking win conditions.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Data asset describing this level.")]
    public GameLevelDataSO data;

    /// <summary>
    /// Reference to the enemy's main base in the scene.
    /// </summary>
    [Tooltip("Reference to the player's main base in the scene.")]
    public Entity playerMainBase;

    /// <summary>
    /// Reference to the enemy's main base in the scene.
    /// </summary>
    [Tooltip("Reference to the enemy's main base in the scene.")]
    public Entity enemyMainBase;

    /// <summary>
    /// Reference to the player's primary controllable character / hero.
    /// Defeat is also triggered when this entity dies.
    /// </summary>
    [Tooltip("Reference to the player's main controllable entity (hero). Defeat if it dies.")]
    public Entity playerEntity;

    // Track whether we've already resolved the outcome to avoid double firing via both polling & event.
    private bool _outcomeResolved;

    private void OnEnable()
    {
        // Subscribe only to the player's hero death – base deaths are still polled each frame (minimal change per request).
        if (playerEntity != null && playerEntity.Health != null)
        {
            playerEntity.Health.OnDied += HandlePlayerHeroDeath;
        }
    }

    private void OnDisable()
    {
        if (playerEntity != null && playerEntity.Health != null)
        {
            playerEntity.Health.OnDied -= HandlePlayerHeroDeath;
        }
    }

    private void Update()
    {
        CheckWin();
    }

    /// <summary>
    /// Initializes the level using the provided data asset.
    /// </summary>
    /// <param name="levelData">Data asset for the level.</param>
    public void InitializeLevel(GameLevelDataSO levelData)
    {
        data = levelData;
        if (data == null)
        {
            Debug.LogError("[LevelManager] GameLevelDataSO is null.", this);
            return;
        }
    }

    /// <summary>
    /// Checks if the player has fulfilled win conditions.
    /// </summary>
    private void CheckWin()
    {
        bool enemyBaseDead = enemyMainBase != null && enemyMainBase.Health != null && enemyMainBase.Health.IsDead;
        bool playerBaseDead = playerMainBase != null && playerMainBase.Health != null && playerMainBase.Health.IsDead;

        if (!_outcomeResolved && enemyBaseDead)
        {
            Debug.Log("[LevelManager] Enemy base destroyed. Triggering victory.");
            GameEvents.TriggerOnVictory();
            _outcomeResolved = true;
            enabled = false; // Stop polling once resolved.
            return;
        }

        if (!_outcomeResolved && playerBaseDead)
        {
            Debug.Log("[LevelManager] player base destroyed. Triggering defeat.");
            GameEvents.TriggerOnDefeat();
            _outcomeResolved = true;
            enabled = false;
        }
    }

    /// <summary>
    /// Handles the player's controllable hero death via Health event.
    /// </summary>
    private void HandlePlayerHeroDeath(GameObject deadObject)
    {
        if (_outcomeResolved)
        {
            return;
        }

        if (playerEntity != null && deadObject == playerEntity.gameObject)
        {
            Debug.Log("[LevelManager] player hero died. Triggering defeat.");
            GameEvents.TriggerOnDefeat();
            _outcomeResolved = true;
            enabled = false; // Stop further polling.
        }
    }
}