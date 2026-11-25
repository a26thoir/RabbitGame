using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    private bool grounded, wasGrounded = true;
    private Vector2 inputDirection = Vector2.zero;
    private Vector2 horizontalVelocity = Vector2.zero;
    private Vector2 verticalVelocity = Vector2.zero;

    public bool controlEnabled { get; set; } = true;
    
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CapsuleCollider2D groundCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask groundLayer = ~0;
    
    [Header("Movement Values")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float walkAcceleration = 15f;
    [SerializeField] private float initalSpeed = 5f;
    
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
        
        
        
    }

    private void FixedUpdate()
    {
        grounded = isGrounded();

        if (inputDirection.x != 0)
        {
            if (math.abs(horizontalVelocity.x) < initalSpeed)
            {
                horizontalVelocity.x = inputDirection.x * initalSpeed;
            }
            horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity + (walkAcceleration * Time.deltaTime * inputDirection), maxSpeed);
        }
        else
        {
            horizontalVelocity = Vector2.zero;
        }


        rigidbody.linearVelocity = new Vector2(horizontalVelocity.x,verticalVelocity.y);
        
        //Animation
        if (inputDirection.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (inputDirection.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool isGrounded()
    {
        return groundCollider.IsTouchingLayers(groundLayer);
    }

    public void onMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            inputDirection = context.ReadValue<Vector2>().normalized;
            Debug.Log("Bullishitti");
        }
    }
}
