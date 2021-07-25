using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Settigns : MonoBehaviour
{
    public GameObject fps_Counter;
    public Toggle fps_Toggle;

    public void Start()
    {
        //Show/Hide FPS
        if (PlayerPrefs.GetInt("ShowFPS") == 1)
        {
            fps_Toggle.isOn = true;
        }
        else
        {
            fps_Toggle.isOn = false;
        }
    }

    public void ShowFPS(bool showFPS)
    {
        if (showFPS)
        {
            PlayerPrefs.SetInt("ShowFPS", 1); //Set 1 if FPS show
            fps_Counter.SetActive(true);
            fps_Toggle.isOn = true;
        }
        else
        {
            PlayerPrefs.SetInt("ShowFPS", 0);
            fps_Counter.SetActive(false);
            fps_Toggle.isOn = false;
        }
    }
}
