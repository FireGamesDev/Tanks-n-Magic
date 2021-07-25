using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevDev
{
    public class TankInputs : MonoBehaviour
    {
        private GameObject playerCam;

        private void Start()
        {
            playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        }


        #region Properties
        private Vector3 reticlePosition;
        public Vector3 ReticlePosition
        {
            get { return reticlePosition; }
        }

        private Vector3 reticleNormal;
        public Vector3 ReticleNormal
        {
            get { return reticleNormal; }
        }
        #endregion

        #region Builtin Methods

        void Update()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                if (GameObject.FindGameObjectWithTag("GameHandler"))
                {
                    if (GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>().stopped)
                    {
                        return;
                    }
                }
            }
            if (playerCam)
            {
                HandleInputs();
            } else playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(reticlePosition, 0.5f);
        }
        #endregion

        #region Custom Methods
        protected virtual void HandleInputs()
        {
            Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(screenRay, out hit))
            {
                reticlePosition = hit.point;
                reticleNormal = hit.normal;
            }
        }
        #endregion
    }
}
