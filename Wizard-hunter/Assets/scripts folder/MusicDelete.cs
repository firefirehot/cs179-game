using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicDelete : MonoBehaviour
{
    private int sceneIndex;
    // Start is called before the first frame update
    void Awake()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != sceneIndex){
            Destroy(gameObject);
        }
    }
}
