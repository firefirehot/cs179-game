using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{

    public void upDateCamera(float xin, float yin)
    {
        transform.position = new Vector3(xin, yin + 2, transform.position.z);
    }
}
