using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlrt_script : MonoBehaviour
{
    // Start is called before the first frame update

    //private CharacterController controller;
    private float movementSpeed = 5f;

    void Start()
    {
        //controller = gameObject.AddComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {

        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        float input = Input.GetAxis("Horizontal");
        transform.position = transform.position + new Vector3(input*Time.deltaTime*movementSpeed, 0, 0);
        //controller.Move(Time.deltaTime * move);

    }
}
