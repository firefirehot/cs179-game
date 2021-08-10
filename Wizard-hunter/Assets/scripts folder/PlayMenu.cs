using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Needed to change scenes for level transitions

public class PlayMenu : MonoBehaviour
{
    //PlayGame1_1 will load level 1 wizard 1, when those scenes have been initialized
    public void PlayGame1_1()
    {
        SceneManager.LoadScene(1);
    }

    //PlayGame1_2 will load level 1 wizard 1, when those scenes have been initialized  
    public void PlayGame1_2()
    {
        Debug.Log("TODO PlayGame1_2");
    }

    //PlayGame2_1 will load level 2 wizard 1, when those scenes have been initialized
    public void PlayGame2_1()
    {
        SceneManager.LoadScene(2);
    }
    
    //PlayGame2_2 will load level 2 wizard 2, when those scenes have been initialized
    public void PlayGame2_2()
    {
        Debug.Log("TODO PlayGame2_2");
    }
}
