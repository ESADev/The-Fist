using System.Collections;
using UnityEngine;
using DG.Tweening;

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
    float hideInteractionButtonCooldown = 2f;

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
        UpdateToNextLevel();
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

    private void UpdateToNextLevel()
    {
        if (!ResourceManager.Instance.SpendResource(nextLevel.cost))
        {
            Debug.LogError("Not enough resources to upgrade!");
            return;
        }

        if (currentBuilding != null)
        {
            try
            {
                currentBuilding.GetComponent<Entity>().Health.TakeDamage(Mathf.Infinity, null, null);
            }
            catch
            {
                Destroy(currentBuilding.gameObject);
            }
        }

        currentBuilding = Instantiate(nextLevel.characterPrefab, transform).gameObject;
        currentLevel++;
        RefreshNextLevel();
    }
}

