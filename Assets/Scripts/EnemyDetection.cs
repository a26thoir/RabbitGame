using Unity.VisualScripting;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Tooltip("This is where you insert the player/object you want detected")] 
    [SerializeField] private GameObject player;

    [Tooltip("This is where you insert the platform the enemy walks on")] 
    [SerializeField] private GameObject bounds;

    private Bounds BoundObject => bounds.GetComponent<Bounds>();

    [Tooltip("This is the offset of the platform, for tweaking edges")] //Not to be confused with the raycastingoffset, as we're using both methods
    [SerializeField] private float boundsOffset;

    [Tooltip("This is the offset how far ahead the raycast should check for ground")]
    [SerializeField] private float raycastOffset;

    [Tooltip("This is how far down the raycast will check for ground")] 
    [SerializeField] private float rayDistance;
    
    [Tooltip("This is the speed in the patrol state")] 
    [SerializeField] public float patrolSpeed = 2.0f;

    [Tooltip("This is the HuntSpeed of the enemy")] 
    [SerializeField] private float huntSpeed = 6.0f;

    [Tooltip("Insert which way the sprite is facing at default (-1 = left, 1 = right) NEVER 0")] 
    [SerializeField] private int direction;

    
    [Tooltip("This is the object in which is the ¨vision¨ for the enemy(usually a cone or box)")]
    [SerializeField] private GameObject detectionZone;

    private float idleTimer = 2.0f;

    private float detectionTime = 1.0f;

    private bool spotted;

    private float idleToPatroltimer;
    
    
    private Vector2 BoundsLeft => new Vector2(BoundObject.min.x, BoundObject.min.y);
    private Vector2 BoundsRight => new Vector2(BoundObject.max.x, BoundObject.min.y);
    

    private EnemyStates currentstate = EnemyStates.Patrol;

    void Start()
    {
        
    }
    
    public enum EnemyStates
    {
        Idle,
        Patrol,
        Detect
    }

    private void Detect()
    {
        detectionTime -= Time.deltaTime;

        if (detectionTime <= 0)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position, 
                    player.transform.position, 
                    huntSpeed * Time.deltaTime);
        }

        if (!spotted) ChangeState(EnemyStates.Idle);
    }

    private void Idle()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            if (raycastOffset < 0.1 && raycastOffset > 0) idleToPatroltimer = 1.0f;
            ChangeState(EnemyStates.Patrol);
        }
        
        if (spotted) ChangeState(EnemyStates.Detect);
    }

    private void Patrol()
    {
        if (raycastOffset < 0.1 && raycastOffset > 0) idleToPatroltimer -= Time.deltaTime;
        
        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(direction * raycastOffset, 0.3f);

        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));

        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, rayHit.collider ? Color.green : Color.red);

        if (idleToPatroltimer <= 0 || (raycastOffset < 0.1 && raycastOffset > 0))
        {
            if (!rayHit.collider)
            {
                direction *= -1;
                ChangeState(EnemyStates.Idle);
            }
        }

        transform.Translate(Vector2.right * (direction * patrolSpeed * Time.deltaTime));

        if (spotted) ChangeState(EnemyStates.Detect);

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Vector2 rayOrigin = (Vector2)detectionZone.transform.position + new Vector2(detectionZone.transform.position.x/2 * direction,0);
            
            
            
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, (detectionZone.transform.position - other.transform.position).normalized, 
                Vector2.Distance(rayOrigin, other.transform.position), LayerMask.GetMask("Player"));
            
            //TODO Debug.DrawRay(rayOrigin, );
            if (rayHit.collider)
            {
                
            }
            
        }
    }
    
    void Hunt()
    {
    
    }
    

    private void ChangeState(EnemyStates nextState)
    {
        currentstate = nextState;

        if (nextState == EnemyStates.Idle)
        {
            idleTimer = 1.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // Statecheck of the enemy, easy to add new functions and states
        switch (currentstate)
        {
            case EnemyStates.Idle:
                Idle();
                break;
            
            case EnemyStates.Patrol:
                Patrol();
                break;
            
            case EnemyStates.Detect:
                Detect();
                break;
            
        }
        //TODO: if removed from original platform, try to search for a new underneath w GetComponent and using the Layermask Ground
    }
}
