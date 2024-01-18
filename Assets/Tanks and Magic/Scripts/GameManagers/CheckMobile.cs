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

    private void Awake()
    {
        Instance = this;
    }

    [DllImport("__Internal")]
    private static extern void isMobileWebGL();

    private bool CheckIfWebGLIsMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return isMobileWebGL();
#endif
        return false;
    }

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        IsMobile = CheckIfWebGLIsMobile();
#endif
        chat.SetActive(!IsMobile);
        chargedBulletDisplay.SetActive(!IsMobile);
        chargedBulletButton.gameObject.SetActive(IsMobile);
    }
}
