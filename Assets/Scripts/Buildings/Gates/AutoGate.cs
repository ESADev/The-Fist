using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Automatically opens gate doors when friendly entities are nearby.
/// Uses faction-based detection to determine friend or foe relationships.
/// </summary>
public class AutoGate : MonoBehaviour
{
    [Header("Gate Configuration")]
    [Tooltip("Transform representing the left hinge/door of the gate")]
    [SerializeField] private Transform leftHinge;
    
    [Tooltip("Transform representing the right hinge/door of the gate")]
    [SerializeField] private Transform rightHinge;
    
    [Header("Detection Settings")]
    [Tooltip("Detection range for nearby entities")]
    [SerializeField] private float detectionRange = 5f;
    
    [Tooltip("How often to scan for nearby entities (in seconds). Lower values = more responsive but higher performance cost")]
    [SerializeField] private float scanInterval = 0.5f;
    
    [Header("Animation Settings")]
    [Tooltip("How fast the gates open/close")]
    [SerializeField] private float gateAnimationDuration = 1f;
    
    [Tooltip("Maximum opening angle for each gate door (in degrees)")]
    [SerializeField] private float maxOpenAngle = 90f;
    
    [Tooltip("Time to keep gates open after no friendly entities detected")]
    [SerializeField] private float gateCloseDelay = 2f;
    
    [Tooltip("Easing curve for gate animation")]
    [SerializeField] private Ease gateEasing = Ease.OutBack;
    
    [Tooltip("Add punch effect when opening")]
    [SerializeField] private bool usePunchEffect = true;
    
    [Tooltip("Punch strength for opening animation")]
    [SerializeField] private float punchStrength = 0.1f;
    
    // Private fields
    private Entity gateEntity;
    private FactionType gateFaction;
    private bool isGateOpen = false;
    private bool shouldGateBeOpen = false;
    private float lastFriendlyDetectionTime;
    
    // Original rotations for gate reset
    private Quaternion leftHingeOriginalRotation;
    private Quaternion rightHingeOriginalRotation;
    
    // Target rotations for open state
    private Quaternion leftHingeOpenRotation;
    private Quaternion rightHingeOpenRotation;
    
    // DOTween references for animation control
    private Tween leftHingeTween;
    private Tween rightHingeTween;
    private Sequence gateAnimationSequence;
    
    private void Start()
    {
        InitializeGate();
        StartCoroutine(ScanForEntities());
    }
    
    private void InitializeGate()
    {
        // Get our own faction
        gateEntity = GetComponentInParent<Entity>();
        if (gateEntity == null)
        {
            Debug.LogError($"[AutoGate] No Entity component found in parent objects for {gameObject.name}. Gate will not function properly.");
            enabled = false;
            return;
        }
        
        gateFaction = gateEntity.Faction.CurrentFaction;
        
        // Validate hinge transforms
        if (leftHinge == null || rightHinge == null)
        {
            Debug.LogError($"[AutoGate] Left or Right hinge transform not assigned for {gameObject.name}. Gate will not function.");
            enabled = false;
            return;
        }
        
        // Store original rotations
        leftHingeOriginalRotation = leftHinge.localRotation;
        rightHingeOriginalRotation = rightHinge.localRotation;
        
        // Calculate open rotations (left door opens left, right door opens right)
        leftHingeOpenRotation = leftHingeOriginalRotation * Quaternion.Euler(0, -maxOpenAngle, 0);
        rightHingeOpenRotation = rightHingeOriginalRotation * Quaternion.Euler(0, maxOpenAngle, 0);
        
        Debug.Log($"[AutoGate] Gate initialized for faction: {gateFaction}");
    }
    
    private IEnumerator ScanForEntities()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(scanInterval);
            
            bool friendlyNearby = IsFriendlyEntityNearby();
            
            if (friendlyNearby)
            {
                lastFriendlyDetectionTime = Time.time;
                shouldGateBeOpen = true;
            }
            else
            {
                // Check if enough time has passed since last friendly detection
                if (Time.time - lastFriendlyDetectionTime > gateCloseDelay)
                {
                    shouldGateBeOpen = false;
                }
            }
            
            // Update gate state if needed
            if (shouldGateBeOpen != isGateOpen)
            {
                if (shouldGateBeOpen)
                {
                    OpenGate();
                }
                else
                {
                    CloseGate();
                }
            }
        }
    }
    
    private bool IsFriendlyEntityNearby()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRange);
        
        foreach (Collider col in nearbyColliders)
        {
            Entity nearbyEntity = col.GetComponentInParent<Entity>();
            if (nearbyEntity != null && nearbyEntity != gateEntity)
            {
                // Check if this entity is friendly (same faction)
                if (nearbyEntity.Faction.CurrentFaction == gateFaction)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private void OpenGate()
    {
        if (!isGateOpen)
        {
            isGateOpen = true;
            AnimateGate(true);

            SFXManager.Instance.PlaySound("gate opening", transform.position);

            Debug.Log($"[AutoGate] Opening gate for friendly {gateFaction} entity");
        }
    }
    
    private void CloseGate()
    {
        if (isGateOpen)
        {
            isGateOpen = false;
            AnimateGate(false);

            SFXManager.Instance.PlaySound("gate opening", transform.position);
            
            Debug.Log($"[AutoGate] Closing gate - no friendly entities nearby");
        }
    }
    
    private void AnimateGate(bool opening)
    {
        // Kill any existing animations
        if (gateAnimationSequence != null)
        {
            gateAnimationSequence.Kill();
        }
        
        // Determine target rotations
        Quaternion leftTargetRotation = opening ? leftHingeOpenRotation : leftHingeOriginalRotation;
        Quaternion rightTargetRotation = opening ? rightHingeOpenRotation : rightHingeOriginalRotation;
        
        // Create new animation sequence
        gateAnimationSequence = DOTween.Sequence();
        
        if (opening && usePunchEffect)
        {
            // For opening, use a more dramatic effect with punch
            leftHingeTween = leftHinge.DOLocalRotateQuaternion(leftTargetRotation, gateAnimationDuration)
                .SetEase(gateEasing);
                
            rightHingeTween = rightHinge.DOLocalRotateQuaternion(rightTargetRotation, gateAnimationDuration)
                .SetEase(gateEasing);
            
            // Add to sequence
            gateAnimationSequence.Append(leftHingeTween);
            gateAnimationSequence.Join(rightHingeTween);
            
            // Add punch effect for more impact
            gateAnimationSequence.Append(leftHinge.DOPunchRotation(Vector3.up * punchStrength, 0.3f, 5, 0.5f));
            gateAnimationSequence.Join(rightHinge.DOPunchRotation(Vector3.up * -punchStrength, 0.3f, 5, 0.5f));
        }
        else
        {
            // For closing or simple opening, use smooth animation
            Ease currentEasing = opening ? gateEasing : Ease.InOutQuad;
            
            leftHingeTween = leftHinge.DOLocalRotateQuaternion(leftTargetRotation, gateAnimationDuration)
                .SetEase(currentEasing);
                
            rightHingeTween = rightHinge.DOLocalRotateQuaternion(rightTargetRotation, gateAnimationDuration)
                .SetEase(currentEasing);
            
            // Add to sequence
            gateAnimationSequence.Append(leftHingeTween);
            gateAnimationSequence.Join(rightHingeTween);
        }
        
        // Optional: Add sound effects or particle effects on complete
        gateAnimationSequence.OnComplete(() => {
            if (opening)
            {
                Debug.Log("[AutoGate] Gate fully opened");
                // Could trigger opening sound/particles here
            }
            else
            {
                Debug.Log("[AutoGate] Gate fully closed");
                // Could trigger closing sound/particles here
            }
        });
        
        // Play the sequence
        gateAnimationSequence.Play();
    }
    
    private void OnDestroy()
    {
        // Clean up DOTween animations to prevent memory leaks
        if (gateAnimationSequence != null)
        {
            gateAnimationSequence.Kill();
        }
        
        if (leftHingeTween != null)
        {
            leftHingeTween.Kill();
        }
        
        if (rightHingeTween != null)
        {
            rightHingeTween.Kill();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection range in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw faction info
        if (Application.isPlaying && gateEntity != null)
        {
            Gizmos.color = gateFaction == FactionType.Player ? Color.blue : Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 3f, Vector3.one * 0.5f);
        }
    }
}
