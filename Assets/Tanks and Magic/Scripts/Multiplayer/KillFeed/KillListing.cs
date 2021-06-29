using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillListing : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text killerDisplay = null;
    [SerializeField] TMPro.TMP_Text killedDisplay = null;
    [SerializeField] UnityEngine.UI.Image howImageDisplay = null;

    private void Start()
    {
        Destroy(gameObject, 10f); //destroy the kill text
    }

    public void CreateKillFeed(string killerName, string killedName, Sprite howImage, Color killerColor, Color killedColor)
    {
        killerDisplay.color = killerColor;
        killedDisplay.color = killedColor;
        killerDisplay.text = killerName;
        killedDisplay.text = killedName;
        howImageDisplay.sprite = howImage;
    }
}
