using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    private bool grounded;
    private bool hasBouncedOnWall = false;
    private Vector2 inputDirection = Vector2.zero;
    private bool jumpInput = false;
    
    private bool jumping = false;
    private Vector2 horizontalVelocity = Vector2.zero; 
    private Vector2 verticalVelocity = Vector2.zero; 
    private float timeSinceMoveInput = 1.0f; 
    private float lastInputX = 0.0f;
    private float timeSinceFalling = 0.0f;

    public bool controlEnabled { get; set; } = true;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CapsuleCollider2D groundCollider;
    [SerializeField] private CapsuleCollider2D wallCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask groundLayer = ~0;
    
    [Header("Movement Values")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float walkAcceleration = 15f;
    [SerializeField] private float initalSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundedDeceleration = 0.05f;
    [SerializeField] private float aerialDeceleration = 0.5f;
    
    // Constructor
    
    // Methods
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rigidbody.gravityScale = 0f;
        controlEnabled = true;
    }
    
    void Start()
    {
        
    }


    void Update()
    {
        //Animation
        Vector2 horizAnimationVelocity = inputDirection;
        if (hasBouncedOnWall)
        {
            horizAnimationVelocity = horizontalVelocity;
        }
        
        if (horizAnimationVelocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (horizAnimationVelocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        grounded = isGrounded();
        ApplyGravity();

        bool touchingWall = isTouchingWall();

        if (grounded)
        {
            hasBouncedOnWall = false;
        }
        
        if (touchingWall)
        {
            if (horizontalVelocity.magnitude > initalSpeed * 1.5) 
            {
                hasBouncedOnWall = true;
                horizontalVelocity.x *= -0.8f;
            }
            else if (horizontalVelocity.magnitude < initalSpeed / 2.5) 
            {
                horizontalVelocity.x = 0;
            }
            
        }

        if (hasBouncedOnWall) // If wall collision has occurred, character is less responsive while airborne. 
        {
            timeSinceMoveInput = 0; 
            horizontalVelocity.x = math.lerp(horizontalVelocity.x, inputDirection.x * maxSpeed, Time.deltaTime);
            lastInputX = horizontalVelocity.x;
        }
        else if (inputDirection.x != 0) // If player moves horizontally, calculates and applies speed
        {
            timeSinceMoveInput = 0;
            if (math.abs(horizontalVelocity.x) < initalSpeed && grounded)
            {
                horizontalVelocity.x = inputDirection.x * initalSpeed;
            }
            horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity + (walkAcceleration * Time.deltaTime * inputDirection), maxSpeed);
            lastInputX = horizontalVelocity.x;
        }
        else // If no horizontal player input is detected, velocity/ movement is decelerated
        {
            timeSinceMoveInput += Time.deltaTime;
            
            float decel = aerialDeceleration;
            if (grounded)
            {
                decel = groundedDeceleration;
            }

            if (grounded && horizontalVelocity.x < initalSpeed)
            {
                decel *= decel;
            }
            
            horizontalVelocity.x = lastInputX * math.pow(decel,timeSinceMoveInput * 2);

            if (horizontalVelocity.magnitude < initalSpeed / 1.5) 
            {
                horizontalVelocity = Vector2.zero;
            }

            
        }
        
        
        //Where jumping actually happens
        if (jumpInput && grounded)
        {
            verticalVelocity.y = jumpForce;
            jumping = true;
            grounded = false;
        }
        //Stops jumping if the key is not held.
        else if (jumping && jumpInput == false)
        {
            jumping = false;
        }

        rigidbody.linearVelocity = new Vector2(horizontalVelocity.x,verticalVelocity.y);
    }

    // Checks collision with ground
    
    private bool isGrounded()
    {
        return groundCollider.IsTouchingLayers(groundLayer);
    }

    // Checks collision with wall
    
    private bool isTouchingWall()
    {
        return wallCollider.IsTouchingLayers(groundLayer);
    }

    // Calculates and applies gravity
    
    private void ApplyGravity()
    {
        if (grounded && verticalVelocity.y < 0.0f)
        {
            verticalVelocity.y = -1.0f;
            timeSinceFalling = 0;
        }
        else
        {
            timeSinceFalling += Time.deltaTime;
            float deceleration = 5f;
            if (verticalVelocity.y > 0.0f) // If traveling upward
            {
                if (jumping)
                {
                    deceleration = 2f;
                }
                else
                {
                    deceleration = 40f;
                }
                
            }

            verticalVelocity.y = math.clamp(verticalVelocity.y + (Physics2D.gravity.y * deceleration * Time.deltaTime),-25f,50);
            
        }
    }

    // Player moves with correct button inputs
    
    public void onMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            inputDirection = context.ReadValue<Vector2>().normalized;
        }
    }

    // Registers when player is pressing jump button
    
    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            jumpInput = true;
        }
        else if (context.canceled && controlEnabled)
        {
            jumpInput = false;
        }
    }
}
