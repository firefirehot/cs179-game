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
    [SerializeField] private LayerMask wallLayerMask;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
  
    private bool facingRight = true; // A boolian value that is true if the player is facing right
    SpriteRenderer m_SpriteRenderer; //empty sprite render container
    private Animator ani;// empty animator container, animator decides which animation to play
    private float dashCatcher; // records time. If the player presses the same direction twice in a short amount of time the player dashes.
    private float lastInput = 1;//records the direction of the last input
    private float dashLock = 0;//locks out any other input while dashing. 
    private float inputLR;//current left or right input. NOT to be confused with "Input" which is defined in unity.
    private float inputUD;//current up or down input. NOT to be confused with "Input" which is defined in unity.
    

    private bool canAirJump = true;//set to true when u want to enable a second jump along with prepDoubleJump
    private bool prepDoubleJump = false; //set to true when u want to enable a second jump along with canAirJump

    private bool canAirDash = true; // set to true when u want to enable a dash

    private float saveVelocity = 5f;
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


            if (rb.velocity.x < 5f && rb.velocity.x > -5f)
                rb.velocity = rb.velocity + new Vector2(inputLR * Time.deltaTime * 10, 0);

            if (isGrounded())
            {
                canAirJump = true;
                canAirDash = true;
            }


            if (inputUD == 1 && isGrounded() && prepDoubleJump == false)
            {//enables jump if player is on the ground and they press up
                rb.velocity = new Vector2(rb.velocity.x, 5f);//this determines the speed at which pressing up will propell you at
                prepDoubleJump = true;//enables the double jump after the first jump
            }
            else if (inputUD == 1 && canAirJump && prepDoubleJump == true)
            {//enables jump if player has not double jumped yet and they press up
                rb.velocity = new Vector2(rb.velocity.x, 5f);//this determines the speed at which pressing up will propell you at
                canAirJump = false;//disables any jumps in air
                prepDoubleJump = false;//disables the double jump
            }

            if (inputLR < 0 && facingRight == true || inputLR > 0 && facingRight == false)
            {//flips the sprite if we get a input in the opposite direction that the sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                facingRight = !facingRight;//records the direction sprite is facing
            }


            if (inputLR == 0)//if we don't detect a LR(left or right) input we enter this loop and play the idle animation
            {//changes animation to idle if no input is detected
                ani.SetBool("isRunning", false);
                dashCatcher = dashCatcher + Time.deltaTime;//keeps track of the how much time it has been since no input has been detected
                if (isGrounded())//if we are on the floor we want to come to a stop since we no input is being given
                {
                    rb.velocity = new Vector2(rb.velocity.x + (-rb.velocity.x * Time.deltaTime * 4f), rb.velocity.y);
                }

            }
            else if (inputLR != 0)
            {// changes animation to running if an input is detected
                if ((dashCatcher < 0.3 && dashCatcher > 0.0001 && ((lastInput > 0 && inputLR > 0) || (lastInput < 0 && inputLR < 0))) && canAirDash)//if a second input LR(left or right) is detected within 0.001s->0.3s then we dash
                {
                    ani.SetBool("isRunning", false);
                    ani.SetBool("dashing", true);
                    dashLock = 0.15F;//sets the dashLock to prevent user input for 0.2 seconds
                    if (!isGrounded())
                        canAirDash = false;
                }
                else//if we don't dash we are running at this point
                {
                    ani.SetBool("isRunning", true);
                    saveVelocity = rb.velocity.x;
                }
                lastInput = inputLR;
                dashCatcher = 0F;
                
            }


            if (isAgainstWall() && !isGrounded() && inputUD == 1)//activates if player is against a wall and is not on the floor and the player pressed up. This will overwrite any velocity the player had before.
            {
                
                inputUD = 0;
                if (lastInput > 0)
                {
                    rb.velocity = new Vector2(-4f, 3f);//gives player a new velocity of -4x and 3y after wall jumping
                    ani.SetTrigger("wallJump"); //wall jump animation plays

                }
                if (lastInput < 0)
                {
                    rb.velocity = new Vector2(4f, 3f); //gives player a new velocity of 4x and 3y after wall jumping
                    ani.SetTrigger("wallJump_unflipped");//flipped version of the wall jump animation plays
                }
                facingRight = !facingRight;//records the direction sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                canAirDash = true;//enable air dash
                canAirJump = true;//enable double jump (part1)
                prepDoubleJump = true;//enable double jump(part2)
                
                
            }


        }
        else if (dashLock > 0)
        {
            if (lastInput > 0)
                transform.position = transform.position + new Vector3(Time.deltaTime * 20F, 0, 0);//this is the dash movement if looking right
            else
                transform.position = transform.position + new Vector3(-Time.deltaTime * 20F, 0, 0);//this is the dash movement if looking left

            dashLock = dashLock - Time.deltaTime;//decrements the dashlock until 0.2 seconds have passed
            if (dashLock <= 0)//when the dashlock is over this if statment gives the player back their velocity that they had before the dash
            {
                if (saveVelocity < -3f && saveVelocity > 3f)
                    rb.velocity = new Vector2(saveVelocity, 0);
                else
                {
                    rb.velocity = new Vector2(lastInput * 3, 0);
                }
            }
        }//end of dashlock






    }//end of update

    bool isGrounded() {//credit to Code Monkey
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        return raycastHit.collider != null;//returns true if it collided, false if it didn't
    }

    bool isAgainstWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.1f, 0f, Vector2.right, 0f, wallLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        
        //return (raycastHit.collider != null || raycastHit2 != null);//returns true if it collided, false if it didn't
        return raycastHit.collider != null;
    }





}
