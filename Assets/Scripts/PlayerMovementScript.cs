using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    private bool grounded, wasGrounded = true;
    private Vector2 inputDirection = Vector2.zero;
    private 

    public bool controlEnabled { get; set; } = true;
    
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CapsuleCollider2D groundCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask groundLayer = ~0;
    
    [Header("Movement Values")]
    [SerializeField] private float maxSpeed = 15f;
    
    // Constructor
    
    // Methods
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        
    }

    private bool isGrounded()
    {
        if (groundCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void onMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            inputDirection = context.ReadValue<Vector2>();
        }
    }
}
