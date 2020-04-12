using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        [SerializeField] private AudioClip m_CorrectSound;
        [SerializeField] private AudioClip m_WrongSound;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        private int m_Jumps = 2; // number of jumps allowed before landing
        private int m_JumpCount; // number of jumps remaining
        private GameObject m_LastArea;
        private int roomNum = 0; // # of forest room
        private Vector3[] roomCenters = new Vector3[7];

        public bool onCarpet = false; // access this variable from MagicCarpetScript

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

            m_JumpCount = m_Jumps;
            m_LastArea = GameObject.Find("Central Hub");

            // Get center of each forest room
            for (int r = 0; r <= 6; r++)
            {
                roomCenters[r] = GameObject.Find($"Room {r}").transform.position;
            }
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
                m_JumpCount = m_Jumps; // reset number of jumps allowed after landing
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                    m_JumpCount--;
                }
            }
            else
            {
                // Check if can jump again in the air (and not falling)
                if (m_Jump && m_Jumping && m_JumpCount > 0)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_JumpCount--;
                }
                else
                {
                    // Don't jump right after hitting the ground if the button was pressed in midair
                    m_Jump = false;
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();

            // Respawn after falling off the map
            if (transform.position.y < -5)
            {
                transform.position = new Vector3(50, 1, 25);
            }

            CheckForestState();
        }


        private void CheckForestState()
        {
            // Check if in the forest area
            if (transform.position.x < 75 || transform.position.z < 50) return;

            bool movedForward = transform.position.z >= roomCenters[roomNum].z + 4;
            bool movedBack = transform.position.z <= roomCenters[roomNum].z - 4;
            bool movedRight = transform.position.x >= roomCenters[roomNum].x + 4;
            bool movedLeft = transform.position.x <= roomCenters[roomNum].x - 4;
            bool notInRoom = movedForward || movedBack || movedRight || movedLeft;

            // Check if player moved correctly in the forest maze
            switch (roomNum)
            {
                case 0:
                    // Forward (moving back will exit the area)
                    if (movedForward)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (movedLeft || movedRight)
                    {
                        transform.position = roomCenters[0];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                case 1:
                    // Right
                    if (movedRight)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                case 2:
                    // Left
                    if (movedLeft)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                case 3:
                    // Forward
                    if (movedForward)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                case 4:
                    // Left
                    if (movedLeft)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                case 5:
                    // Right
                    if (movedRight)
                    {
                        roomNum++;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_CorrectSound, transform.position);
                    }
                    else if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                        AudioSource.PlayClipAtPoint(m_WrongSound, transform.position);
                    }

                    break;
                default:
                    // Goal (Any direction takes you back to start)
                    if (notInRoom)
                    {
                        roomNum = 0;
                        transform.position = roomCenters[roomNum];
                    }

                    break;
            }
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        private IEnumerator FadeOut(AudioSource source, float fadeTime)
        {
            // Fade out music for fadeTime seconds
            float startVolume = source.volume; // keep original volume to revert back

            while (source.volume > 0)
            {
                source.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            source.Stop();
            source.volume = startVolume; // return to original volume if played later
        }

        private IEnumerator FadeIn(AudioSource source, float fadeTime)
        {
            // Fade in music for fadeTime seconds
            float startVolume = source.volume; // keep original volume to set at the end
            source.volume = 0;
            source.Play();

            while (source.volume < startVolume)
            {
                source.volume += startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            source.volume = startVolume; // return to original volume
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Change audio track if player is in another terrain
            if (hit.gameObject.CompareTag("Ground") && hit.gameObject != m_LastArea)
            {
                // Crossfade between tracks
                StartCoroutine(FadeOut(m_LastArea.GetComponent<AudioSource>(), 1f));
                StartCoroutine(FadeIn(hit.gameObject.GetComponent<AudioSource>(), 1f));
                m_LastArea = hit.gameObject;
            }
                    
            onCarpet = hit.gameObject.name == "Carpet";

            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
