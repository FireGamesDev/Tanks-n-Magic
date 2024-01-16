using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.AudioSettings;

namespace LevDev
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TankInputs))]
    public class TankController : MonoBehaviourPunCallbacks
    {
        #region Variables

        [Header("Movement Properties")]
        private float tankSpeed = 15f;
        public float tankSpeedBase = 15f;
        public float tankSpeedShoot = 15f;
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

        [Header("Mobile")]
        [SerializeField] private VariableJoystick _movementJoystick;
        [SerializeField] private VariableJoystick _shootingJoystick;

        private bool isMobile = false;
        #endregion


        #region Methods
        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<TankInputs>();

            isMobile = CheckMobile.IsMobile;

            _movementJoystick.gameObject.SetActive(isMobile);
            _shootingJoystick.gameObject.SetActive(isMobile);
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

            reticleTransform.gameObject.SetActive(!isMobile);
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

            float speedTransitionRate = 0.5f;

            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse1))
            {
                // Transition to the shoot speed
                tankSpeed = Mathf.Lerp(tankSpeed, tankSpeedShoot, Time.deltaTime * speedTransitionRate);
            }
            else
            {
                // Transition back to the base speed
                tankSpeed = Mathf.Lerp(tankSpeed, tankSpeedBase, Time.deltaTime * speedTransitionRate);
            }


            if (photonView.IsMine)
            {
                if (Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic)
                {
                    return;
                }

                Move();

                Vector2 rotation = Vector2.zero;

                if (isMobile)
                {
                    rotation = _shootingJoystick.Direction.normalized;

                    if (rotation.x == 0 && rotation.y == 0)
                    {
                        rotation = _movementJoystick.Direction.normalized;
                    }
                }

                if (gameObject.tag == "GreenTank")
                {
                    TurnGreen(rotation);
                } else
                {
                    Turn(rotation);
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

        private void Turn(Vector2 rotation)
        {
            if (isMobile)
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation.x, rotation.y, 0f), tankRotationSpeed * Time.deltaTime));
            }

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

        private void TurnGreen(Vector2 rotation)
        {
            if (isMobile)
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation.x, rotation.y, 0f), tankRotationSpeed * Time.deltaTime));
            }

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
            if (isMobile)
            {
                movement = _movementJoystick.Direction;
                Vector3 normalizedMovement = movement.normalized;
                rb.MovePosition(transform.position - normalizedMovement * tankSpeed * Time.deltaTime);

                if (movement == Vector3.zero)
                {
                    LightsOff();
                } else LightsOn();

                return;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Vector3 normalizedMovement = movement.normalized;
                rb.MovePosition(transform.position - normalizedMovement * tankSpeed * Time.deltaTime);

                LightsOn();
            } else LightsOff();
        }

        private void HandleTurret()
        {
            if (isMobile)
            {
                Vector3 turretLookDir = _shootingJoystick.Direction;
                turretLookDir.y = 0f;

                finalTurretLookDir = Vector3.Lerp(finalTurretLookDir, turretLookDir, Time.deltaTime * turretLagSpeed);
                turretTransform.rotation = Quaternion.LookRotation(finalTurretLookDir);

                return;
            }

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
                    m_MovementAudio.Stop();
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.Play();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    m_MovementAudio.Stop();
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
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
