using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball_shooter : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject fireballObject;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask unactivatedTrapLayerMask;
    [SerializeField] private float distToStartFireing;
    [SerializeField] private float fireBallFireRate;
    [SerializeField] private float fireBallSpeed;

    private BoxCollider2D boxCollider;
    private float timeToNextFireBall;


    void Start()
    {

        boxCollider = GetComponent<BoxCollider2D>();
        timeToNextFireBall = fireBallFireRate;
    }

    // Update is called once per frame
     void Update()
    {
        fireBallController();
    }



    void fireBallController() {

        /*RaycastHit2D[] turnOffArray = new RaycastHit2D[15];
        int numberOfHits = Physics2D.BoxCastNonAlloc(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.size.y * 2f),
            new Vector2(boxCollider.bounds.size.x * 2.5f, boxCollider.bounds.size.y), 0f, Vector2.up, turnOffArray, boxCollider.bounds.size.y * 4f, thinPlatformLayerMask);
        */

        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, new Vector2(boxCollider.bounds.size.x, distToStartFireing), 0f, Vector2.left, distToStartFireing, playerLayerMask);//detects the player in a square left of the wizard that is distToStartFireing in length


        if (raycastHit.collider != null)//if player detected
        {

            timeToNextFireBall = timeToNextFireBall - Time.deltaTime;//count down timer to fire next fireball
            if (timeToNextFireBall <= 0)//if timer at 0
            {
                timeToNextFireBall = fireBallFireRate;//reset timer
                shootFireBall((raycastHit.point.x - transform.position.x) / raycastHit.distance, (raycastHit.point.y - transform.position.y) / raycastHit.distance);//shoot fireball in direction of detected player
            }
        }
        else
        {
            timeToNextFireBall = fireBallFireRate;//if player is out of range then reset the time to next fireball

            //if player outside of fire ball range he starts to activate traps instead
            RaycastHit2D unactivatedTrapRaycastHit = Physics2D.BoxCast(transform.position, new Vector2(boxCollider.bounds.size.x, distToStartFireing*2f), 0f, Vector2.left, distToStartFireing*0.6f, unactivatedTrapLayerMask);//detect unactivated traps
            if (unactivatedTrapRaycastHit.collider != null)
            {
                unactivatedTrapRaycastHit.transform.gameObject.layer = 9;//if unactivatedTrap detected then we change it's layer to trap. The trap itself will handle what it needs to do to "activate"
            }
        }

    }//

    void shootFireBall(float directionX, float directionY) {
        GameObject fireballInstance = Instantiate(fireballObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(0f, 0f, 0f));//create a fireball object at the position of the shooter
        fireballInstance.GetComponent<fireball_script>().setFireBallDirection(directionX, directionY,fireBallSpeed);//give the fireball it's speed and it's direction
    }
}
