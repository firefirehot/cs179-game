using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlrt_script : MonoBehaviour
{
    // Start is called before the first frame update

    private float movementSpeed = 5f; // player movement speed, change this to slow/speed the player up. This does not change the animation at all
    private bool facingRight = true; // A boolian value that is true if the player is facing right
    SpriteRenderer m_SpriteRenderer; //empty sprite render container
    private Animator ani;// empty animator container, animator decides which animation to play


    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // assigns empty container with the actual component
        facingRight = true; 
        ani = GetComponent<Animator>(); // assigns empty container with the actual component

    }

    // Update is called once per frame



    void Update()
    {

        float input = Input.GetAxis("Horizontal");//gets user input. User input is -1,0,1 representing in order left, nothing, right
        transform.position = transform.position + new Vector3(input*Time.deltaTime*movementSpeed, 0, 0);//moves the character by getting the characters current position and 
                                                                                                        //adding "input*Time.deltaTime*movementSpeed"(direction = input, Time.deltaTime = time since last frame, movementSpeed multiplier)
        if (input < 0 && facingRight == true || input > 0 && facingRight == false) {//flips the sprite if we get a input in the opposite direction that the sprite is facing
            m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;//flips the sprite
            facingRight = !facingRight;//records the direction sprite is facing

        }
        if (input == 0) {//changes animation to idle if no input is detected
            ani.SetBool("isRunning", false);
            
        }
        else if (input != 0) {// changes animation to running if an input is detected
            ani.SetBool("isRunning", true);
        }

        

    }
}
