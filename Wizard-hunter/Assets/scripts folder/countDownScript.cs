using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class countDownScript : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] private float initialTime = 100f;
    private Text myText;
    //public Text myText;
    private float TimePassed;
    void Start()
    {
        myText = GetComponent<Text>();
        TimePassed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float timeCalc = 0;
        int minuete = 0;
        int tenSecond = 0;
        int oneSecond = 0;

        if (TimePassed < initialTime+1)
        {
            TimePassed = Time.deltaTime + TimePassed;
            timeCalc = initialTime - TimePassed;

            while (timeCalc - 60 >= 0)
            {
                timeCalc = timeCalc - 60;
                minuete++;
            }
            while (timeCalc - 10 >= 0)
            {
                timeCalc = timeCalc - 10;
                tenSecond++;
            }
            while (timeCalc - 1 >= 0)
            {
                timeCalc = timeCalc - 1;
                oneSecond++;
            }

            myText.text = minuete.ToString() + ":" + tenSecond.ToString() + oneSecond.ToString();
        }
        else {
            myText.text = "0" + ":" + "0" + "0";
            FindObjectOfType<managingScript>().playerDied();
        }

    }
}
