using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    private bool grounded, wasGrounded = true;
    private bool hasBouncedOnWall = false;
    private Vector2 inputDirection = Vector2.zero;
    private bool jumpInput = false;
    
    private bool jumping = false;
    private Vector2 horizontalVelocity = Vector2.zero;
    private Vector2 verticalVelocity = Vector2.zero;
    private float timeSinceMove = 1.0f;
    private float lastInputX = 0.0f;

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
    [SerializeField] private float groundedDeceleration = 0.5f;
    [SerializeField] private float aerialDeceleration = 0.5f;
    
    // Constructor
    
    // Methods
    
    void Start()
    {
        
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rigidbody.gravityScale = 0f;
        controlEnabled = true;
    }

    void Update()
    {
        //Animation
        Vector2 horzAnimationVelocity = inputDirection;
        if (hasBouncedOnWall)
        {
            horzAnimationVelocity = horizontalVelocity;
        }
        
        if (horzAnimationVelocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (horzAnimationVelocity.x < 0)
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
            timeSinceMove = 0; 
            horizontalVelocity.x = math.lerp(horizontalVelocity.x, inputDirection.x * maxSpeed, Time.deltaTime);
            lastInputX = horizontalVelocity.x;
        }
        else if (inputDirection.x != 0) // If player moves horizontally, calculates and applies speed
        {
            timeSinceMove = 0;
            if (math.abs(horizontalVelocity.x) < initalSpeed && grounded)
            {
                horizontalVelocity.x = inputDirection.x * initalSpeed;
            }
            horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity + (walkAcceleration * Time.deltaTime * inputDirection), maxSpeed);
            lastInputX = horizontalVelocity.x;
        }
        else // If no horizontal player input is detected, velocity/ movement is decelerated
        {
            timeSinceMove += Time.deltaTime;
            
            float decel = aerialDeceleration;
            if (grounded)
            {
                decel = groundedDeceleration;
            }

            if (grounded && horizontalVelocity.magnitude < initalSpeed)
            {
                decel *= decel;
            }
            
            horizontalVelocity.x = lastInputX * math.pow(decel,timeSinceMove);

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

        wasGrounded = grounded;
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
        }
        else
        {
            if (verticalVelocity.y > 0.0f)
            {
                float deceleration = 10;
                if (jumping)
                {
                    deceleration = 2;
                }

                verticalVelocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            else
            {
                jumping = false;
                verticalVelocity.y += Physics2D.gravity.y * Time.deltaTime;
            }
        }
    }

    // Player moves with correct button inputs
    
    public void onMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            inputDirection = context.ReadValue<Vector2>().normalized;
            //Debug.Log("Bullishitti");
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
