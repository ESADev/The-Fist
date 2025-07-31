using UnityEngine;

public class RotateContinuously : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private Space rotationSpace = Space.World;
    [SerializeField] private bool randomness = true;

    void Start()
    {
        if (randomness)
        {
            rotationSpeed = rotationSpeed.Randomized();
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, rotationSpace);
    }
}
