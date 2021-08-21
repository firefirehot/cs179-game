using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//credit to unity documentation https://docs.unity3d.com/ScriptReference/Rigidbody2D-velocity.html
//credit to youtube video by samyam https://www.youtube.com/watch?v=3Ad1wr3qBRw&t=189s   and   https://www.youtube.com/watch?v=1Ll1fy2EehU  and  https://www.youtube.com/watch?v=on9nwbZngyw&t=376s
//crdit to youtube video by Coding With Unity https://www.youtube.com/watch?v=VHYke1HrlMY
//credit to youtube video by Brackeys https://www.youtube.com/watch?v=gAB64vfbrhI and https://www.youtube.com/watch?v=VbZ9_C4-Qbo
//credit to youtube video by Code Monkey https://www.youtube.com/watch?v=c3iEl5AwUF8&t=659s
//credit to forum https://answers.unity.com/questions/958370/how-to-change-alpha-of-a-sprite.html

public class controlrt_script : MonoBehaviour
{//
    // Start is called before the first frame update
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask trapLayerMask;
    [SerializeField] private LayerMask wizardLayerMask;
    [SerializeField] private LayerMask thickPlatformLayerMask;
    [SerializeField] private LayerMask thinPlatformLayerMask;
    [SerializeField] private GameObject far_back_object;
    [SerializeField] private GameObject stepSound_object;

    private AudioSource stepSound;
    [SerializeField] private GameObject dashSound_object;
    private AudioSource dashSound;
    [SerializeField] private GameObject jumpSound_object;
    private AudioSource jumpSound;
    [SerializeField] private GameObject hurtSound_object;
    private AudioSource hurtSound;

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
                                   //private float maxAirSpeed = 7f; // sets player max speed in air
                                   // private float airControl = 0.5f; // reduces the player's ability to control velocity in air

    //speed vars
    private float maxSpeed = 5f; //sets a limit in how much velocity the player can add to themselves using the LR inputs
    private float maxSpeedInAir = 10f;//sets limit on player x velocity in air
    private float playerAcceleration = 8f; //determines how fast the player will accelerate
    private float slowSpeed = 7f; // changes how fast the player will slow down due to touching a floor or platform(FWI RigidBody2D friction is also slowing the player so slowSpeed == 0 does not mean the player will never stop)

    //dash vars
    private float lastInput = 1;//records the direction of the last LR input(used for dashing and wall jumping)
    private float dashRange = 40f; //changes how far the dash will go
    private float dashLenency = 0.15f; // this is how much time between button presses the player has in order to dash
    private float dashCatcher; // records time between LR button presses. If the player presses the same direction twice in a short amount of time(determined by dashLenency) the player dashes.
    private float dashLock = 0f;//locks out any other input while dashing. when dashLock == 0 user inputs are accepted
    private float dashLockSet = 0.15f;//sets the dash lock to prevent user input for 0.15sec
    private bool canAirDash = true; // locks out dash when false. Alows us to only allow one dash per jump
    private float saveVelocity = 5f;//save's the velocity before a dash so that the player can return to the saved velocity after the dash.
    private float minVelocityAfterDash = 3f;//sets the minimum velocity the player will have after the dash.
    private float dashCoolDown = 1.2f;//sets how long between dashes
    private float dashClock = 0f;// while != 0 player can't dash
    /*END OF MOVEMENT VARS*****************************************************************************/

    //gets user inputs
    private float inputLR;//current left or right input. NOT to be confused with "Input" which is defined in unity.
    private float inputUD;//current up or down input. NOT to be confused with "Input" which is defined in unity.
    private float downHold;//if player presses down arrow key we want to "hold" it for a couple frames so that they fall through platforms

    //Being damaged/after damage variables
    private float setInvincibility = 1.5f;//sets invinicibility's time
    private float invincibilityAfterHit = 0;
    private Color playerColor;
    private float flickerSpeed = 0.2f;//higher flicker speed means slower flickers
    private float flickerCurrent = 0f;


    //Scripts in the form of objects. Used to call functions from those scripts.
    my_hp_script hp_scriptObject;
    cameraScript cameraObject;
    far_object_movement far_object_script;

    //other used var's that don't have a neat catagory
    private bool facingRight = true; // A boolian value that is true if the player is facing right
    private float backgroundDistance = 0.2f;//(original distance + moved_distance*backgroundDistance) is the calculation. If backgroundDistance > 1 it moves like forground. If 1 > backgroundDistance > 0 moves like background. Reverse if <0
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // assigns empty container with the actual component
        facingRight = true;
        ani = GetComponent<Animator>(); // assigns empty container with the actual component
        boxCollider = GetComponent<BoxCollider2D>();
        hp_scriptObject = GameObject.FindGameObjectWithTag("HP Bar").GetComponent<my_hp_script>();
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraScript>();
        far_object_script = far_back_object.GetComponent<far_object_movement>();
        stepSound = stepSound_object.GetComponent<AudioSource>();
        dashSound = dashSound_object.GetComponent<AudioSource>();
        jumpSound = jumpSound_object.GetComponent<AudioSource>();
        hurtSound = hurtSound_object.GetComponent<AudioSource>();
        playerColor = m_SpriteRenderer.GetComponent<SpriteRenderer>().color;

        //spikeObjectImage = hpObject.GetComponent<Image>();//current added

    }

    // Update is called once per frame



    void Update()
    {
        //Block of code that handles collisions
        bool groundedBool = isGrounded();//ground bool is true if player is against an object labled ground
        bool wallBool = isAgainstWall();//wallBool is true if player is against an object labeled wall
        string isTouchingTrapReturnString = isTouchingTrap();
        if (isTouchingWizard()) {
            FindObjectOfType<managingScript>().playerWon();
        }
        int interactionStatus = 0;
        bool platformBool = isTouchingThickPlatform(ref interactionStatus);//platform bool is true if we are touching a platform, interactionStatus is 0 for walls, 1 for floors, 2 for ceiling
        if (platformBool != true)
        {
            platformBool = isTouchingThinPlatform();
            if (platformBool == true)
                interactionStatus = 1;
        }
        //***

        //**block of code that controlls visual aspects of the game like the camera's position, background object's movement, and the player's color after being hit
        cameraObject.upDateCamera(transform.position.x, transform.position.y);//sends the player's position to the camera function called upDateCamera. upDateCamera moves then camera to passed position
        far_object_script.upDateFarObject(transform.position.x, transform.position.y, backgroundDistance);
        if (invincibilityAfterHit > 0) {
            
            if (flickerCurrent <= 0 || invincibilityAfterHit <= 0.3f)//the player will be reset to the normal color for flicker effect or if the flicker effect is about to end
            {
                GetComponent<SpriteRenderer>().color = playerColor;
                flickerCurrent = flickerSpeed;
            }
            else {
                GetComponent<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, playerColor.a - 0.75f * flickerCurrent / flickerSpeed);//player picture drops to a max of 0.75% of it's color
                flickerCurrent = flickerCurrent - Time.deltaTime;
            }

        }
        //***

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

            if (rb.velocity.x + inputLR * Time.deltaTime * playerAcceleration < maxSpeed && rb.velocity.x + inputLR * Time.deltaTime * playerAcceleration > -maxSpeed)//if player is below max speed we can let the input accelerate the player
            {
                rb.velocity = rb.velocity + new Vector2(inputLR * Time.deltaTime * playerAcceleration, 0f);// accelerates the player if they give a LR input
            }
            if ((rb.velocity.x > maxSpeed || rb.velocity.x < -maxSpeed) && (isGrounded() || (platformBool && interactionStatus == 1)))//prevents player from exceeding max speed in the floor
                rb.velocity = new Vector2(inputLR * maxSpeed - 0.01f, rb.velocity.y);
            if ((rb.velocity.x > maxSpeedInAir || rb.velocity.x < -maxSpeedInAir) && !(isGrounded() || (platformBool && interactionStatus == 1)))//prevents player from exceeding maxSpeedInAir while in the air
            {
                if(rb.velocity.x > 0)
                    rb.velocity = new Vector2(maxSpeedInAir - 0.01f, rb.velocity.y);
                if(rb.velocity.x < 0)
                    rb.velocity = new Vector2(-maxSpeedInAir + 0.01f, rb.velocity.y);
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


            //code that handles LR input animation and variables and dashing
            if (dashClock >= 0)//counts down the dash clock
                dashClock = dashClock - Time.deltaTime;
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
                if ((dashCatcher < dashLenency && dashCatcher > 0.0001 && ((lastInput > 0 && inputLR > 0) || (lastInput < 0 && inputLR < 0))) && canAirDash && dashClock <= 0)//if a second input LR(left or right) is detected within 0.001s->0.3s then we dash
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
                    //soundPlayer(1);
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
            if (((wallBool || (platformBool && (interactionStatus == 2 || interactionStatus == 3))) && !groundedBool && inputUD == 1))//activates if player is against a wall and is not on the floor and the player pressed up. This will overwrite any velocity the player had before.
            {

                inputUD = 0;
                if (interactionStatus == 2)
                {
                    rb.velocity = new Vector2(-wallJumpVelocityX, wallJumpVelocityY);//gives player a new velocity of -4x and 3y after wall jumping
                    ani.SetTrigger("wallJump"); //wall jump animation plays

                }
                if (interactionStatus == 3)
                {
                    rb.velocity = new Vector2(wallJumpVelocityX, wallJumpVelocityY); //gives player a new velocity of 4x and 3y after wall jumping
                    //ani.SetTrigger("wallJump_unflipped");//flipped version of the wall jump animation plays
                    ani.SetTrigger("wallJump");
                }
                facingRight = !facingRight;//records the direction sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                dashClock = 0;
                canAirDash = true;//enable air dash
                canAirJump = true;//enable double jump 


            }
            //**



        }
        else if (dashLock > 0)//code that handles dashing
        {
            if (!wallBool && !(platformBool && (interactionStatus == 2 || interactionStatus == 3)))//rb.MovePosition will prevent dashing from moving through walls but it is glitched. Needs further fixing
                if (lastInput > 0)
                    //transform.position = transform.position + new Vector3(Time.deltaTime * dashRange, 0, 0);//this is the dash movement if looking right
                    rb.MovePosition(new Vector2(transform.position.x + Time.fixedDeltaTime * dashRange, transform.position.y));
                else if (lastInput < 0)
                    rb.MovePosition(new Vector2(transform.position.x - Time.fixedDeltaTime * dashRange, transform.position.y));
                //transform.position = transform.position + new Vector3(-Time.deltaTime * dashRange, 0, 0);//this is the dash movement if looking left
                else
                    Debug.Log("error");

            dashLock = dashLock - Time.deltaTime;//decrements the dashlock until 0.2 seconds have passed
            if (dashLock <= 0)//when the dashlock is over this if statment gives the player back their velocity that they had before the dash and activate dash cooldown
            {
                if (saveVelocity < -minVelocityAfterDash && saveVelocity > minVelocityAfterDash)
                    rb.velocity = new Vector2(saveVelocity, 0);
                else
                {
                    rb.velocity = new Vector2(lastInput * minVelocityAfterDash, 0);
                }

                dashClock = dashCoolDown;//activate the dash cool down
            }
        }
        //***end of dashlock


        //code that handles invincibility after being hit and player touching something that damages them by half of a heart.
        if (invincibilityAfterHit > 0f)
        {
            invincibilityAfterHit = invincibilityAfterHit - Time.deltaTime;
            if(isTouchingTrapReturnString == "firewall")
                rb.velocity = new Vector2(-lastInput * wallJumpVelocityX / 2f, wallJumpVelocityY / 4f);

        }
        else if (isTouchingTrapReturnString != "no")
        {
            hp_scriptObject.halfHeartDamage();
            rb.velocity = new Vector2(-lastInput * wallJumpVelocityX/2f, wallJumpVelocityY/4f);
            hurtSound.Play();
            invincibilityAfterHit = setInvincibility;//sets how long the player is invisible for after being hit
        }
        //***

    }//end of update

    float AbsoluteValueFloat(float mathIn)
    {
        if (mathIn > 0f)
        {
            return mathIn;
        }
        return mathIn * -1f;
    }

    void playDash()
    {
        dashSound.Play();
    }

    void playStep() {
        stepSound.Play();
    }
    void playJump()
    {
        jumpSound.Play();
    }




    bool isGrounded() {//credit to Code Monkey
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
        return raycastHit.collider != null;//returns true if it collided, false if it didn't
    }

    string isTouchingTrap() {
        //RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.05f, 0f, Vector2.down, 0f, trapLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)
 
        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, trapLayerMask);
        //return collidedPlatform != null;//returns true if it collided, false if it didn't
        if (collidedPlatform != null)
        {
            return collidedPlatform.gameObject.tag;
        }
        else
            return "no";
    }

    bool isTouchingWizard()
    {
        //RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size*1.05f, 0f, Vector2.down, 0f, trapLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)

        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, wizardLayerMask);
        return collidedPlatform != null;//returns true if it collided, false if it didn't
    }



    bool isTouchingThickPlatform(ref int objectTouched) {

        /*
        Collider2D collidedFloor = Physics2D.OverlapBox(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.size.y/10f), boxCollider.bounds.size*0.95f, 0f, thickPlatformLayerMask);
        Collider2D collidedWall = Physics2D.OverlapBox(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x + boxCollider.bounds.size.x / 5f, boxCollider.bounds.size.y*0.95f), 0f, thickPlatformLayerMask);
         if (collidedFloor == null && collidedWall == null)
        {
            objectTouched = -1;
            return false;
        }
        else if (collidedFloor != null)//if we are on the floor then the fact we are touching a wall does not matter

        {
          
            objectTouched = 1; //1 will be floor
        }
        else if (collidedWall != null && collidedFloor == null)
        {
            //determines if the ray hit a point next to the player
            objectTouched = 0; //0 will be wall
        }
        else//otherwise the point is above the player
            //objectTouched = 2; // 2 will be ceiling, ceiling 
            Debug.Log("error, unintended interaction in function isTouchingThickPlatform in player controller code.");

        return true;*/

        Collider2D collidedFloor = Physics2D.OverlapBox(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.size.y / 10f), boxCollider.bounds.size * 0.95f, 0f, thickPlatformLayerMask);//checks for floor
        RaycastHit2D raycastHitRightWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size * 0.9f, 0f, Vector2.right, boxCollider.bounds.size.x*0.1f, thickPlatformLayerMask);//checks for right wall 
        RaycastHit2D raycastHitLeftWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size * 0.9f, 0f, Vector2.left, boxCollider.bounds.size.x * 0.1f, thickPlatformLayerMask);//checks for left wall
        if (collidedFloor == null && raycastHitRightWall.collider == null && raycastHitLeftWall.collider == null)
        {
            objectTouched = -1;//nothing touched
            return false;
        }
        else if (collidedFloor != null)
        {
            objectTouched = 1; //1 will be floor
        }
        else if (raycastHitRightWall.collider != null && raycastHitLeftWall.collider != null)//if both right and left wall touched
        {
            if (raycastHitRightWall.distance >= raycastHitLeftWall.distance)
                objectTouched = 2;//right wall closer
            else
                objectTouched = 3;//left wall closer
        }
        else if (raycastHitRightWall.collider != null) {
            objectTouched = 2;//right wall
        }
        else if (raycastHitLeftWall.collider != null)
        {
            objectTouched = 3;//left wall
        }
        else
            Debug.Log("error, unintended interaction in function isTouchingThickPlatform in player controller code.");
        
        return true;


    }


    bool isTouchingThinPlatform()
    {
        bool returnBool = false;
        Collider2D[] turnOffArray = new Collider2D[15];

        int numberOfHits = Physics2D.OverlapBoxNonAlloc(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.y*2,boxCollider.bounds.size.y*2), 0, turnOffArray, thinPlatformLayerMask);
        RaycastHit2D raycastHitTurnOn1 = Physics2D.Raycast(new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.size.x/2f, boxCollider.bounds.center.y), 
            Vector2.down, boxCollider.bounds.size.y, thinPlatformLayerMask);
        RaycastHit2D raycastHitTurnOn2 = Physics2D.Raycast(new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.size.x / 2f,boxCollider.bounds.center.y), 
            Vector2.down, boxCollider.bounds.size.y, thinPlatformLayerMask);


        if (downHold > 0)
            downHold = downHold - Time.deltaTime;
        if (inputUD == -1)
        {
            downHold = 0.15f;
        }

        for (int i = 0; i < numberOfHits; i++)//this loop turns off the collision of all the platforms collided with
            turnOffArray[i].isTrigger = true;

        if (downHold <= 0)
        {//if the player held down within the last 0.15 seconds this is false.
            if (raycastHitTurnOn1.collider != null)
            {//checks if there is a platform right under the player, and it's box collider does't overlap with the player, then that platform's collision is turned on

                if ((boxCollider.bounds.center.y - boxCollider.bounds.size.y*5.05f/10f  > raycastHitTurnOn1.point.y) && boxCollider.bounds.center.y > raycastHitTurnOn1.point.y) {
                    raycastHitTurnOn1.collider.isTrigger = false;
                    returnBool = true;
                    
                }
            }
            if (raycastHitTurnOn2.collider != null)
            {//checks if there is a platform right under the player, and it's box collider does't overlap with the player, then that platform's collision is turned on
                if ((boxCollider.bounds.center.y - boxCollider.bounds.size.y * 5.05f / 10f > raycastHitTurnOn2.point.y) && boxCollider.bounds.center.y > raycastHitTurnOn2.point.y)
                {
                    
                    raycastHitTurnOn2.collider.isTrigger = false;
                    returnBool = true;
                }
            }
        }

        return returnBool;


    }


    /*Version 2.0 of isTouchingThinPlatform, comes with a glitch of the player being half way into the floor
    bool isTouchingThinPlatform() {
        RaycastHit2D[] turnOffArray = new RaycastHit2D[15];
        int numberOfHits = Physics2D.BoxCastNonAlloc(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.size.y * 2f),
            new Vector2(boxCollider.bounds.size.x * 2.5f, boxCollider.bounds.size.y), 0f, Vector2.up, turnOffArray, boxCollider.bounds.size.y * 4f, thinPlatformLayerMask);
        
        RaycastHit2D raycastHitTurnOn = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x*0.90f, boxCollider.bounds.size.y), 0f, Vector2.down, boxCollider.bounds.size.y/7f, thinPlatformLayerMask);


        if (downHold > 0)
            downHold = downHold - Time.deltaTime;

        if (inputUD == -1)
        {
            downHold = 0.15f;
        }


        for (int i = 0; i < numberOfHits; i++)//this loop turns off the collision of all the platforms collided with
            turnOffArray[i].collider.isTrigger = true;
        
        if(downHold <= 0)//if the player held down within the last 0.15 seconds this is false.
            if (raycastHitTurnOn.collider != null) {//checks if there is a platform right under the player, and it's box collider does't overlap with the player, then that platform's collision is turned on

                if (raycastHitTurnOn.point.y + boxCollider.bounds.size.y/2f <= boxCollider.bounds.center.y+ boxCollider.bounds.size.y/9f) {
                    raycastHitTurnOn.collider.isTrigger = false;
                    return true;
                }

            }

        return false;


    }
    */

    /* 
     * Version1.0 of isTouchingThinPlatform(this version feels jerky when played
     bool isTouchingThinPlatform()//FWI thin platforms need to be less than 30% player size
     {//returns true if the object is touching a platform and returns 0 if player is on top of the platform, 1 if player is below platform, 2 if player is next to platform, and -1 if player is not touching anything
         RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.size.y * 0.9f),
             new Vector2(boxCollider.bounds.size.x * 1.2f, boxCollider.bounds.size.y/10), 0f, Vector2.down, boxCollider.bounds.size.y * 1.2f + boxCollider.bounds.size.y*0.2f, thinPlatformLayerMask);
         //can be changed to multi_hit if you use  int return_array_size = Physics2D.BoxCast(center,  bounds, angle, direction, distance, RaycastHit2D[], LayerMask);

         float boxColliderCenterX = boxCollider.bounds.center.x;
         float boxColliderCenterY = boxCollider.bounds.center.y;
         float boxColliderSizeX = boxCollider.bounds.size.x / 2f;
         float boxColliderSizeY = boxCollider.bounds.size.y / 2f;
         float mathLenency = 0.06f;
         float detectionLenency = 0.3f;//this gives the detection a bit more time in order to turn the platform back on



             if (raycastHit.collider == null)
         {

             return false;
         }
         else if (((AbsoluteValueFloat(boxColliderCenterY - raycastHit.point.y) > boxColliderSizeY - detectionLenency) 
             && (AbsoluteValueFloat(boxColliderCenterY - raycastHit.point.y) <= boxColliderSizeY + mathLenency)) 
             && (boxColliderCenterY - raycastHit.point.y > 0)//checks if char is within the correct y range
             && (AbsoluteValueFloat(boxCollider.bounds.center.x - raycastHit.collider.bounds.center.x) - raycastHit.collider.bounds.size.x / 2f - boxColliderSizeX <= 0)
             && (inputUD != -1))

         {//determines if the ray hit a point below the player


             raycastHit.collider.isTrigger = false;
             return true;
         }
         raycastHit.collider.isTrigger = true;
         return false;



     }
     */


    bool isAgainstWall() {
        float mathLenency = 0.1f;
        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size + new Vector3(mathLenency, mathLenency, 0f), 0f, wallLayerMask);// casts ray with (center of player, player collider size, rotation, direction, added size to detect, layers to hit)


        return collidedPlatform != null;
    }





}
