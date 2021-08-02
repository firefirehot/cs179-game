using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball_shooter2 : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject fireballObject;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float distToStartFireing;
    [SerializeField] private float fireBallFireRate;
    [SerializeField] private float fireBallSpeed;
    private BoxCollider2D boxCollider;
    private float timeToNextFireBall;
    private cameraScript cameraObject;

    void Start()
    {
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraScript>();
        boxCollider = GetComponent<BoxCollider2D>();
        timeToNextFireBall = fireBallFireRate;
    }

    // Update is called once per frame
    void Update()
    {
        fireBallController();
    }



    void fireBallController()
    {
        // Collider2D collidedPlatform = Physics2D.OverlapBox(cameraObject.transform.position, new Vector2(distToStartFireing, distToStartFireing), 0f, playerLayerMask);

        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(cameraObject.transform.position.x, cameraObject.transform.position.y),
             new Vector2(boxCollider.bounds.size.x, distToStartFireing), 0f, Vector2.left, distToStartFireing, playerLayerMask);

        if (raycastHit.collider != null)
        {

            timeToNextFireBall = timeToNextFireBall - Time.deltaTime;
            if (timeToNextFireBall <= 0)
            {
                timeToNextFireBall = fireBallFireRate;
                //shootFireBall((raycastHit.point.x - transform.position.x) / raycastHit.distance, (raycastHit.point.y - transfrom.position.y) / raycastHit.distance);
            }
        }
        else
        {
            timeToNextFireBall = fireBallFireRate;
        }

    }//

    void shootFireBall(float directionX, float directionY)
    {
        GameObject fireballInstance = Instantiate(fireballObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(0f, 0f, 0f));
        fireballInstance.GetComponent<fireball_script>().setFireBallDirection(directionX, directionY, fireBallSpeed);
    }
}
