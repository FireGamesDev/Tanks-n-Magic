using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Shower : MonoBehaviour
{
    public GameObject fpsCounter;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("ShowFPS") == 1)
        {
            fpsCounter.SetActive(true); // Set the FPS if ShowFPS is true
        }
        else fpsCounter.SetActive(false);
    }
}
