using System.Collections;
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

    [Tooltip("This is the object which is the outer bounds for the enemy(outer detection)")] 
    [SerializeField] private GameObject OuterDetectionZone;

    [SerializeField] private Transform test;

    [SerializeField] private Collider2D playerhitbox;
    
    private CircleCollider2D OuterDetectCircle => OuterDetectionZone.GetComponent<CircleCollider2D>();
    

    private float idleTimer = 2.0f;

    private float detectionholdupTime = 1.0f;

    private float detectionHuntTime = 5.0f;

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
        detectionholdupTime -= Time.deltaTime;
        //if (detectionholdupTime <= 0 && Physics2D.OverlapCircle(OuterDetectCircle.transform.position, OuterDetectCircle.radius, 7 ) )
        if (OuterDetectCircle.IsTouching(playerhitbox))
        {
            Debug.Log("detected");
            Vector2 tryToMove = Vector2.MoveTowards(
                    transform.position, 
                    player.transform.position, 
                    huntSpeed * Time.deltaTime);
            
        }
        /*else
        {
            Debug.Log(" not detected");
            ChangeState(EnemyStates.Idle);
            spotted = false;
        }*/
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


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            Vector2 rayOrigin = new Vector2(test.position.x, test.position.y);
            
            Vector2 rayDir = (other.transform.position - detectionZone.transform.position).normalized;

            float rayDistance = Vector2.Distance(rayOrigin, other.transform.position);
            
            RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, rayDir, rayDistance, 
                LayerMask.GetMask("Ground"));
            
            Debug.DrawRay(rayOrigin, rayDir * rayDistance, rayHit.collider ? Color.green : Color.red);
            
            if (!rayHit.collider)
            {
                spotted = true;
                ChangeState(EnemyStates.Detect);
            }
            else spotted = false;
        }
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
