using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicHandler : MonoBehaviour
{
    private AudioSource aSource;
    private bool isPaused = false;
    public static GameObject first = null; 
    public static int scene; 

    //Called to ensure gameObject, if the first, is saved as such and will be preserved on reset
    void Awake(){
        if(first != null){
            Destroy(gameObject);
        }
        else{
            first = gameObject;
            scene = SceneManager.GetActiveScene().buildIndex;
            aSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
    }

    //Update is called every frame. Checks to see if we are in a different scene than what we started in.
    void Update(){
        if(SceneManager.GetActiveScene().buildIndex != scene){
            Destroy(gameObject);
        }
    }



    // Pauses and unpauses the Music
    public void Pause(){
        if(!isPaused){
            aSource.Pause();
            isPaused = true;
        }
        else{
            aSource.Play(0);
            isPaused = false;
        }
    }
}
