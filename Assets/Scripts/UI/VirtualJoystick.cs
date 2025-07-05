using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the virtual joystick UI element.
/// This component is responsible for translating UI pointer events (drag, down, up)
/// into a normalized horizontal movement vector. It then passes this information
/// to the InputManager. This is a great example of SRP, as this script's only
/// concern is the joystick's UI logic. It doesn't know or care what moves,
/// just that it needs to report input.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Visuals")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    private Vector2 currentInput = Vector2.zero;
    private bool isHeld = false;

    private void Start()
    {
        // Ensure the handle is centered initially
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (isHeld && InputManager.Instance != null)
        {
            InputManager.Instance.SetMovementInput(currentInput);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // When the user first presses on the joystick area, start tracking the drag
        isHeld = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition;
        // Convert screen point to local point within the joystick background
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out dragPosition))
        {
            // Calculate the vector from the center of the joystick to the drag position
            Vector2 joystickVector = dragPosition;

            // Clamp the handle's position to the background's radius
            if (joystickVector.magnitude > joystickBackground.sizeDelta.x / 2f)
            {
                joystickVector = joystickVector.normalized * (joystickBackground.sizeDelta.x / 2f);
            }

            // Move the visual handle
            joystickHandle.anchoredPosition = joystickVector;

            // Normalize the input vector and send it to the InputManager
            Vector2 normalizedInput = joystickVector / (joystickBackground.sizeDelta / 2f);
            currentInput = normalizedInput;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // When the user lifts their finger/mouse, reset the handle and input
        isHeld = false;
        joystickHandle.anchoredPosition = Vector2.zero;
        currentInput = Vector2.zero;
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetMovementInput(Vector2.zero);
        }
    }
}
