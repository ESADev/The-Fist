using DG.Tweening;
using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{
    public ResourceType resourceType;
    [Space]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private Ease moveEase = Ease.InOutQuint;

    //private float moveDuration = 0.5f;

    private int amount = 1;
    private float sizeLogBase = 50f;
    private float collectCooldown = 0.75f;
    private bool isCollected = false;
    private float initializationTime;

    public int GetResourceAmount()
    {
        return amount;
    }

    public void Initialize(int amount = 1)
    {
        initializationTime = Time.time;

        this.amount = amount;

        // Initialize the resource's visual size based on the amount
        InitializeSize();
    }

    private void InitializeSize()
    {
        transform.localScale *= 1 + Mathf.Log(amount, sizeLogBase);
    }

    public void Collect(Vector3 collectionPoint)
    {
        if (isCollected || Time.time - initializationTime < collectCooldown)
            return;

        isCollected = true;

        // Animation followed by logic for collecting the resource
        float randomizedDuration = moveDuration.Randomized(0.25f);
        transform.DOScale(Vector3.zero, randomizedDuration - 0.001f).SetEase(moveEase);
        transform.DOMove(collectionPoint, randomizedDuration).OnComplete(() =>
        {
            ResourceManager.Instance.AddResource(resourceType, amount);
            Destroy(gameObject);
        }).SetEase(moveEase);

        // SFX
        SFXManager.Instance.PlaySound("collect resource", transform.position);
    }

    /// <summary>
    /// Collect this resource while dynamically following a moving target (e.g. the player) over the tween duration.
    /// The resource smoothly shrinks to zero scale and homes toward the target's current position each frame.
    /// </summary>
    /// <param name="target">Transform to follow (its position may change while collecting).</param>
    public void CollectFollowing(Transform target)
    {
        if (isCollected || Time.time - initializationTime < collectCooldown || target == null)
            return;

        isCollected = true;

        float randomizedDuration = moveDuration.Randomized(0.25f);
        // Kill any existing tweens on this transform so they don't conflict with the manual coroutine animation.
        DOTween.Kill(transform, complete: false);
        StartCoroutine(FollowAndCollect(target, randomizedDuration));

        // SFX
        SFXManager.Instance.PlaySound("collect resource", transform.position);
    }

    private IEnumerator FollowAndCollect(Transform target, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Apply the same easing style used for the static tween.
            float easedT = DOVirtual.EasedValue(0f, 1f, t, moveEase);

            // Target might be destroyed; if so we keep last known position.
            Vector3 dynamicTargetPos = target != null ? target.position : transform.position;

            transform.position = Vector3.Lerp(startPos, dynamicTargetPos, easedT);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, easedT);
            yield return null;
        }

        // Finalize collection.
        ResourceManager.Instance.AddResource(resourceType, amount);
        Destroy(gameObject);
    }
}