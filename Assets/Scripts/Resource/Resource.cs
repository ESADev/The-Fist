using DG.Tweening;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceType resourceType;
    [Space]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private Ease moveEase = Ease.InOutQuint;

    //private float moveDuration = 0.5f;

    private int amount = 1;
    private float sizeLogBase = 100f;
    private float collectCooldown = 1.5f;
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
    }
}