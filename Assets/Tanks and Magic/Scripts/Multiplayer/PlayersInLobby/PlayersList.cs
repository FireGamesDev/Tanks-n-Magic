using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
    public class PlayersList : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text playerNameDisplay = null;

        public void GetPlayer(string name, Color color)
        {
            playerNameDisplay.text = name;
            playerNameDisplay.color = color;
        }

        public string GetName()
        {
            return playerNameDisplay.text;
        }
    }
}
