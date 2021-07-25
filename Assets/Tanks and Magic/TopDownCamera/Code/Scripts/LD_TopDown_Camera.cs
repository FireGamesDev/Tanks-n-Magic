using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace LevDev.Cameras
{
    public class LD_TopDown_Camera : LD_BaseCamera
    {
        #region Variables

        public float m_Height = 45f;
        public float m_Distance = 30f;

        [SerializeField]
        public float m_Angle = -90f;

        [SerializeField]
        private float m_SmoothSpeed = 0f;

        private Vector3 refVelocity = Vector3.zero;

        private bool found = false;
        #endregion

        #region Main Methods

        void Update()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                if (GameObject.FindGameObjectWithTag("GameHandler"))
                {
                    if (GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>().stopped)
                    {
                        HandleCamera();
                        found = false;
                    }
                }
            }
            if (found)
            {
                if (m_Target)
                {
                    HandleCamera();
                }
            } else
            {
                if (m_Target)
                {
                    if (m_Target.tag == "GreenTank")
                    {
                        m_Angle = 90f;
                        found = true;
                    }
                    if (m_Target.tag == "PurpleTank")
                    {
                        found = true;
                    }
                }
            }
        }
        #endregion

        #region Helper Methods
        protected override void HandleCamera()
        {
            base.HandleCamera();

            //Build World position vector
            Vector3 worldPosition = (Vector3.forward * m_Distance) + (Vector3.up * m_Height);
            //Debug.DrawLine(m_Target.position, worldPosition, Color.red);

            //Build our Rotated vector
            Vector3 rotatedVector = Quaternion.AngleAxis(m_Angle, Vector3.up) * worldPosition;
            //Debug.DrawLine(m_Target.position, rotatedVector, Color.green);

            //Move our position
            Vector3 flatTargetPosition = m_Target.position;
            flatTargetPosition.y = 0f;
            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(m_Target.position, finalPosition, Color.blue);
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, m_SmoothSpeed);
            transform.LookAt(m_Target.position);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            if (m_Target)
            {
                Gizmos.DrawLine(transform.position, m_Target.position);
                Gizmos.DrawSphere(m_Target.position, 1.5f);
            }
            Gizmos.DrawSphere(transform.position, 1.5f);
        }
        #endregion
    }

}
