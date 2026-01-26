using UnityEngine;

public interface IInputProvider
{
    // Interface defining what player can do
    Vector2 GetMoveInput();
    
    Vector2 GetLookInput();
    
    bool GetJumpInput();
    
    bool GetCrouchInput();
}
