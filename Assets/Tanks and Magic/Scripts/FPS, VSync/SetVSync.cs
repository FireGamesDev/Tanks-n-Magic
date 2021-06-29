using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVSync : MonoBehaviour
{
    public Toggle toggle_VSync;

    private void Awake()
    {
        Application.targetFrameRate = 300;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("isVSync") == 1)
        {
            QualitySettings.vSyncCount = 1;
            VSync(true);
        }
        else {
            QualitySettings.vSyncCount = 0;
            VSync(false);
        }
    }

    public void VSync(bool isVSync)
    {
        if (isVSync)
        {
            PlayerPrefs.SetInt("isVSync", 1);
            QualitySettings.vSyncCount = 1;
            toggle_VSync.isOn = true;
            Debug.Log("VSync is on.");
        }
        else
        {
            PlayerPrefs.SetInt("isVSync", 0);
            QualitySettings.vSyncCount = 0;
            toggle_VSync.isOn = false;
            Debug.Log("VSync is off.");
        }
    }
}
