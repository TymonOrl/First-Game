using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    IInputProvider input;
    
    [Header("Ground")]
    public float walkSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    
    public float maxGroundSpeed;
    public float maxGroundAcceleration;
    
    public float maxWallSpeed;
    public float maxWallAcceleration;
    
    [Header("Air")]
    public float maxAirSpeed;
    public float maxAirAcceleration;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundFriction;
    
    
    private Vector2 inputVec;
    Vector3 moveDirection;

    public MovementState state;

    public enum MovementState
    {
        walking,
        wallrunning,
        air
    }
    
    public bool wallrunning;

    public TextMeshProUGUI text_speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        
        input = GetComponent<IInputProvider>();

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.2f, whatIsGround))
        {
            grounded = true;
            groundFriction = hit.collider.sharedMaterial != null ? hit.collider.sharedMaterial.dynamicFriction : 0.6f;
        }
        else
        {
            grounded = false;
            groundFriction = 0f; // No friction in the air
        }
        
        MyInput();
        StateHandler();
        UpdateSpeedUI();
        
        
        Debug.Log($"Input: {inputVec} | Jump: {input.GetJumpInput()}");
    }

    private void FixedUpdate()
    {
        if (wallrunning) return;

        if (grounded)
        {
            MovePlayer();
        }
        else
        {
            MovePlayerAir();
        }
    }

    private void MyInput()
    {
        inputVec = input.GetMoveInput();
        bool jumpRequest = input.GetJumpInput();

        // when to jump
        if (jumpRequest && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
    }

    private void StateHandler()
    {
        if (grounded)
        {
            state = MovementState.walking;
        }
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        Vector3 wishDir = transform.right * inputVec.x + transform.forward * inputVec.y;
        wishDir.y = 0;
        wishDir = wishDir.normalized;
        
        rb.linearVelocity = ApplyFriction();
        float curr_speed = Vector3.Dot(rb.linearVelocity, wishDir);
        float add_speed = Mathf.Clamp(maxGroundSpeed - curr_speed, 0, maxGroundAcceleration * Time.fixedDeltaTime);

        rb.linearVelocity += wishDir * add_speed;
    }
    
    private void MovePlayerAir()
    {
        // calculate movement direction
        Vector3 wishDir = transform.right * inputVec.x + transform.forward * inputVec.y;
        wishDir.y = 0;
        wishDir = wishDir.normalized;
        
        float curr_speed = Vector3.Dot(rb.linearVelocity, wishDir);
        float add_speed = Mathf.Clamp(maxAirSpeed - curr_speed, 0, maxAirAcceleration * Time.fixedDeltaTime);

        rb.linearVelocity += wishDir * add_speed;
    }
    
    private Vector3 ApplyFriction()
    {
        Vector3 vel = rb.linearVelocity;
        float speed = vel.magnitude;

        if (speed < 0.1f) return vel;


        float drop = speed * groundFriction * Time.fixedDeltaTime;

        float newSpeed = Mathf.Max(speed - drop, 0);

        if (speed > 0) vel *= newSpeed / speed;


        return new Vector3(vel.x, rb.linearVelocity.y, vel.z);
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    
    private void clipVelocity(Vector3 normal, float overBounce, float delta)
    {
        float backoff = Vector3.Dot(rb.linearVelocity, normal);
        if (backoff >= 0)
            return;
        
        float change = backoff * overBounce;
        rb.linearVelocity -= normal * change;

        float adjust = Vector3.Dot(rb.linearVelocity, normal);
        if (adjust < 0)
        {
            rb.linearVelocity -= normal * adjust;
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (grounded || wallrunning) return;
        
        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(Vector3.up, contact.normal);
            
            if (angle > 5f && angle < 160f) 
            {
                clipVelocity(contact.normal, 1.001f, Time.fixedDeltaTime);
            }
        }
    }
    
    private void UpdateSpeedUI()
    {
        if (text_speed != null)
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            float speedDisplay = Mathf.Round(flatVel.magnitude * 10f) / 10f;
            
            text_speed.text = "Speed: " + speedDisplay;
        }
    }
    
}

