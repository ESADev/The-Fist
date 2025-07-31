using UnityEngine;

public class VoidProtection : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y < 0)
        {
            // Reset position to a safe height
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
    }
}
