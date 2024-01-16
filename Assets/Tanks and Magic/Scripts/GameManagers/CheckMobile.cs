using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        chat.SetActive(!IsMobile);
        chargedBulletDisplay.SetActive(!IsMobile);
        chargedBulletButton.gameObject.SetActive(IsMobile);
    }
}
