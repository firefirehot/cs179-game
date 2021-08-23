using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTracker : MonoBehaviour
{

    public Slider slider;
    public GameObject target;
    public GameObject endpoint;
    public bool x_value;

    // Update is called once per frame
    void Update()
    {
        if (x_value)
        {
            float playerProgress = target.transform.transform.position.x / endpoint.transform.position.x;
            if (playerProgress < 0.0f)
            {
                playerProgress = 0.0f;
            }
            slider.value = playerProgress;
        }
        else
        {
            float playerProgress = target.transform.transform.position.y / endpoint.transform.position.y;
            if (playerProgress < 0.0f)
            {
                playerProgress = 0.0f;
            }
            slider.value = playerProgress;
        }
    }
}
