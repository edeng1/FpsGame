// Designed by Kinemation, 2023

using UnityEngine;
using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.FPSAnimator;
using Kinemation.FPSFramework.Runtime.Layers;
using Kinemation.FPSFramework.Runtime.Recoil;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Demo.Scripts.Runtime.Base
{
    public enum FPSMovementState
    {
        Idle,
        Walking,
        Running,
        Sprinting
    }

    public enum FPSPoseState
    {
        Standing,
        Crouching
    }

    public enum FPSActionState
    {
        None,
        Ready,
        Aiming,
        PointAiming,
    }

    public enum FPSCameraState
    {
        Default,
        Barrel,
        InFront
    }

    // An example-controller class
    public class FPSController : FPSAnimController
    {
        [Tab("Animation")] [Header("General")] [SerializeField]
        private Animator animator;

        [SerializeField] private float turnInPlaceAngle;
        [SerializeField] private AnimationCurve turnCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField] private float turnSpeed = 1f;
        
        [Header("Dynamic Motions")] 
        [SerializeField] private DynamicMotion aimMotion;
        [SerializeField] private DynamicMotion leanMotion;

        // Animation Layers
        [SerializeField] [HideInInspector] private LookLayer lookLayer;
        [SerializeField] [HideInInspector] private AdsLayer adsLayer;
        [SerializeField] [HideInInspector] private SwayLayer swayLayer;
        [SerializeField] [HideInInspector] private LocomotionLayer locoLayer;
        [SerializeField] [HideInInspector] private SlotLayer slotLayer;
        // Animation Layers

        [Tab("Controller")] 
        [Header("General")] 
        [SerializeField] private CharacterController controller;
        
        [Header("Camera")] 
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform firstPersonCamera;
        [SerializeField] private float sensitivity;
        [SerializeField] private Vector2 freeLookAngle;

        [Header("Movement")] 
        [SerializeField] private float moveSmoothing = 2f;
        [SerializeField] private float sprintSpeed = 3f;
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float crouchSpeed = 1f;
        [SerializeField] private float crouchRatio = 0.5f;
        private float speed;

        [Tab("Weapon")] 
        [SerializeField] private List<Weapon> weapons;
        [SerializeField] public FPSCameraShake shake;

        private Vector2 _playerInput;

        // Used for free-look
        private Vector2 _freeLookInput;
        private Vector2 _smoothAnimatorMove;
        private Vector2 _smoothMove;

        private int _index;
        private int _lastIndex;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool _freeLook;
        private bool _hasActiveAction;
        
        private FPSActionState actionState;
        private FPSMovementState movementState;
        private FPSPoseState poseState;
        private FPSCameraState cameraState = FPSCameraState.Default;
        
        private float originalHeight;
        private Vector3 originalCenter;

        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int OverlayType = Animator.StringToHash("OverlayType");
        private static readonly int TurnRight = Animator.StringToHash("TurnRight");
        private static readonly int TurnLeft = Animator.StringToHash("TurnLeft");

        private void InitLayers()
        {
            InitAnimController(UpdateCameraRotation);

            controller = GetComponentInChildren<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            lookLayer = GetComponentInChildren<LookLayer>();
            adsLayer = GetComponentInChildren<AdsLayer>();
            locoLayer = GetComponentInChildren<LocomotionLayer>();
            swayLayer = GetComponentInChildren<SwayLayer>();
            slotLayer = GetComponentInChildren<SlotLayer>();
        }

        private void Start()
        {
            Time.timeScale = 1f;

            Application.targetFrameRate = 120;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            speed = walkingSpeed;
            
            originalHeight = controller.height;
            originalCenter = controller.center;

            moveRotation = transform.rotation;

            InitLayers();
            EquipWeapon();
        }

        public void SetActionActive(int isActive)
        {
            _hasActiveAction = isActive != 0;
        }

        private void EquipWeapon()
        {
            if (weapons.Count == 0) return;

            weapons[_lastIndex].gameObject.SetActive(false);
            var gun = weapons[_index];

            _bursts = gun.burstAmount;
            animator.SetFloat(OverlayType, (float) gun.overlayType);
            StopAnimation(0.1f);
            InitWeapon(gun);
            gun.gameObject.SetActive(true);
        }

        private void ChangeWeapon()
        {
            if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;
            
            int newIndex = _index;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            _lastIndex = _index;
            _index = newIndex;

            EquipWeapon();
        }

        public void ToggleAim()
        {
            _aiming = !_aiming;

            if (_aiming)
            {
                actionState = FPSActionState.Aiming;
                adsLayer.SetAds(true);
                swayLayer.SetFreeAimEnable(false);
                slotLayer.PlayMotion(aimMotion);
            }
            else
            {
                actionState = FPSActionState.None;
                adsLayer.SetAds(false);
                adsLayer.SetPointAim(false);
                swayLayer.SetFreeAimEnable(true);
                slotLayer.PlayMotion(aimMotion);
            }

            recoilComponent.isAiming = _aiming;
        }

        public void ChangeScope()
        {
            InitAimPoint(GetGun());
        }

        private void Fire()
        {
            GetGun().OnFire();
            recoilComponent.Play();
            PlayCameraShake(shake);
        }

        private void OnFirePressed()
        {
            if (weapons.Count == 0) return;

            Fire();
            _bursts = GetGun().burstAmount - 1;
            _fireTimer = 0f;
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        private void OnFireReleased()
        {
            recoilComponent.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            if (poseState == FPSPoseState.Crouching || _hasActiveAction)
            {
                return;
            }

            lookLayer.SetLayerAlpha(0.5f);
            adsLayer.SetLayerAlpha(0f);
            locoLayer.SetReadyWeight(0f);

            movementState = FPSMovementState.Sprinting;
            actionState = FPSActionState.None;

            recoilComponent.Stop();

            speed = sprintSpeed;
        }

        private void SprintReleased()
        {
            if (poseState == FPSPoseState.Crouching)
            {
                return;
            }

            lookLayer.SetLayerAlpha(1f);
            adsLayer.SetLayerAlpha(1f);
            movementState = FPSMovementState.Walking;

            speed = walkingSpeed;
        }

        private void Crouch()
        {
            //todo: crouching implementation
            
            float crouchedHeight = originalHeight * crouchRatio;
            float heightDifference = originalHeight - crouchedHeight;

            controller.height = crouchedHeight;

            // Adjust the center position so the bottom of the capsule remains at the same position
            Vector3 crouchedCenter = originalCenter;
            crouchedCenter.y -= heightDifference / 2;
            controller.center = crouchedCenter;

            speed = crouchSpeed;

            lookLayer.SetPelvisWeight(0f);

            poseState = FPSPoseState.Crouching;
            animator.SetBool(Crouching, true);
        }

        private void Uncrouch()
        {
            //todo: crouching implementation
            controller.height = originalHeight;
            controller.center = originalCenter;

            speed = walkingSpeed;

            lookLayer.SetPelvisWeight(1f);

            poseState = FPSPoseState.Standing;
            animator.SetBool(Crouching, false);
        }

        private void UpdateActionInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

                if (GetGun().reloadClip == null) return;
                PlayAnimation(GetGun().reloadClip);
                GetGun().Reload();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

                if (GetGun().grenadeClip == null) return;
                PlayAnimation(GetGun().grenadeClip);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                StopAnimation(0.2f);
            }

            charAnimData.leanDirection = 0;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SprintPressed();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SprintReleased();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon();
            }

            if (movementState == FPSMovementState.Sprinting)
            {
                return;
            }

            if (actionState != FPSActionState.Ready)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.Q)
                                                || Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.E))
                {
                    slotLayer.PlayMotion(leanMotion);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    charAnimData.leanDirection = 1;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    charAnimData.leanDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnFirePressed();
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnFireReleased();
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    ToggleAim();
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }

                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (actionState == FPSActionState.PointAiming)
                    {
                        adsLayer.SetPointAim(false);
                        actionState = FPSActionState.Aiming;
                    }
                    else
                    {
                        adsLayer.SetPointAim(true);
                        actionState = FPSActionState.PointAiming;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (poseState == FPSPoseState.Standing)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (actionState == FPSActionState.Ready)
                {
                    actionState = FPSActionState.None;
                    locoLayer.SetReadyWeight(0f);
                    lookLayer.SetLayerAlpha(1f);
                }
                else
                {
                    actionState = FPSActionState.Ready;
                    locoLayer.SetReadyWeight(1f);
                    lookLayer.SetLayerAlpha(.5f);
                    OnFireReleased();
                }
            }
        }

        private Quaternion desiredRotation;
        private Quaternion moveRotation;
        private float turnProgress = 1f;
        private bool isTurning = false;

        private void TurnInPlace()
        {
            float turnInput = _playerInput.x;
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            turnInput -= _playerInput.x;

            float sign = Mathf.Sign(_playerInput.x);
            if (Mathf.Abs(_playerInput.x) > turnInPlaceAngle)
            {
                if (!isTurning)
                {
                    turnProgress = 0f;
                    
                    animator.ResetTrigger(TurnRight);
                    animator.ResetTrigger(TurnLeft);
                    
                    animator.SetTrigger(sign > 0f ? TurnRight : TurnLeft);
                }
                
                isTurning = true;
            }

            transform.rotation *= Quaternion.Euler(0f, turnInput, 0f);
            
            float lastProgress = turnCurve.Evaluate(turnProgress);
            turnProgress += Time.deltaTime * turnSpeed;
            turnProgress = Mathf.Min(turnProgress, 1f);
            
            float deltaProgress = turnCurve.Evaluate(turnProgress) - lastProgress;

            _playerInput.x -= sign * turnInPlaceAngle * deltaProgress;
            
            transform.rotation *= Quaternion.Slerp(Quaternion.identity,
                Quaternion.Euler(0f, sign * turnInPlaceAngle, 0f), deltaProgress);
            
            if (Mathf.Approximately(turnProgress, 1f) && isTurning)
            {
                isTurning = false;
            }
        }

        private void UpdateLookInput()
        {
            _freeLook = Input.GetKey(KeyCode.X);

            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            if (_freeLook)
            {
                // No input for both controller and animation component. We only want to rotate the camera

                _freeLookInput.x += deltaMouseX;
                _freeLookInput.y += deltaMouseY;

                _freeLookInput.x = Mathf.Clamp(_freeLookInput.x, -freeLookAngle.x, freeLookAngle.x);
                _freeLookInput.y = Mathf.Clamp(_freeLookInput.y, -freeLookAngle.y, freeLookAngle.y);

                return;
            }

            _freeLookInput = FPSAnimLib.ExpDecay(_freeLookInput, Vector2.zero, 15f, Time.deltaTime);
            
            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);
            moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
            TurnInPlace();

            float moveWeight = Mathf.Clamp01(Mathf.Abs(_smoothMove.magnitude));
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, moveWeight);
            _playerInput.x *= 1f - moveWeight;

            charAnimData.SetAimInput(_playerInput);
            charAnimData.AddDeltaInput(new Vector2(deltaMouseX, charAnimData.deltaAimInput.y));
        }

        private void UpdateFiring()
        {
            if (recoilComponent == null) return;
            
            if (recoilComponent.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (recoilComponent.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }
        }

        private bool IsZero(float value)
        {
            return Mathf.Approximately(0f, value);
        }

        private void UpdateMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            Vector2 rawInput = new Vector2(moveX, moveY);
            Vector2 normInput = new Vector2(moveX, moveY);
            normInput.Normalize();
            
            if ((IsZero(normInput.y) || !IsZero(normInput.x)) 
                && movementState == FPSMovementState.Sprinting)
            {
                SprintReleased();
            }
            
            if (movementState == FPSMovementState.Sprinting)
            {
                normInput.x = rawInput.x = 0f;
                normInput.y = rawInput.y = 2f;
            }

            _smoothMove = FPSAnimLib.ExpDecay(_smoothMove, normInput, moveSmoothing, Time.deltaTime);

            moveX = _smoothMove.x;
            moveY = _smoothMove.y;
            
            charAnimData.moveInput = normInput;

            _smoothAnimatorMove.x = FPSAnimLib.ExpDecay(_smoothAnimatorMove.x, rawInput.x, 5f, Time.deltaTime);
            _smoothAnimatorMove.y = FPSAnimLib.ExpDecay(_smoothAnimatorMove.y, rawInput.y, 4f, Time.deltaTime);

            bool moving = Mathf.Approximately(0f, normInput.magnitude);

            animator.SetBool(Moving, !moving);
            animator.SetFloat(MoveX, _smoothAnimatorMove.x);
            animator.SetFloat(MoveY, _smoothAnimatorMove.y);
            
            Vector3 move = transform.right * moveX + transform.forward * moveY;
            controller.Move(move * speed * Time.deltaTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit(0);
            }

            UpdateActionInput();
            UpdateLookInput();
            UpdateFiring();
            UpdateMovement();
            UpdateAnimController();
        }

        private void UpdateCameraRotation()
        {
            Vector2 finalInput = new Vector2(_playerInput.x, _playerInput.y);
            (Quaternion, Vector3) cameraTransform =
                (transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f),
                    firstPersonCamera.position);

            if (cameraState == FPSCameraState.InFront)
            {
                //cameraTransform = (frontCamera.rotation, frontCamera.position);
            }

            if (cameraState == FPSCameraState.Barrel)
            {
                //cameraTransform = (barrelCamera.rotation, barrelCamera.position);
            }

            cameraHolder.rotation = cameraTransform.Item1;
            cameraHolder.position = cameraTransform.Item2;

            mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
        }
    }
}