using UnityEngine;

public class FloatInTheAir : MonoBehaviour
{
    [SerializeField] private float floatHeight = 0.33f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private bool useCustomOffset = false;
    [SerializeField] private Vector3 customOffset = new Vector3(0, 1, 0);

    private Vector3 anchorPosition;
    private Vector3 offset;

    private void Start()
    {
        if (useCustomOffset)
        {
            offset = customOffset;
        }
        else
        {
            offset = new Vector3(0, 1, 0);
        }
        if (offset.y < floatHeight)
            offset.y = floatHeight;
        transform.position = transform.parent.position + offset;

        floatHeight = floatHeight.Randomized();
        floatSpeed = floatSpeed.Randomized();
    }

    private void Update()
    {
        SetAnchorPosition();
        float newY = anchorPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void SetAnchorPosition()
    {
        anchorPosition = transform.parent.position + offset;
    }
}