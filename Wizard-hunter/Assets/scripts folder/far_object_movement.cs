using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class far_object_movement : MonoBehaviour
{
    // Start is called before the first frame update

    private float startingX = 0;
    private float startingY = 0;
    private float playerStartX = 0;
    private float playerStartY = 0;
    private bool firstLoopBool = true;

    void Start()
    {
        startingX = transform.position.x;
        startingY = transform.position.y;

    }

    public void upDateFarObject(float xin, float yin, float distance)
    {
        if (firstLoopBool) {
            playerStartX = xin;
            playerStartY = yin;
            firstLoopBool = false;
        }
        transform.position = new Vector3(startingX + (xin - playerStartX) *distance, startingY + (yin - playerStartY) *distance, transform.position.z);
    }
}

