using UnityEngine;

public class AIInputProvider : MonoBehaviour, IInputProvider
{
    // Tests will write to these variables directly
    public Vector2 MoveInput;
    public Vector2 LookInput;
    public bool JumpInput;
    public bool CrouchInput;

    public Vector2 GetMoveInput() => MoveInput;
    public Vector2 GetLookInput() => LookInput;
    public bool GetJumpInput() => JumpInput;
    public bool GetCrouchInput() => CrouchInput;
}