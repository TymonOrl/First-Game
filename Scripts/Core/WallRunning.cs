using System;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;

    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")] 
    private IInputProvider input;
    
    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    
    [Header("References")]
    public PlayerMovement playerMovement;
    private Rigidbody rb;
    
    private Vector2 inputVec;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        input = GetComponent<IInputProvider>();
    }

    public void Update()
    {
        CheckForWalls();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (playerMovement.wallrunning)
        {
            WallRunningMovement();
        }
    }
    
    private void CheckForWalls()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        inputVec = input.GetMoveInput();
        bool jumpRequest = input.GetJumpInput();

        if ((wallLeft || wallRight) && jumpRequest && AboveGround())
        {
            if (!playerMovement.wallrunning)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    private void StartWallRun()
    {
        playerMovement.wallrunning = true;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false; // Disable gravity while on wall
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // Ensure we move forward along the wall, not backward
        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        
        float currentSpeedAlongWall = Vector3.Dot(rb.linearVelocity, wallForward);
        if (currentSpeedAlongWall < playerMovement.maxWallSpeed)
        {
            rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        }
        
        // Optional
        if (!(wallLeft && inputVec.x > 0) && !(wallRight && inputVec.x < 0))
            rb.AddForce(-wallNormal * 10, ForceMode.Force);
    }

    private void StopWallRun()
    {
        playerMovement.wallrunning = false;
        rb.useGravity = true;
    }
}
