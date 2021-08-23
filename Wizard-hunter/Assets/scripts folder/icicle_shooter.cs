using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icicle_shooter : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject IceballObject;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask unactivatedTrapLayerMask;
    [SerializeField] private float distToStartIceing;
    [SerializeField] private float IceBallIceRate;
    [SerializeField] private float IceBallIceRateFinal;
    [SerializeField] private float IceBallSpeed;

    private BoxCollider2D boxCollider;
    private float timeToNextIceBall;


    void Start()
    {

        boxCollider = GetComponent<BoxCollider2D>();
        timeToNextIceBall = IceBallIceRate;
    }

    // Update is called once per frame
    void Update()
    {
        IceBallController();
    }



    void IceBallController()
    {

        /*RaycastHit2D[] turnOffArray = new RaycastHit2D[15];
        int numberOfHits = Physics2D.BoxCastNonAlloc(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.size.y * 2f),
            new Vector2(boxCollider.bounds.size.x * 2.5f, boxCollider.bounds.size.y), 0f, Vector2.up, turnOffArray, boxCollider.bounds.size.y * 4f, thinPlatformLayerMask);
        */

        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + distToStartIceing), new Vector2(distToStartIceing,boxCollider.bounds.size.x ), 0f, Vector2.down, distToStartIceing*2f, playerLayerMask);//detects the player in a square left of the wizard that is distToStartIceing in length

        if (GetComponent<BezierFollow>().currentRoute == 15)
        {
            IceBallIceRate = IceBallIceRateFinal;
        }

        if (raycastHit.collider != null)//if player detected
        {

            timeToNextIceBall = timeToNextIceBall - Time.deltaTime;//count down timer to Ice next Iceball
            if (timeToNextIceBall <= 0)//if timer at 0
            {
                timeToNextIceBall = IceBallIceRate;//reset timer
                shootIceBall((raycastHit.point.x - transform.position.x) / (raycastHit.point- new Vector2(transform.position.x, transform.position.y)).magnitude, (raycastHit.point.y - transform.position.y) / (raycastHit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);//shoot Iceball in direction of detected player
            }
        }
        else
        {
            timeToNextIceBall = IceBallIceRate;//if player is out of range then reset the time to next Iceball

            //if player outside of Ice ball range he starts to activate traps instead
            RaycastHit2D unactivatedTrapRaycastHit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + distToStartIceing), new Vector2(distToStartIceing, boxCollider.bounds.size.x), 0f, Vector2.down, distToStartIceing * 2f, unactivatedTrapLayerMask);//detect unactivated traps
            if (unactivatedTrapRaycastHit.collider != null)
            {
                unactivatedTrapRaycastHit.transform.gameObject.layer = 9;//if unactivatedTrap detected then we change it's layer to trap. The trap itself will handle what it needs to do to "activate"
            }
        }

    }//

    void shootIceBall(float directionX, float directionY)
    {
        GameObject IceballInstance = Instantiate(IceballObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(0f, 0f, 0f));//create a Iceball object at the position of the shooter
        IceballInstance.GetComponent<icicle_script>().setIceDirection(directionX, directionY, IceBallSpeed);//give the Iceball it's speed and it's direction
    }
}

