using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball_script : MonoBehaviour
{

    [SerializeField] private LayerMask thickPlatformLayerMask;
    [SerializeField] private float selfDestructTimer;
    private bool directionSet = false;
    private float velocityX = 0f;
    private float velocityY = 0f;
    private float fireBallSpeed = 0f;
    private float selfDestruct = 10f;
    private BoxCollider2D boxCollider;
    private Animator ani;



    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        ani = GetComponent<Animator>();
        selfDestruct = selfDestructTimer;
    }

    // Update is called once per frame
    void Update()
    {

        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, thickPlatformLayerMask);//checks if the fireball has hit a thickPlatform

        //kills itself after x seconds (determined by selfDestructTimer)
        selfDestruct = selfDestruct - Time.deltaTime;
        if(selfDestruct <= 0)
            Destroy(gameObject);

        
        if (collidedPlatform != null)
        {
            ani.SetTrigger("explode");
            Destroy(gameObject, 0.5f);
            
        }
        else {
            if (directionSet == true)
            {
                transform.position = transform.position + new Vector3(Time.deltaTime * velocityX * fireBallSpeed, Time.deltaTime * velocityY * fireBallSpeed, 0);//moves fireball to new spot every frame based on the passed fireballSpeed and direction
            }

        }
    }

    public void setFireBallDirection(float velX, float velY, float speed)
    {
        velocityX = velX;
        velocityY = velY;
        fireBallSpeed = speed;
        directionSet = true;
    }
}



