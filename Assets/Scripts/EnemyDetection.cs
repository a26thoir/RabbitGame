using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Tooltip("This is where you insert the player/object you want detected")] 
    [SerializeField] private GameObject player;

    [Tooltip("This is where you insert the object for boundaries")] 
    [SerializeField] private Bounds boundObject;


    [Tooltip("This is the speed in the patrol state")] [SerializeField]
    public float patrolspeed = 2.0f;

    private Vector2 boundsLeft => new Vector2(boundObject.min.x, boundObject.min.y);
    private Vector2 boundsRight => new Vector2(boundObject.max.x, boundObject.min.y);

    private EnemyStates currentstate = EnemyStates.Idle;
    
    public enum EnemyStates
    {
        Idle,
        Patrol,
        Detect
    }

    private void Detect()
    {
        
    }

    private void Idle()
    {
        
    }

    private void Patrol()
    {
        
        
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
        
    }
}
