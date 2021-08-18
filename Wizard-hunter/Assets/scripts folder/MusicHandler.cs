using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    private AudioSource aSource;
    private bool isPaused = false;
    public static GameObject first = null; 
    [SerializeField] private GameObject audioS;

    void Awake(){
        if(first != null){
            Destroy(audioS);
        }
        else{
            first = audioS;
            aSource = first.GetComponent<AudioSource>();
            DontDestroyOnLoad(first);
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

    public void QuitTrigger(){
        if(first != null){
            Destroy(first);
        }
    }
}
