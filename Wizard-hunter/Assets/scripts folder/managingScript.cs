using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class managingScript : MonoBehaviour
{
    private bool playerDead;
    private bool playerWins;


    void Start() {
        playerDead = false;
        playerWins = false;
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
    public void playerWon()
    {
        if (playerWins == false)
        {
            Debug.Log("testingDeath");
            playerWins = true;
            SceneManager.LoadScene("testingWin");
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
