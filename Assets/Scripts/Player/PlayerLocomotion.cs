using FMODUnity;
using Sound.FloorMaterials;
using UnityEngine;

namespace Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private InputManager _inputManager;
        private Rigidbody _rb;
        private Camera _camera;
        private SurfaceDetector _surfaceDetector;

        private Vector3 _moveDirection;

        [Header("Step Sound")]
        [SerializeField] private StudioEventEmitter _stepSoundController;

        [Header("Scuff Sound")]
        [SerializeField] private StudioEventEmitter _scuffSoundController;
        [SerializeField] private float _scuffCooldown = 0.5f;

        private float _scuffCooldownTimer;
        private bool _wasMoving;

        [Header("Movement Flags")]
        public bool IsSprinting;
        public bool IsWalking;

        [Header("Movement Speed")]
        [SerializeField] private float _walkingSpeed = 2f;
        [SerializeField] private float _runningSpeed = 4f;
        [SerializeField] private float _sprintingSpeed = 6f;

        private float _currentSpeed;

        public float CurrentSpeed => _currentSpeed;
        public float RunningSpeed => _runningSpeed;

        public float ActualSpeed
        {
            get
            {
                Vector3 velocity = _rb.velocity;
                velocity.y = 0f;

                return velocity.magnitude;
            }
        }

        public bool IsMoving => ActualSpeed > 0.05f;

        [Header("FOV")]
        [SerializeField] private float _speedChange = 30f;
        [SerializeField] private float _sprintingFov = 70f;

        private PlayerFOVController _fovController;
        private float _defaultFOV;

        [Header("Steps — Movement")]
        [SerializeField] private float _stepDistance = 1.7f;

        [Header("Steps — Rotation")]
        [SerializeField] private float _stepRotation = 140f;
        [SerializeField] private float _rotationStepMinSpeed = 10f;

        private float _distance;
        private float _rotationProgress;
        private float _rotationDirection;
        private float _lastYRotation;

        private bool _leftLeg = true;

        public float StepDistance => _distance;
        public float StepRotation => _rotationProgress;

        public float StepProgress
        {
            get
            {
                if (IsMoving)
                {
                    if (_stepDistance <= 0f) return 0f;
                    return Mathf.Clamp01(_distance / _stepDistance);
                }

                if (_stepRotation <= 0f) return 0f;
                return Mathf.Clamp01(_rotationProgress / _stepRotation);
            }
        }

        public bool IsFootstepMoment { get; private set; }


        private void Awake()
        {
            _inputManager = GetComponent<InputManager>();
            _rb = GetComponent<Rigidbody>();
            _playerManager = GetComponent<PlayerManager>();
            _surfaceDetector = GetComponent<SurfaceDetector>();
            _fovController = GetComponent<PlayerFOVController>();

            _camera = Camera.main;
            _lastYRotation = transform.eulerAngles.y;

            if (_camera != null) _defaultFOV = _camera.fieldOfView;
        }


        private void Update()
        {
            IsFootstepMoment = false;

            HandleSteps();
            HandleScuff();

            if (_scuffCooldownTimer > 0f) _scuffCooldownTimer -= Time.deltaTime;
        }


        public void HandleAllMovement()
        {
            if (_playerManager != null && _playerManager.isInteracting)
            {
                StopMovement();
                return;
            }

            HandleMovement();
        }


        private void HandleMovement()
        {
            _moveDirection = transform.forward * _inputManager.verticalInput;
            _moveDirection += transform.right * _inputManager.horizontalalInput;
            _moveDirection.y = 0f;

            if (_moveDirection.sqrMagnitude > 1f) _moveDirection.Normalize();

            if (IsSprinting)
            {
                _currentSpeed = _sprintingSpeed;
            }
            else if (IsWalking)
            {
                _currentSpeed = _walkingSpeed;
            }
            else if (_inputManager.moveAmount >= 0.5f)
            {
                _currentSpeed = _runningSpeed;
            }
            else
            {
                _currentSpeed = _walkingSpeed;
            }

            _moveDirection *= _currentSpeed;
            _rb.velocity = new Vector3(_moveDirection.x, _rb.velocity.y, _moveDirection.z);

            HandleFOV();
        }


        private void StopMovement()
        {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            _currentSpeed = 0f;

            HandleFOV();
        }


        private void HandleFOV()
        {
            if (!IsSprinting)
            {
                _fovController.Sprinting = false;
                return;
            }
            _fovController.Sprinting = true;
            _fovController.ChangeParametersFov(_sprintingFov, _speedChange);
        }


        private void HandleSteps()
        {
            float deltaTime = Time.deltaTime;

            if (IsMoving) HandleMovementStep(deltaTime);
            else HandleRotationStep(deltaTime);

            _lastYRotation = transform.eulerAngles.y;
        }


        private void HandleMovementStep(float deltaTime)
        {
            _rotationProgress = 0f;
            _distance += ActualSpeed * deltaTime;

            while (_distance >= _stepDistance && _stepDistance > 0f)
            {
                _distance -= _stepDistance;
                MakeStep();
            }
        }


        private void HandleRotationStep(float deltaTime)
        {
            _distance = 0f;

            float currentYRotation = transform.eulerAngles.y;
            float rotationDelta = Mathf.DeltaAngle(_lastYRotation, currentYRotation);

            if (Mathf.Abs(rotationDelta) < _rotationStepMinSpeed * deltaTime) return;

            float currentDirection = Mathf.Sign(rotationDelta);

            if (_rotationDirection != 0f && currentDirection != _rotationDirection) _rotationProgress = 0f;

            _rotationDirection = currentDirection;
            _rotationProgress += Mathf.Abs(rotationDelta);

            if (_rotationProgress >= _stepRotation)
            {
                _rotationProgress -= _stepRotation;
                MakeStep();
            }
        }


        private void MakeStep()
        {
            _leftLeg = !_leftLeg;
            IsFootstepMoment = true;

            _rotationProgress = 0f;
            _rotationDirection = 0f;

            PlayStepSound();
        }


        private void PlayStepSound()
        {
            if (_stepSoundController == null) return;

            SetSurfaceParameter(_stepSoundController);
            _stepSoundController.Play();
        }


        private void HandleScuff()
        {
            bool isMoving = IsMoving;

            if (_wasMoving && !isMoving && _scuffCooldownTimer <= 0f)
            {
                PlayScuffSound();
                _scuffCooldownTimer = _scuffCooldown;
            }

            _wasMoving = isMoving;
        }


        private void PlayScuffSound()
        {
            if (_scuffSoundController == null) return;

            SetSurfaceParameter(_scuffSoundController);
            _scuffSoundController.Play();
        }


        private void SetSurfaceParameter(StudioEventEmitter emitter)
        {
            if (_surfaceDetector == null) return;

            float surfaceValue = (float)_surfaceDetector.CurrentSurface;
            emitter.SetParameter("surface", surfaceValue);
        }


        private void OnValidate()
        {
            _walkingSpeed = Mathf.Max(0f, _walkingSpeed);
            _runningSpeed = Mathf.Max(0f, _runningSpeed);
            _sprintingSpeed = Mathf.Max(0f, _sprintingSpeed);
            _speedChange = Mathf.Max(0f, _speedChange);
            _stepDistance = Mathf.Max(0.01f, _stepDistance);
            _stepRotation = Mathf.Max(0.1f, _stepRotation);
            _rotationStepMinSpeed = Mathf.Max(0f, _rotationStepMinSpeed);
            _scuffCooldown = Mathf.Max(0f, _scuffCooldown);
        }
    }
}