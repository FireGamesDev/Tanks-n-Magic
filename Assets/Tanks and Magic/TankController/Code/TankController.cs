using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LevDev
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TankInputs))]
    public class TankController : MonoBehaviourPunCallbacks
    {
        #region Variables

        [Header("Movement Properties")]
        public float tankSpeed = 15f;
        public float tankRotationSpeed = 20f;
        Vector3 movement;

        [Header("Turret Properties")]
        public Transform turretTransform;
        public float turretLagSpeed = 0.5f;

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        private Rigidbody rb;
        private TankInputs input;
        private Vector3 finalTurretLookDir;

        [Header("SFX")]
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
        public float m_PitchRange = 0.4f;           // The amount by which the pitch of the engine noises can vary.

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

        [Header("Trail")]
        public GameObject leftTrack;
        public GameObject leftTrackLight;
        public GameObject rightTrack;
        public GameObject rightTrackLight;
        public GameObject leftTrail;
        public GameObject rightTrail;
        #endregion


        #region Methods
        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<TankInputs>();
        }

        private void Start()
        {
            if (!photonView.IsMine)
            {
                reticleTransform.gameObject.SetActive(false);
                return;
            }

            // The axes names are based on player number.
            m_MovementAxisName = "Vertical";
            m_TurnAxisName = "Horizontal";

            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;

            LightsOn();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                // Store the value of both input axes.
                m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
                m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

                //Handle movement
                if (gameObject.tag == "GreenTank")
                {
                    movement.z = -Input.GetAxis("Horizontal");
                    movement.x = -Input.GetAxis("Vertical");
                } else
                {
                    movement.z = Input.GetAxis("Horizontal");
                    movement.x = Input.GetAxis("Vertical");
                }
            }
            EngineAudio();
        }

        void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse1))
            {
                return;
            }

            if (photonView.IsMine)
            {
                if (Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic)
                {
                    return;
                }

                Move();

                if(gameObject.tag == "GreenTank")
                {
                    TurnGreen();
                } else
                {
                    Turn();
                }
            }
        }

        void LateUpdate()
        {
            if (photonView.IsMine)
            {
                HandleTurret();
                HandleReticle();
            }
        }
        #endregion

        #region Custom Methods

        #endregion

        #region Multiplayer synch

        private void Turn()
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 135f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 45f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 225f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 315f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
            {
                return;
            }
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                return;
            }
            if (Input.GetKey(KeyCode.W))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 90f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 270f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 180f, 0f), tankRotationSpeed * Time.deltaTime));
            }
        }

        private void TurnGreen()
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -45f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -135f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -315f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -225f, 0f), tankRotationSpeed * Time.deltaTime));
                return;
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
            {
                return;
            }
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                return;
            }
            if (Input.GetKey(KeyCode.W))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -90f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -180f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, -270f, 0f), tankRotationSpeed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), tankRotationSpeed * Time.deltaTime));
            }
        }

        private void Move()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                rb.MovePosition(transform.position - movement * tankSpeed * Time.deltaTime);

                LightsOn();
            } else LightsOff();
        }

        private void HandleTurret()
        {
            if (turretTransform)
            {
                Vector3 turretLookDir = input.ReticlePosition - turretTransform.position;
                turretLookDir.y = 0f;

                finalTurretLookDir = Vector3.Lerp(finalTurretLookDir, turretLookDir, Time.deltaTime * turretLagSpeed);
                turretTransform.rotation = Quaternion.LookRotation(finalTurretLookDir);
            }
        }

        private void HandleReticle()
        {
            if (reticleTransform)
            {
                reticleTransform.position = input.ReticlePosition;
            }
        }

        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange) + m_PitchRange;
                    m_MovementAudio.Play();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange) + m_PitchRange;
                    m_MovementAudio.Play();
                }
            }
        }

        private void LightsOn()
        {
            leftTrail.SetActive(true);
            rightTrail.SetActive(true);
            leftTrackLight.SetActive(true);
            rightTrackLight.SetActive(true);
            leftTrack.SetActive(false);
            rightTrack.SetActive(false);
        }

        private void LightsOff()
        {
            leftTrail.SetActive(false);
            rightTrail.SetActive(false);
            leftTrackLight.SetActive(false);
            rightTrackLight.SetActive(false);
            leftTrack.SetActive(true);
            rightTrack.SetActive(true);
        }

        #endregion
    }
}
