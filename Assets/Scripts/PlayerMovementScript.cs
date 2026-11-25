using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    private bool grounded, wasGrounded = true;
    
    
    
    [SerializeField] private Rigidbody2D rigidbody;

    [SerializeField] private CapsuleCollider2D groundCollider;

    [SerializeField] private LayerMask groundLayer = ~0;
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
        
    }
}
