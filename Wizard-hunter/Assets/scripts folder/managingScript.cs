using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class managingScript : MonoBehaviour
{
    private bool playerDead;


    void Start() {
        playerDead = false;

    }
    public void nextLevel() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name + 1);

    }

    public void playerDied() {
        if (playerDead == false)
        {
            //Debug.Log("testingDeath");
            playerDead = true;
            SceneManager.LoadScene("testingDeath");
        }
    } 

    //void Start()
   // {
        
    //}

    

    // Update is called once per frame
   // void Update()
   // {
        
   // }
}
