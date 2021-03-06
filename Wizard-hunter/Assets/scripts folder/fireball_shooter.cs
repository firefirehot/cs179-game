using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball_shooter : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject fireballObject;
    [SerializeField] private LayerMask playerLayerMask;
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
        }

    }//

    void shootFireBall(float directionX, float directionY) {
        GameObject fireballInstance = Instantiate(fireballObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(0f, 0f, 0f));//create a fireball object at the position of the shooter
        fireballInstance.GetComponent<fireball_script>().setFireBallDirection(directionX, directionY,fireBallSpeed);//give the fireball it's speed and it's direction
    }
}
