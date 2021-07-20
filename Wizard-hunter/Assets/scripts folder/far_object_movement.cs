using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class far_object_movement : MonoBehaviour
{
    // Start is called before the first frame update

    private float startingX = 0;
    private float startingY = 0;

    void Start()
    {
        startingX = transform.position.x;
        startingY = transform.position.y;

    }

    public void upDateFarObject(float xin, float yin, float distance)
    {
        if (distance <= 0)
            Debug.Log("DO NOT PASS distance <= 0 to upDateFarObject from far_object_movement script");
        transform.position = new Vector3(startingX + (xin-startingX)/distance, startingY + (yin-startingY)/distance, transform.position.z);
    }
}

