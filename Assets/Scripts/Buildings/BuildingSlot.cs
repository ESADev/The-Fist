using System.Collections;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

/// <summary>
/// Handles constructing and upgrading a building through simple interactions.
/// </summary>
[DisallowMultipleComponent]
public class BuildingSlot : MonoBehaviour, IInteractable
{
    [Tooltip("Upgrade tree for this slot.")]
    public BuildingUpgradeTreeSO upgradeTree;

    [Space]
    [SerializeField] float fadeDuration = 0.25f;

    [Space]
    [SerializeField] CanvasGroup interactionButton;

    [SerializeField] private GameObject currentBuilding = null;
    private int currentLevel = -1; // Meaning it is not unlocked
    private CharacterDefinitionSO nextLevel;
    private bool maxedOut = false;
    private bool beingInteracted = false;
    AutoInteractor interactor = null;
    Coroutine hideInteractionButtonAfterCooldownCor;
    float lastInteractionRequestTime = -1f;
    float hideInteractionButtonCooldown = 0.05f;
    
    // Track health component of the currently spawned building so we can listen for death
    private Health _currentBuildingHealth;

    /// <summary>
    /// Gets the current building level. Zero means nothing built yet.
    /// </summary>
    public int CurrentLevel => currentLevel;

    /// <summary>
    /// Called when an interactor uses this slot. Builds the next level if possible.
    /// </summary>
    /// <param name="interactor">The interacting entity.</param>
    public void Interact(AutoInteractor interactor)
    {
        if (beingInteracted)
        {
            if (interactor == this.interactor)
            {
                lastInteractionRequestTime = Time.time;
            }

            return;
        }

        beingInteracted = true;
        this.interactor = interactor;
        lastInteractionRequestTime = Time.time;

        ShowInteractionButton();
    }

    private void ShowInteractionButton()
    {
        interactionButton.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            interactionButton.interactable = true;
            interactionButton.blocksRaycasts = true;
        });

        ActicateHideInteractionButtonCheck();
    }

    private void ActicateHideInteractionButtonCheck()
    {
        if (hideInteractionButtonAfterCooldownCor != null) StopCoroutine(hideInteractionButtonAfterCooldownCor);
        hideInteractionButtonAfterCooldownCor = StartCoroutine(HideInteractionButtonCoroutine(hideInteractionButtonCooldown));
    }

    private IEnumerator HideInteractionButtonCoroutine(float delay)
    {
        while (Time.time - lastInteractionRequestTime < delay)
        {
            int ticksToWait = 10;
            for (int i = 0; i < ticksToWait; i++)
            {
                yield return null;
            }
        }

        beingInteracted = false;

        interactionButton.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            interactionButton.interactable = false;
            interactionButton.blocksRaycasts = false;
        });
    }

    public void OnInteractionButtonClicked()
    {
        RefreshNextLevel();

        string panelName = "";
        if (currentLevel == -1)
            panelName = "Unlock " + nextLevel.name + "?";
        else
            panelName = "Upgrade to " + nextLevel.name + "?";
        ConfirmationPanelManager.Instance.ShowConfirmationPanel(
            panelName,
            nextLevel.characterPortrait,
            nextLevel.name,
            nextLevel.characterType.ToString(),
            nextLevel.description,
            nextLevel.cost,
            onConfirmed: OnConfirmed,
            onCancelled: OnCancelled,
            ResourceManager.Instance.CanAfford(nextLevel.cost) && !CheckMaxedOut()
        );


    }

    private void OnConfirmed()
    {
        StartCoroutine(UpdateToNextLevelCoroutine());
    }

    private void OnCancelled()
    {

    }

    private void RefreshNextLevel()
    {
        if (CheckMaxedOut())
            return;
        nextLevel = upgradeTree.levels[currentLevel + 1];
    }

    private bool CheckMaxedOut()
    {
        maxedOut = !(currentLevel < upgradeTree.levels.Count - 1);
        return maxedOut;
    }

    private IEnumerator UpdateToNextLevelCoroutine()
    {
        if (!ResourceManager.Instance.SpendResource(nextLevel.cost))
        {
            Debug.LogError("Not enough resources to upgrade!");
            yield break;
        }

        float destroyAnimationDuraiton = 0.15f;

        if (currentBuilding != null)
        {
            //currentBuilding.GetComponent<Entity>().Health.TakeDamage(Mathf.Infinity, null, null);
            // Detach previous listener so upgrade removal does not count as a death downgrade
            DetachCurrentBuildingHealthListener();
            currentBuilding.transform.DOScale(Vector3.zero, destroyAnimationDuraiton);
            yield return new WaitForSecondsRealtime(destroyAnimationDuraiton);
        }


        if (currentBuilding != null)
        {
            Destroy(currentBuilding.gameObject);
        }

        currentBuilding = Instantiate(nextLevel.characterPrefab, transform).gameObject;
        currentLevel++;
        AttachCurrentBuildingHealthListener();
        RefreshNextLevel();

        // SFX
        SFXManager.Instance.PlaySound("upgrade", transform.position);

        // VFX
        VFXManager.Instance.PlayEffect("upgrade", transform.position);
    }

    #region Building Death Handling
    private void AttachCurrentBuildingHealthListener()
    {
        if (currentBuilding == null) return;

        // Prefer direct Health; fall back to Entity -> Health
        _currentBuildingHealth = currentBuilding.GetComponent<Health>();
        if (_currentBuildingHealth == null)
        {
            Entity entity = currentBuilding.GetComponent<Entity>();
            if (entity != null)
            {
                _currentBuildingHealth = entity.Health;
            }
        }

        if (_currentBuildingHealth != null)
        {
            _currentBuildingHealth.OnDied += HandleCurrentBuildingDied;
        }
        else
        {
            Debug.LogWarning($"[BuildingSlot] Spawned building '{currentBuilding.name}' has no Health component to monitor.");
        }
    }

    private void DetachCurrentBuildingHealthListener()
    {
        if (_currentBuildingHealth != null)
        {
            _currentBuildingHealth.OnDied -= HandleCurrentBuildingDied;
            _currentBuildingHealth = null;
        }
    }

    private void HandleCurrentBuildingDied(GameObject dead)
    {
        // Ignore if somehow different reference
        if (currentBuilding == null || dead != currentBuilding) return;

        Debug.Log($"[BuildingSlot] Building '{dead.name}' died. Downgrading slot level.");

        DetachCurrentBuildingHealthListener();
        currentBuilding = null; // Already dead

        // Downgrade one level so player can rebuild same tier
        int previousLevel = currentLevel;
        currentLevel = Mathf.Max(-1, currentLevel - 1);
        if (previousLevel != currentLevel)
        {
            RefreshNextLevel();
        }

        // Optional feedback (reuse upgrade VFX/SFX or dedicated ones if available)
        SFXManager.Instance.PlaySound("building_destroyed", transform.position); // Will silently fail if not defined
        VFXManager.Instance.PlayEffect("building_destroyed", transform.position);
    }

    private void OnDestroy()
    {
        DetachCurrentBuildingHealthListener();
    }
    #endregion
}

