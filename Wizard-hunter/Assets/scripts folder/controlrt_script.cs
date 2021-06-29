using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlrt_script : MonoBehaviour
{
    // Start is called before the first frame update

    private float movementSpeed = 2f; // player movement speed, change this to slow/speed the player up. This does not change the animation at all
    private bool facingRight = true; // A boolian value that is true if the player is facing right
    SpriteRenderer m_SpriteRenderer; //empty sprite render container
    private Animator ani;// empty animator container, animator decides which animation to play
    private float dashCatcher; // records time. If the player presses the same direction twice in a short amount of time the player dashes.
    private float lastInput = 1;//records the direction of the last input
    private float dashLock = 0;//locks out any other input while dashing. 
    private float input;//current input. NOT to be confused with "Input" which is defined in unity.
    private float timeRunning = 0f;
    //private int dashPlayer = 3;


    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // assigns empty container with the actual component
        facingRight = true; 
        ani = GetComponent<Animator>(); // assigns empty container with the actual component

    }

    // Update is called once per frame



    void Update()
    {
        if (dashLock <= 0)
        {
            ani.SetBool("dashing", false);

            if (Input.GetKey("d"))
                input = 1;
            else if (Input.GetKey("a"))
                input = -1;
            else
                input = 0;
            transform.position = transform.position + new Vector3(input * Time.deltaTime * movementSpeed, 0, 0);//moves the character by getting the characters current position and 
                                                                                                                //adding "input*Time.deltaTime*movementSpeed"(direction = input, Time.deltaTime = time since last frame, movementSpeed multiplier)

            if (input < 0 && facingRight == true || input > 0 && facingRight == false)
            {//flips the sprite if we get a input in the opposite direction that the sprite is facing
                m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
                facingRight = !facingRight;//records the direction sprite is facing

            }
            if (input == 0)
            {//changes animation to idle if no input is detected
                ani.SetBool("isRunning", false);
                dashCatcher = dashCatcher + Time.deltaTime;//keeps track of the how much time it has been since no input has been detected
                resetMovementSpeed();//resets movement speed

            }
            else if (input != 0)
            {// changes animation to running if an input is detected

                if ((dashCatcher < 0.3 && dashCatcher > 0.0001 && ((lastInput > 0 && input > 0) || (lastInput < 0 && input < 0))))
                {
                    ani.SetBool("isRunning", false);
                    ani.SetBool("dashing",true);
                    dashLock = 0.2F;//sets the dashLock to prevent user input for 0.2 seconds
                }
                else
                    ani.SetBool("isRunning", true);
                lastInput = input;
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
