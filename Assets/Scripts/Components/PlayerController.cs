using System;
using System.Collections;
using Elias.Scripts.Helper;
using Noah.Scripts.Camera;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Elias.Scripts.Components
{
    public class PlayerController : MonoBehaviour
    {
        [HideInInspector] public static bool IsClimbing;
        [HideInInspector] public bool IsOnPlatform;
        [HideInInspector] public Rigidbody2D PlatformRb;


        [Header("Movement")] [SerializeField] private float _moveSpeed = 7.5f;

        [Header("Jump")] [SerializeField] private float _jumpForce = 12f;

        [SerializeField] private float _jumpTime = 0.35f;

        [Header("Turn Check")] [SerializeField]
        private GameObject _leftLeg;

        [SerializeField] private GameObject _rightLeg;
        
        [SerializeField] private LayerMask _whatIsGround;

        [Header("Camera")] [SerializeField] private GameObject _cameraFollowGO;

        [HideInInspector] public bool IsFacingRight;
        public Animator _anim;

        private Light2D _playerLight;
        private CameraFollowObject _cameraFollowObject;
        private bool _canMoveBox;
        private Collider2D _coll;
        private float _fallSpeedYDampingChangeThreshold;
        private RaycastHit2D _groundHit;

        private bool _isFalling;
        private bool _isGrounded;
        private bool _isJumping;
        private bool _isMoving;
        private float _jumpTimeCounter;

        private GameObject _movableBox;
        private Rigidbody2D _movableRigidbody2D;

        private float _moveInputx;
        private float _moveInputy;

        private float _normalGravity;

        private Rigidbody2D _rb;
        private RelativeJoint2D _relativeJoint2D;

        private Coroutine _resetTriggerCoroutine;
        private readonly float idleThreshold = 5f;

        [HideInInspector] public bool canMove;
        private float _jumpCooldown = 0.5f; 
        [HideInInspector] public bool canJump;

        private float idleTimer;
        
        public static PlayerController Instance;

        public ParticleSystem Part;
        public bool particuleSystemON =false;

        public AudioClip[] cityFootstepSounds;
        public AudioClip[] forestFootstepSounds;
        public AudioClip[] lampSounds;
        public AudioClip jumpSound;
        [HideInInspector] public AudioSource audioSource;
        private int lastFootstepIndex = -1;
        private int lastLampIndex = -1;
        public bool isInCity = true;

        public float footstepVolume = 0.1f;
        public float jumpVolume = 0.5f;
        public float lampVolume = 0.3f;

        public ParticleSystem feetParticleSystem;

        private void Awake() {
            _playerLight = GetComponent<Light2D>();
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


        private void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _relativeJoint2D = GetComponent<RelativeJoint2D>();
            //     _anim = GetComponent<Animator>();
            _coll = GetComponent<Collider2D>();
            _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
            StartDirectionCheck();
            _fallSpeedYDampingChangeThreshold = CameraManager.Instance._fallSpeedYDampingChangeThreshold;
            canMove = true;
            
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = footstepVolume;
            audioSource.volume = lampVolume;
            
        }

        private void Update()
        {
            if (canMove)
            {
                if (canJump)
                {
                    Jump();
                }
            }
            
            GrabBox();
            ReleaseBox();
            if (_rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.Instance.IsLerpingYDamping &&
                !CameraManager.Instance.LerpedFromPlayerFalling) CameraManager.Instance.LerpYDamping(true);

            if (_rb.velocity.y >= 0f && !CameraManager.Instance.IsLerpingYDamping &&
                CameraManager.Instance.LerpedFromPlayerFalling)
            {
                CameraManager.Instance.LerpedFromPlayerFalling = false;
                CameraManager.Instance.LerpYDamping(false);
            }
            
            _anim.SetBool("IsWalking", _moveInputx != 0);

            if (_moveInputx != 0 && _isGrounded && !audioSource.isPlaying)
            {
                PlayRandomFootstep();
                feetParticleSystem.Play();
            }

            else
            {
                feetParticleSystem.Stop();
            }

            _anim.SetBool("IsJumping", _isJumping);

            _anim.SetBool("IsFalling", _isFalling);

            _anim.SetBool("IsClimbing", IsClimbing);

            _anim.SetBool("IsFalling", !_isJumping && !_isGrounded);

            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold) _anim.SetBool("IsDancing", true);

            if (_moveInputx != 0 || _isJumping)
            {
                idleTimer = 0;
                _anim.SetBool("IsDancing", false);
            }

            if (!canMove)
            { 
                _anim.SetBool("IsWalking", false);
            }

            if (_isMoving)
            {
                particuleSystemON = true;
            }
            
        }

        private void FixedUpdate()
        {
            if (canMove)
            {
                Climb();
                Move();
            }
        }

        #region Jump Function

        private void Jump()
        {
            if (InputManager.instance.JumpJustPressed && (_isGrounded || IsClimbing))
            {
                _isJumping = true;
                _jumpTimeCounter = _jumpTime;
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                canJump = false; 
                StartCoroutine(JumpCooldown());
            }

            if (InputManager.instance.JumpJustPressed && _isGrounded)
            {
                JumpSound();
            }

            if (InputManager.instance.JumpBeingHeld)
            {
                if (_jumpTimeCounter > 0 && _isJumping)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                    _jumpTimeCounter -= Time.deltaTime;
                }

                else if (_jumpTimeCounter == 0)
                {
                    _isFalling = true;
                    _isJumping = false;
                }

                else
                {
                    _isJumping = false;
                }
            }

            if (InputManager.instance.JumpReleased)
            {
                _isJumping = false;
                _isFalling = true;
            }

            if (!_isJumping && CheckForLand())
                //            _anim.SetTrigger("land");
                _resetTriggerCoroutine = StartCoroutine(Reset());
        }
        
        IEnumerator JumpCooldown()
        {
            yield return new WaitForSeconds(_jumpCooldown);
            canJump = true; 
        }

        #endregion

        #region Climb Function

        private void Climb()
        {
            _moveInputx = InputManager.instance.MoveInput.x;
            _moveInputy = InputManager.instance.MoveInput.y;

            if (IsClimbing)
            {
                _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _moveInputy * _moveSpeed);
                _rb.gravityScale = 0f;
            }

            else
            {
                _rb.gravityScale = 7f;
            }
        }

        #endregion

        /*private void DrawGroundCheck()
        {
            Color rayColor;

            if (_isGrounded)
                rayColor = Color.green;

            else
                rayColor = Color.red;

            Debug.DrawRay(_coll.bounds.center + new Vector3(_coll.bounds.extents.x, 0),
                Vector2.down * (_coll.bounds.extents.y + extraHeight), rayColor);
            Debug.DrawRay(_coll.bounds.center - new Vector3(_coll.bounds.extents.x, 0),
                Vector2.down * (_coll.bounds.extents.y + extraHeight), rayColor);
            Debug.DrawRay(
                _coll.bounds.center - new Vector3(_coll.bounds.extents.x, _coll.bounds.extents.y + extraHeight),
                Vector2.right * (_coll.bounds.extents.x * 2), rayColor);
        }*/

        #region Movement Functions

        private void Move()
        {
            _moveInputx = InputManager.instance.MoveInput.x;
            
            if (_moveInputx > 0 || _moveInputx < 0) TurnCheck();
            if (IsOnPlatform)
            {
                if (IsMoving())
                    _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _rb.velocity.y);
                else if (IsMoving() == false)
                    _rb.velocity = new Vector2(_moveInputx * _moveSpeed + PlatformRb.velocity.x, _rb.velocity.y);
            }
            else
            {
                _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _rb.velocity.y);
            }
        }

        private bool IsMoving()
        {
            if (_rb.velocity.x != 0 && _rb.velocity.x != PlatformRb.velocity.x) return true;
            return false;
        }

        #endregion

        #region Ground/Landed + Push/Pull Functions

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Tags.CompareTags("Movable", other.gameObject))
            {
                _canMoveBox = true;
                _movableBox = other.gameObject;
            }

            if (other.CompareTag("Movable,Ground")) _isGrounded = true; canJump = true;

            if (other.CompareTag("Ground")) _isGrounded = true; canJump = true;
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (Tags.CompareTags("Movable", other.gameObject)) _canMoveBox = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (Tags.CompareTags("Ground", other.gameObject)) _isGrounded = false;
            canJump = false;
        }

        private void GrabBox()
        {
            if (_isGrounded)
            {
                if (InputManager.instance.PushPullBeingHeld && _canMoveBox && _movableBox != null)
                {
                    _movableRigidbody2D = _movableBox.GetComponent<Rigidbody2D>();

                    if (_movableRigidbody2D != null)
                    {
                        // Freeze all constraints
                        _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezeAll;

                        // Freeze rotation on the Z-axis
                        _movableRigidbody2D.constraints |= RigidbodyConstraints2D.FreezeRotation;

                        _relativeJoint2D.enabled = true;
                        _relativeJoint2D.connectedBody = _movableRigidbody2D;

                        // Determine if pushing or pulling
                        if (_movableRigidbody2D.velocity.x > 0)
                        {
                            // Play pushing animation
                            if (IsFacingRight)
                            {
                                _anim.SetBool("IsPulling", false);
                                _anim.SetBool("IsPushing", true);
                            }
                            else
                            {
                                _anim.SetBool("IsPushing", false);
                                _anim.SetBool("IsPulling", true);
                            }
                        }
                        else if (_movableRigidbody2D.velocity.x < 0)
                        {
                            if (IsFacingRight)
                            {
                                _anim.SetBool("IsPushing", false);
                                _anim.SetBool("IsPulling", true);
                            }
                            else
                            {
                                _anim.SetBool("IsPulling", false);
                                _anim.SetBool("IsPushing", true);
                            }
                        }
                    }
                }
            }
            else if (!_isGrounded)
            {
                if (_relativeJoint2D != null)
                {
                    _relativeJoint2D.enabled = false;
                    _relativeJoint2D.connectedBody = null;
                }
            }
        }

        private void ReleaseBox()
        {
            if (InputManager.instance.PushPullReleased && _movableBox != null)
            {
                _movableRigidbody2D = _movableBox.GetComponent<Rigidbody2D>();
                if (_movableRigidbody2D != null)
                {
                    _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezeAll;
                    _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezePositionY;

                    if (_relativeJoint2D != null)
                    {
                        _relativeJoint2D.enabled = false;
                        _relativeJoint2D.connectedBody = null;
                    }
                }

                _anim.SetBool("IsPushing", false);
                _anim.SetBool("IsPulling", false);
            }
        }


        private bool CheckForLand()
        {
            if (_isFalling)
            {
                if (_isGrounded)
                {
                    _isFalling = false;
                    return true;
                }

                return false;
            }

            return false;
        }

        private IEnumerator Reset()
        {
            yield return null;

//            _anim.ResetTrigger("land");
        }

        #endregion

        #region Turn Checks

        private void StartDirectionCheck()
        {
            if (_rightLeg.transform.position.x > _leftLeg.transform.position.x)
                IsFacingRight = true;

            else
                IsFacingRight = false;
        }

        private void TurnCheck()
        {
            if (InputManager.instance.MoveInput.x > 0 && !IsFacingRight)
                Turn();

            else if (InputManager.instance.MoveInput.x < 0 && IsFacingRight) Turn();
        }

        private void Turn()
        {
            if (IsFacingRight)
            {
                Vector3 rotator = new(0f, 180f, 0f);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }

            else
            {
                Vector3 rotator = new(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }
        }

        #endregion
        
        void PlayRandomFootstep()
        {
            AudioClip[] currentFootstepSounds = isInCity ? cityFootstepSounds : forestFootstepSounds;
            int randomIndex = GetRandomFootstepIndex(currentFootstepSounds.Length);

            if (randomIndex != -1)
            {
                audioSource.clip = currentFootstepSounds[randomIndex];
                audioSource.Play();
                lastFootstepIndex = randomIndex;
            }
        }
        
        public void PlayRandomLampSound()
        {
            int randomIndex = GetRandomLampIndex();

            if (randomIndex != -1)
            {
                audioSource.clip = lampSounds[randomIndex];
                audioSource.Play();
                lastLampIndex = randomIndex;
            }
        }

        void JumpSound()
        {
            if (jumpSound != null)
            {
                audioSource.clip = jumpSound;
                audioSource.volume = jumpVolume; // Set the volume for the jump sound
                audioSource.Play();
                audioSource.volume = footstepVolume; // Reset volume to footstep volume
            }
        }

        int GetRandomFootstepIndex(int arrayLength)
        {
            if (arrayLength == 0)
            {
                Debug.LogWarning("No footstep sounds assigned.");
                return -1;
            }

            int randomIndex = Random.Range(0, arrayLength);

            // Ensure the next sound is different from the last one
            if (arrayLength > 1)
            {
                while (randomIndex == lastFootstepIndex)
                {
                    randomIndex = Random.Range(0, arrayLength);
                }
            }

            return randomIndex;
        }
        
        int GetRandomLampIndex()
        {
            if (lampSounds.Length == 0)
            {
                Debug.LogWarning("No lamp sounds assigned.");
                return -1;
            }

            int randomIndex = Random.Range(0, lampSounds.Length);

            // Ensure the next sound is different from the last one
            if (lampSounds.Length > 1)
            {
                while (randomIndex == lastLampIndex)
                {
                    randomIndex = Random.Range(0, lampSounds.Length);
                }
            }

            return randomIndex;
        }
    }
}