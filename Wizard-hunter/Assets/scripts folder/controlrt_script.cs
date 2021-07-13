using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//credit to unity documentation https://docs.unity3d.com/ScriptReference/Rigidbody2D-velocity.html
//credit to youtube video by samyam https://www.youtube.com/watch?v=3Ad1wr3qBRw&t=189s   and   https://www.youtube.com/watch?v=1Ll1fy2EehU  and  https://www.youtube.com/watch?v=on9nwbZngyw&t=376s
//crdit to youtube video by Coding With Unity https://www.youtube.com/watch?v=VHYke1HrlMY
//credit to youtube video by Brackeys https://www.youtube.com/watch?v=gAB64vfbrhI and https://www.youtube.com/watch?v=VbZ9_C4-Qbo
//credit to youtube video by Code Monkey https://www.youtube.com/watch?v=c3iEl5AwUF8&t=659s

public class controlrt_script : MonoBehaviour
{//
    // Start is called before the first frame update
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask trapLayerMask;
    [SerializeField] private LayerMask thickPlatformLayerMask;
    [SerializeField] private LayerMask thinPlatformLayerMask;

    //private GameObject hpObject; //current added
    //private Image spikeObjectImage; //current added
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    SpriteRenderer m_SpriteRenderer; //empty sprite render container
    private Animator ani;// empty animator container, animator decides which animation to play
    
    /*MOVEMENT VARS***********************************************************************************/
    //jumps, walljumps
    private float jumpHeight = 7.5f;//changes the height of the jump by giving it more velocity
    private float wallJumpVelocityX = 4f; // changes velocity of player after wall Jumping
    private float wallJumpVelocityY = 5f; // changes velocity of player after wall Jumping
    private bool canAirJump = true;//set to true when u want to enable a second jump along with prepDoubleJump
    private float fallSpeed = 10f; // changes the player's fall rate while not touching ground or platforms

    //running speed vars
    private float maxSpeed = 5f; //sets a limit in how much velocity the player can add to themselves using the LR inputs
    private float playerAcceleration = 8f; //determines how fast the player will accelerate
    private float slowSpeed = 4f; // changes how fast the player will slow down due to touching a floor or platform(FWI RigidBody2D friction is also slowing the player so slowSpeed == 0 does not mean the player will never stop)

    //dash vars
    private float lastInput = 1;//records the direction of the last LR input(used for dashing and wall jumping)
    private float dashRange = 20f; //changes how far the dash will go
    private float dashLenency = 0.3f; // this is how much time between button presses the player has in order to dash
    private float dashCatcher; // records time between LR button presses. If the player presses the same direction twice in a short amount of time(determined by dashLenency) the player dashes.
    private float dashLock = 0f;//locks out any other input while dashing. when dashLock == 0 user inputs are accepted
    private float dashLockSet = 0.15f;//sets the dash lock to prevent user input for 0.15sec
    private bool canAirDash = true; // locks out dash when false. Alows us to only allow one dash per jump
    private float saveVelocity = 5f;//save's the velocity before a dash so that the player can return to the saved velocity after the dash.
    private float minVelocityAfterDash = 3f;//sets the minimum velocity the player will have after the dash.
    /*END OF MOVEMENT VARS*****************************************************************************/

    //gets user inputs
    private float inputLR;//current left or right input. NOT to be confused with "Input" which is defined in unity.
    private float inputUD;//current up or down input. NOT to be confused with "Input" which is defined in unity.

    //invinsibility variables
    private float setInvincibility = 1.5f;
    private float invincibilityAfterHit = 0;

    //Scripts in the form of objects. Used to call functions from those scripts.
    my_hp_script hp_scriptObject;
    cameraScript cameraObject;

    //other used var's that don't have a neat catagory
    private bool facingRight = true; // A boolian value that is true if the player is facing right

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // assigns empty container with the actual component
        facingRight = true; 
        ani = GetComponent<Animator>(); // assigns empty container with the actual component
        boxCollider = GetComponent<BoxCollider2D>();
        hp_scriptObject = GameObject.FindGameObjectWithTag("HP Bar").GetComponent<my_hp_script>();
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraScript>();
        //spikeObjectImage = hpObject.GetComponent<Image>();//current added

    }

    // Update is called once per frame



    void Update()
    {

        bool groundedBool = isGrounded();//ground bool is true if player is against an object labled ground
        bool wallBool = isAgainstWall();//wallBool is true if player is against an object labeled wall
        int interactionStatus = 0;
        bool platformBool = isTouchingThickPlatform(ref interactionStatus);//platform bool is true if we are touching a platform, interactionStatus is 0 for walls, 1 for floors, 2 for ceiling
        if (platformBool != true)
        {
            platformBool = isTouchingThinPlatform();
            if (platformBool == true)
                interactionStatus = 1;
        }

        cameraObject.upDateCamera(transform.position.x, transform.position.y);//sends the player's position to the camera function called upDateCamera. upDateCamera moves then camera to passed position


        if (dashLock <= 0)
        {
            ani.SetBool("dashing", false);
            //get user input code block
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
            //***


            //player LR input turns into movement here.
            if ((rb.velocity.x < maxSpeed && rb.velocity.x > -maxSpeed))// prevents player from increasing running speed forever
            {
                rb.velocity = rb.velocity + new Vector2(inputLR * Time.deltaTime * playerAcceleration, 0);// accelerates the player if they give a LR input

            }
            //**


            //code block that handles air variables and fall speed
            if (groundedBool || (platformBool && interactionStatus == 1))//if player is on the ground or a platform we set these variables to true
            {
                canAirJump = true;
                canAirDash = true;

            }
            else
            {//if player is not on the ground or on a platform we give the player downwards velocity
                rb.velocity = rb.velocity + new Vector2(0f, -Time.deltaTime * fallSpeed);//THIS IS THE GRAVITY

            }
            //**


            //flip player if they change direction code
            if (inputLR < 0 && facingRight == true || inputLR > 0 && facingRight == false)//flips the sprite if the player inputs a new direcition
            {//flips the sprite if we get a input in the opposite direction that the sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                facingRight = !facingRight;//records the direction sprite is facing
            }
            //**


            //code that handles LR input animation and variables
            if (inputLR == 0)//if we don't detect a LR(left or right) input and want to play idle animation. If we are in idle animation we create a friction force.
            {//changes animation to idle if no input is detected

                dashCatcher = dashCatcher + Time.deltaTime;//keeps track of the how much time it has been since no input has been detected
                if (groundedBool || (platformBool && interactionStatus == 1))//if we are on the floor we want to come to a stop since we no input is being given
                {
                    ani.SetBool("isRunning", false);
                    rb.velocity = new Vector2(rb.velocity.x + (-rb.velocity.x * Time.deltaTime * slowSpeed), rb.velocity.y);
                }

            }
            else if (inputLR != 0)//we detect LR input and want to decide if we play running animation or dashing animation
            {
                if ((dashCatcher < dashLenency && dashCatcher > 0.0001 && ((lastInput > 0 && inputLR > 0) || (lastInput < 0 && inputLR < 0))) && canAirDash)//if a second input LR(left or right) is detected within 0.001s->0.3s then we dash
                {
                    ani.SetBool("isRunning", false);
                    ani.SetBool("dashing", true);
                    dashLock = dashLockSet;//sets the dashLock to prevent user input for 0.2 seconds
                    if (!groundedBool)
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
            //**


            //single and double jumping code
            if (inputUD == 1 && (groundedBool || (platformBool && interactionStatus == 1)))//if player is on the ground or on a platform they can jump
            {//enables jump if player is on the ground and they press up
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);//this determines the speed at which pressing up will propell you at

                //prepDoubleJump = true;//enables the double jump after the first jump
            }
            else if (inputUD == 1 && canAirJump)//if player has a air jump they can jump, then we disable the air jump
            {//enables jump if player has not double jumped yet and they press up
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);//this determines the speed at which pressing up will propell you at
                canAirJump = false;//disables any jumps in air
            }

            if (!(groundedBool || (platformBool && interactionStatus == 1)))//if we are not on the floor we want to play the jump animations
            {
                ani.SetBool("inAir", true);
                //ani.SetBool("wallJump", false);
                if (rb.velocity.y > 0)
                {
                    ani.SetBool("velocityIsPositive", true);
                }
                else
                {
                    ani.SetBool("velocityIsPositive", false);
                }
            }
            else
            {
                ani.SetBool("inAir", false);
            }
            //**


            //code that handles wall jumping
            if (((wallBool || (platformBool && interactionStatus == 0)) && !groundedBool && inputUD == 1))//activates if player is against a wall and is not on the floor and the player pressed up. This will overwrite any velocity the player had before.
            {

                inputUD = 0;
                if (lastInput > 0)
                {
                    rb.velocity = new Vector2(-wallJumpVelocityX, wallJumpVelocityY);//gives player a new velocity of -4x and 3y after wall jumping
                    ani.SetTrigger("wallJump"); //wall jump animation plays

                }
                if (lastInput < 0)
                {
                    rb.velocity = new Vector2(wallJumpVelocityX, wallJumpVelocityY); //gives player a new velocity of 4x and 3y after wall jumping
                    //ani.SetTrigger("wallJump_unflipped");//flipped version of the wall jump animation plays
                    ani.SetTrigger("wallJump");
                }
                facingRight = !facingRight;//records the direction sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                canAirDash = true;//enable air dash
                canAirJump = true;//enable double jump 


            }
            //**



        }
        else if (dashLock > 0)//code that handles dashing
        {
            if (lastInput > 0)
                transform.position = transform.position + new Vector3(Time.deltaTime * dashRange, 0, 0);//this is the dash movement if looking right
            else
                transform.position = transform.position + new Vector3(-Time.deltaTime * dashRange, 0, 0);//this is the dash movement if looking left

            dashLock = dashLock - Time.deltaTime;//decrements the dashlock until 0.2 seconds have passed
            if (dashLock <= 0)//when the dashlock is over this if statment gives the player back their velocity that they had before the dash
            {
                if (saveVelocity < -minVelocityAfterDash && saveVelocity > minVelocityAfterDash)
                    rb.velocity = new Vector2(saveVelocity, 0);
                else
                {
                    rb.velocity = new Vector2(lastInput * minVelocityAfterDash, 0);
                }
            }
        }//end of dashlock


        //code that handles invincibility after being hit and player touching something that damages them by half of a heart.
        if (invincibilityAfterHit > 0f)
        {
            invincibilityAfterHit = invincibilityAfterHit - Time.deltaTime;

        }
        else if (isTouchingTrap())
        {
            
            hp_scriptObject.halfHeartDamage();
            invincibilityAfterHit = setInvincibility;//sets how long the player is invisible for after being hit
        }
        //**

    }//end of update

    float AbsoluteValueFloat(float mathIn)
    {
        if (mathIn > 0f)
        {
            return mathIn;
        }
        return mathIn * -1f;
    }


    bool isGrounded() {//credit to Code Monkey
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        return raycastHit.collider != null;//returns true if it collided, false if it didn't
    }

    bool isTouchingTrap() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.05f, 0f, Vector2.down, 0f, trapLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        
        return raycastHit.collider != null;//returns true if it collided, false if it didn't
    }

    bool isTouchingThickPlatform(ref int objectTouched)
    {//returns true if the object is touching a platform and returns 0 if player is on top of the platform, 1 if player is below platform, 2 if player is next to platform, and -1 if player is not touching anything
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size * 1.05f, 0f, Vector2.down, 0f, thickPlatformLayerMask);
        //public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);


        float boxColliderCenterX = transform.position.x + boxCollider.offset.x * transform.lossyScale.x;
        float boxColliderCenterY = transform.position.y + boxCollider.offset.y * transform.lossyScale.y;
        float boxColliderSizeX = boxCollider.size.x * transform.lossyScale.x / 2f;
        float boxColliderSizeY = boxCollider.size.y * transform.lossyScale.y / 2f;
        float mathLenency = 0.05f;
        ;
        if (raycastHit.collider == null)
        {
            objectTouched = -1;
            return false;
        }
        else if ((boxColliderCenterY - raycastHit.point.y > boxColliderSizeY - mathLenency) && (boxColliderCenterY - raycastHit.point.y < boxColliderSizeY + mathLenency))
        {//determines if the ray hit a point below the player
            objectTouched = 1; //1 will be floor
        }
        else if (((AbsoluteValueFloat(raycastHit.point.x - boxColliderCenterX) > boxColliderSizeX - mathLenency) && (AbsoluteValueFloat(raycastHit.point.x - boxColliderCenterX) < boxColliderSizeX + mathLenency)))
        {
            //determines if the ray hit a point next to the player
            objectTouched = 0; //0 will be wall
        }
        else//otherwise the point is above the player
            objectTouched = 2; // 2 will be ceiling

        return true;

    }

    bool isTouchingThinPlatform()
    {//returns true if the object is touching a platform and returns 0 if player is on top of the platform, 1 if player is below platform, 2 if player is next to platform, and -1 if player is not touching anything
        //RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.01f, 0f, Vector2.down, 0f, thinPlatformLayerMask);
        //RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.03f, thinPlatformLayerMask);
        float mathLenency = 0.1f;
        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size + new Vector3(mathLenency, mathLenency, 0f), 0f, thinPlatformLayerMask);
        float playerColliderX = boxCollider.bounds.center.x;
        float playerColliderY = boxCollider.bounds.center.y;
        float playerColliderSizeX = boxCollider.bounds.size.x / 2f;
        float playerColliderSizeY = boxCollider.bounds.size.y / 2f;


        if (collidedPlatform == null)
        {
            return false;
        }
        else if (((playerColliderY + mathLenency > playerColliderSizeY + collidedPlatform.bounds.size.y / 2f + collidedPlatform.bounds.center.y) && (playerColliderY - mathLenency < playerColliderSizeY + collidedPlatform.bounds.size.y / 2f + collidedPlatform.bounds.center.y)) &&
            ((playerColliderSizeX + playerColliderX > collidedPlatform.bounds.center.x - collidedPlatform.bounds.size.x / 2f) && (playerColliderX - playerColliderSizeX < collidedPlatform.bounds.center.x + collidedPlatform.bounds.size.x / 2f)))
        {//determines if the ray hit a point below the player
            if (inputUD != -1)
            {
                collidedPlatform.isTrigger = false;//player can't fall through on false
                return true;
            }
            else
            {
                collidedPlatform.isTrigger = true;
                return false;
            }
        }
        else
        {
            collidedPlatform.isTrigger = true;
            return false;
        }
    }

    bool isAgainstWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.1f, 0f, Vector2.right, 0f, wallLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        
        
        return raycastHit.collider != null;
    }





}
