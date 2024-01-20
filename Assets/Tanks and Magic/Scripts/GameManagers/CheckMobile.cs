using PlayFab.ExperimentationModels;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CheckMobile : MonoBehaviour
{
    public static bool IsMobile = true;
    public static CheckMobile Instance;

    [SerializeField] private GameObject chargedBulletDisplay;
    [SerializeField] private GameObject chat;
    public Button chargedBulletButton;

    [DllImport("__Internal")]
    private static extern bool isMobileWebGL();

    public bool CheckIfWebGLIsMobile()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        return true;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        return true;
#endif
#if !UNITY_EDITOR && UNITY_WEBGL
        return isMobileWebGL();
#endif
        return IsMobile;
    }

    private void Start()
    {
        Instance = this;

#if !UNITY_EDITOR && UNITY_WEBGL
        IsMobile = CheckIfWebGLIsMobile();
#endif
        if (chat != null)
        {
            chat.SetActive(!IsMobile);
            chargedBulletDisplay.SetActive(!IsMobile);
            chargedBulletButton.gameObject.SetActive(IsMobile);
        }
        
    }
}
