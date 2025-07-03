using System;
using UnityEngine;

/// <summary>
/// Brain component that handles target acquisition and basic movement.
/// Uses ScriptableObject settings for data-driven behavior.
/// Communicates via events so other systems can react without tight coupling.
/// </summary>
[RequireComponent(typeof(FactionComponent))]
public class UnitAIController : MonoBehaviour
{
    public enum AIState { Moving, Fighting }

    [SerializeField] private UnitAISettingsSO settings;
    [SerializeField] private Transform longRangeDestination;

    private FactionComponent _faction;
    private AIState _state = AIState.Moving;
    private GameObject _currentTarget;
    private float _scanTimer;

    public AIState State => _state;
    public GameObject CurrentTarget => _currentTarget;

    // Fired when a new target is acquired
    public event Action<GameObject> OnTargetAcquired;

    // Fired when the current target is lost
    public event Action OnTargetLost;

    private void Awake()
    {
        _faction = GetComponent<FactionComponent>();
        if (settings == null)
        {
            Debug.LogError("[UnitAIController] Settings asset not assigned.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (settings == null) return;

        _scanTimer += Time.deltaTime;
        if (_scanTimer >= settings.scanInterval)
        {
            _scanTimer = 0f;
            ScanForTargets();
        }

        if (_state == AIState.Moving)
        {
            MoveTowards(longRangeDestination ? longRangeDestination.position : transform.position + transform.forward);
        }
        else if (_state == AIState.Fighting && _currentTarget != null)
        {
            MoveTowards(_currentTarget.transform.position);

            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
            if (distance > settings.aggroRadius)
            {
                ClearTarget();
            }
        }
    }

    private void MoveTowards(Vector3 destination)
    {
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * settings.moveSpeed * Time.deltaTime;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private void ScanForTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, settings.aggroRadius);
        float closestDist = float.MaxValue;
        GameObject closest = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            FactionComponent otherFaction = hit.GetComponent<FactionComponent>();
            if (otherFaction == null) continue;
            if (otherFaction.unitTeam == _faction.unitTeam) continue; // ignore allies

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        if (closest != null)
        {
            SetTarget(closest);
        }
        else if (_state == AIState.Fighting)
        {
            ClearTarget();
        }
    }

    private void SetTarget(GameObject target)
    {
        if (_currentTarget == target) return;

        _currentTarget = target;
        _state = AIState.Fighting;
        OnTargetAcquired?.Invoke(target);
    }

    private void ClearTarget()
    {
        if (_state == AIState.Moving) return;

        _currentTarget = null;
        _state = AIState.Moving;
        OnTargetLost?.Invoke();
    }
}
