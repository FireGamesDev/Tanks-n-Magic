using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Multiplayer
{
    public class PlayersInLobby : MonoBehaviour
    {
        public static PlayersInLobby instance;
        [SerializeField] GameObject playerListingPrefab = null;

        private void Start()
        {
            instance = this;
        }

        private void Update()
        {
            if (instance)
            {
                instance = this;
            } 
        }

        public void AddNewPlayer(string name, Color color)
        {
            GameObject temp = Instantiate(playerListingPrefab, transform);
            temp.transform.SetSiblingIndex(0);
            PlayersList tempListing = temp.GetComponent<PlayersList>();
            tempListing.GetPlayer(name, color);
        }

        public static void RemovePlayer(string name)
        {
            GameObject[] playersName = GameObject.FindGameObjectsWithTag("PlayerListInstance");

            foreach(GameObject playerName in playersName)
            {
                if (playerName.GetComponent<PlayersList>().GetName().Equals(name))
                {
                    Destroy(playerName);
                }
            }
        }
    }
}
