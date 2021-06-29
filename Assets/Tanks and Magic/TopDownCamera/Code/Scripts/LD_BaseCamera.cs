using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LevDev.Cameras
{
    public class LD_BaseCamera : MonoBehaviourPunCallbacks
    {
        #region Variables
        public Transform m_Target;
        #endregion

        #region Main Methods

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                HandleCamera();
            }
        }
        #endregion

        #region Helper Methods
        protected virtual void HandleCamera()
        {
            if (!m_Target)
            {
                return;
            }
        }
        #endregion
    }
}
