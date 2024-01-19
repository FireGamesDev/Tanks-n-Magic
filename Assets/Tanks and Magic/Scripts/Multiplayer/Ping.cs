using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace Scripts.Managers.UI {
	public class Ping : MonoBehaviour
	{
        [SerializeField] private TMPro.TMP_Text _text;
        [SerializeField] private List<Image> _pingSquares;

        private void Start()
        {
            InvokeRepeating(nameof(UpdatePingUI), 0f, 1f); // Change to a less frequent update if needed
        }

        private void UpdatePingUI()
        {
            int ping = PhotonNetwork.GetPing();
            _text.text = ping.ToString() + " ms " + PhotonNetwork.CloudRegion;

            UpdatePingSquares(ping);
        }

        private void UpdatePingSquares(int ping)
        {
            Color squareColorDef = new Color(0, 0, 0, 0.3f); // Black with 0.3 opacity

            // Set color based on ping
            if (ping <= 65)
            {
                SetColorForSquares(4, Color.green, squareColorDef);
            }
            else if (ping <= 100)
            {
                SetColorForSquares(3, Color.yellow, squareColorDef);
            }
            else if (ping <= 200)
            {
                Color _orange = new Color(1.0f, 0.64f, 0.0f);
                SetColorForSquares(2, _orange, squareColorDef);
            }
            else
            {
                SetColorForSquares(1, Color.red, squareColorDef);
            }
        }

        private void SetColorForSquares(int amount, Color color, Color colorDef)
        {
            int i = 1;
            _text.color = color;
            foreach (Image square in _pingSquares)
            {
                if (i <= amount)
                {
                    square.color = color;
                }
                else
                {
                    square.color = colorDef;
                }
                
                i++;
            }
        }
    }
}
