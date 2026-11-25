using UnityEngine;
using UnityEngine.InputSystem;
/////////////// INFORMATION ///////////////
// SETUP:
// Add the script to the object you want the player to control
// TIPS:
// - The Rigidbody2D component should (probably) have some constraints: Freeze Rotation Z
// - Adjust the CapsuleCollider2D to fit the object appropriately
// - Objects that should be used as platforms needs to have a collider (non-trigger)
// LIMITATIONS:
// This script enables very limited platformer movement, but with minimal setup required.
// The available movement consists of: a jump with a fixed height (based on jumpForce), and moving side to side (based on maxSpeed).
// It will NOT easily enable you to add animations or sound or similar effects.
// This script automatically adds a PlayerInput, Rigidbody2D and a CapsuleCollider2D component in the inspector.
[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class LimitedPlatformerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeeda = 10f;
    [SerializeField] private float jumpForcea = 10f;
    private Vector2 moveInputa;
    private bool jumpInputa;
    private Rigidbody2D rba;
    private CapsuleCollider2D groundCheckCollidera; // Test
    void Awake()
    {
        rba = GetComponent<Rigidbody2D>();
        groundCheckCollidera = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
// Apply horizontal velocity from input
        rba.linearVelocityX = moveInputa.x * maxSpeeda;
// Apply vertical velocity from jump
        if (jumpInputa && IsGrounded())
        {
            rba.linearVelocityY = jumpForcea;
        }
        jumpInputa = false; // only jump once for each press of the jump-button
    }
    private bool IsGrounded()
    {
// Is our groundCheckCollider touching the groundLayer? If so, return the value "true"
        if (groundCheckCollidera.IsTouchingLayers(~0)) // ~0 is referring to EVERY layer.
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void OnMove(InputValue input)
    {
        moveInputa = input.Get<Vector2>();
    }
    public void OnJump(InputValue input)
    {
        jumpInputa = true;
    }
}

