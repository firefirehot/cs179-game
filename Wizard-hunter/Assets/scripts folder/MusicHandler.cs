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
    [SerializeField] GameObject player;

    //Called to ensure the music source, if the first, is saved as such and will be preserved on reset.
    void Awake(){
        if(first != null){
            Destroy(player);
            aSource = first.GetComponent<AudioSource>();
        }
        else{
            first = player;
            scene = SceneManager.GetActiveScene().buildIndex;
            aSource = first.GetComponent<AudioSource>();
            DontDestroyOnLoad(first);
        }
    }

    //Update is called every frame. Checks to see if we are in a different scene than what we started in.
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Pause();
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
