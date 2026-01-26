using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour, IInputProvider
{
    private SourceMovement _inputActions;

    private void Awake()
    {
        _inputActions = new SourceMovement();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public Vector2 GetMoveInput()
    {
        // Reads the raw Vector2 from the "Move" action we configured
        return _inputActions.Gameplay.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return _inputActions.Gameplay.Look.ReadValue<Vector2>();
    }

    public bool GetJumpInput()
    {
        // For bunnyhopping, we usually check "IsPressed" or "WasPressedThisFrame"
        return _inputActions.Gameplay.Jump.IsPressed();
    }

    public bool GetCrouchInput()
    {
        return _inputActions.Gameplay.Crouch.IsPressed();
    }
}