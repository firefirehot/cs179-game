using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//credit to unity documentation https://docs.unity3d.com/ScriptReference/Rigidbody2D-velocity.html
//credit to youtube video by samyam https://www.youtube.com/watch?v=3Ad1wr3qBRw&t=189s   and   https://www.youtube.com/watch?v=1Ll1fy2EehU  and  https://www.youtube.com/watch?v=on9nwbZngyw&t=376s
//crdit to youtube video by Coding With Unity https://www.youtube.com/watch?v=VHYke1HrlMY
//credit to youtube video by Brackeys https://www.youtube.com/watch?v=gAB64vfbrhI
//credit to youtube video by Code Monkey https://www.youtube.com/watch?v=c3iEl5AwUF8&t=659s

public class controlrt_script : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LayerMask platformLayerMask;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private float movementSpeed = 2f; // player movement speed, change this to slow/speed the player up. This does not change the animation at all
    private bool facingRight = true; // A boolian value that is true if the player is facing right
    SpriteRenderer m_SpriteRenderer; //empty sprite render container
    private Animator ani;// empty animator container, animator decides which animation to play
    private float dashCatcher; // records time. If the player presses the same direction twice in a short amount of time the player dashes.
    private float lastInput = 1;//records the direction of the last input
    private float dashLock = 0;//locks out any other input while dashing. 
    private float inputLR;//current left or right input. NOT to be confused with "Input" which is defined in unity.
    private float inputUD;//current up or down input. NOT to be confused with "Input" which is defined in unity.
    private float timeRunning = 0f;
    private bool canAirJump = true;
    private bool canAirDash = true;
    //private int dashPlayer = 3;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // assigns empty container with the actual component
        facingRight = true; 
        ani = GetComponent<Animator>(); // assigns empty container with the actual component
        boxCollider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame



    void Update()
    {
        
        if (dashLock <= 0)
        {
            ani.SetBool("dashing", false);

            if (Input.GetKey("d"))
                inputLR = 1;
            else if (Input.GetKey("a"))
                inputLR = -1;
            else
                inputLR = 0;

            if (Input.GetKeyDown("w"))
                inputUD = 1;
            else if (Input.GetKey("s"))
                inputUD = -1;
            else
                inputUD = 0;

            transform.position = transform.position + new Vector3(inputLR * Time.deltaTime * movementSpeed, 0, 0);//moves the character by getting the characters current position and 
                                                                                                                  //adding "input*Time.deltaTime*movementSpeed"(direction = inputLR, Time.deltaTime = time since last frame, movementSpeed multiplier)


            if (isGrounded()) {
                canAirJump = true;
                canAirDash = true;
            }
            if (inputUD == 1 && isGrounded())//enables jump if player is on the ground and they press up
                rb.velocity = new Vector2(0f, 5f);//this determines the speed at which pressing up will propell you at
            else if(inputUD == 1 && canAirJump) {//enables jump if player has not double jumped yet and they press up
                rb.velocity = new Vector2(0f, 5f);//this determines the speed at which pressing up will propell you at
                canAirJump = false;
            }

            if (inputLR < 0 && facingRight == true || inputLR > 0 && facingRight == false)
            {//flips the sprite if we get a input in the opposite direction that the sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                facingRight = !facingRight;//records the direction sprite is facing

            }
            if (inputLR == 0)
            {//changes animation to idle if no input is detected
                ani.SetBool("isRunning", false);
                dashCatcher = dashCatcher + Time.deltaTime;//keeps track of the how much time it has been since no input has been detected
                resetMovementSpeed();//resets movement speed

            }
            else if (inputLR != 0)
            {// changes animation to running if an input is detected

                if ((dashCatcher < 0.3 && dashCatcher > 0.0001 && ((lastInput > 0 && inputLR > 0) || (lastInput < 0 && inputLR < 0))) && canAirDash)
                {
                    ani.SetBool("isRunning", false);
                    ani.SetBool("dashing",true);
                    dashLock = 0.2F;//sets the dashLock to prevent user input for 0.2 seconds
                    if (!isGrounded())
                        canAirDash = false;
                }
                else
                    ani.SetBool("isRunning", true);
                lastInput = inputLR;
                dashCatcher = 0F;
                increaseMovementSpeed();//increases movement speed to a max of 5f to immitate momentum
            }
        }
        else {
            if (lastInput > 0)
                transform.position = transform.position + new Vector3(Time.deltaTime*20F, 0, 0);//
            else
                transform.position = transform.position + new Vector3(-Time.deltaTime * 20F, 0, 0);
            movementSpeed = 3f;
            dashLock = dashLock - Time.deltaTime;//decrements the dashlock until 0.2 seconds have passed
        }






    }

    bool isGrounded() {//credit to Code Monkey
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 2f, platformLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        return raycastHit.collider != null;//returns true if it collided, false if it didn't
    }


    void increaseMovementSpeed()
    {
        if (movementSpeed < 5f) {
            movementSpeed = movementSpeed + ((Time.deltaTime*timeRunning)*5 / 3f);//increases movementspeed over ~3 seconds (acomplished using testing, not math)
            timeRunning = timeRunning + Time.deltaTime;
        }
        else
        {
            movementSpeed = 5f;//max movementspeed will be 5
        }
    }
    void resetMovementSpeed() {// reset mmovement speed so that player will need to rebuild it.
        movementSpeed = 2f;
        timeRunning = 0;
    }


}
