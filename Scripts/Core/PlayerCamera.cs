using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _sensitivity = 0.5f; // reduced default
    [SerializeField] private float _maxLookAngle = 85f;

    [Header("References")]
    [SerializeField] private Transform _playerBody; // The capsule/root to rotate

    private IInputProvider _input;
    private float _currentPitch = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _input = GetComponentInParent<IInputProvider>();
    }

    private void LateUpdate()
    {
        if (_input == null) return;
        
        Vector2 lookInput = _input.GetLookInput() * _sensitivity;

        // Yaw - Left and Right
        if (_playerBody != null)
        {
            _playerBody.Rotate(Vector3.up * lookInput.x);
        }

        // Pitch - Up & Down
        _currentPitch -= lookInput.y;
        _currentPitch = Mathf.Clamp(_currentPitch, -_maxLookAngle, _maxLookAngle);

        transform.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
    }
}