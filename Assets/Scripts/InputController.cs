using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class for input action event propagation
/// </summary>
public class InputController : MonoBehaviour
{
    public delegate void OnMove(Vector2 move);
    public static event OnMove OnMoveEvent;

    public delegate void OnRotate(float direction);
    public static event OnRotate OnRotateEvent;

    public delegate void OnRotateField(float direction);
    public static event OnRotateField OnRotateFieldEvent;

    public delegate void OnDropDown();
    public static event OnDropDown OnDropDownEvent;

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRotateAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRotateEvent?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnRotateFieldAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnRotateFieldEvent?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnDropBlock(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            OnDropDownEvent?.Invoke();
        }
    }

}
