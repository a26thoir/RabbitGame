using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    // Fields

    public bool canMove { get; set; } = true; // Can set movement controls on/ off Reference: Canvas Customizable Script 
    
    [SerializeField] private Rigidbody2D rigidbody;
    // Constructor
    
    // Methods
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        
    }
}
