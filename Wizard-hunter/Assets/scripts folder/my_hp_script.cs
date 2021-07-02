using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class my_hp_script : MonoBehaviour
{
    // Start is called before the first frame update\\renamed now

    private int hitsTaken;
    private Image myImage;
    private Sprite fourHearts;
    private Sprite threeHalfHearts;
    private Sprite threeHearts;
    private Sprite twoHalfHearts;
    private Sprite twoHearts;
    private Sprite oneHalfHearts;
    private Sprite oneHearts;
    private Sprite halfHearts;
    //private Sprite dead;
    void Start()
    {
        hitsTaken = 0;
        myImage = GetComponent<Image>();
        fourHearts = Resources.Load<Sprite>("heart_Images/four_hearts");
        threeHalfHearts = Resources.Load<Sprite>("heart_Images/three_and_half_hearts"); 
        threeHearts = Resources.Load<Sprite>("heart_Images/three_hearts");
        twoHalfHearts = Resources.Load<Sprite>("heart_Images/two_and_half_hearts");
        twoHearts = Resources.Load<Sprite>("heart_Images/two_hearts");
        oneHalfHearts = Resources.Load<Sprite>("heart_Images/one_and_half_hearts");
        oneHearts = Resources.Load<Sprite>("heart_Images/one_heart"); 
        halfHearts = Resources.Load<Sprite>("heart_Images/half_heart");
        
    }



    public void halfHeartDamage() {//I should change this to a switch statement but im wayyyyy to lazy to actually do it instead of typing all this long, run-on-sentence out.
        if(hitsTaken < 7)
            hitsTaken = hitsTaken + 1;
        if (hitsTaken == 0) { myImage.sprite = fourHearts; }
        else if (hitsTaken == 1) { myImage.sprite = threeHalfHearts; }
        else if (hitsTaken == 2) { myImage.sprite = threeHearts; }
        else if (hitsTaken == 3) { myImage.sprite = twoHalfHearts; }
        else if (hitsTaken == 4) { myImage.sprite = twoHearts; }
        else if (hitsTaken == 5) { myImage.sprite = oneHalfHearts; }
        else if (hitsTaken == 6) { myImage.sprite = oneHearts; }
        else if (hitsTaken == 7) { myImage.sprite = halfHearts; }
        //else if (hitsTaken == 8) { myImage.sprite = dead; kill player }
        
    }
}
