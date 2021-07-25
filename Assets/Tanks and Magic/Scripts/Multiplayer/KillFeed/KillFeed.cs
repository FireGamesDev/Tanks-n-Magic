using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace TankScripts
{
    public class KillFeed : MonoBehaviourPunCallbacks
    {
        public static KillFeed instance;
        public GameObject killListingPrefab;
        public Sprite[] howImages;


        private void Start()
        {
            instance = this;
        }

        public void AddNewKillListing(string killer, string killed, int howIndex, Color killerColor, Color killedColor)
        {
            GameObject temp = Instantiate(killListingPrefab, transform);
            temp.transform.SetSiblingIndex(0);
            KillListing tempListing = temp.GetComponent<KillListing>();
            tempListing.CreateKillFeed(killer, killed, howImages[howIndex], killerColor, killedColor);
        }
    }
}
