using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private SoundController _soundController;
        private InputManager _inputManager;
        private Rigidbody _rb;
        private Camera _camera;

        private Vector3 _moveDirection;

        [Header("Movement Flags")]
        public bool IsSprinting;

        [Header("Movement Speed")]
        [SerializeField] private float _walkingSpeed = 2f;
        [SerializeField] private float _runningSpeed = 4f;
        [SerializeField] private float _sprintingSpeed = 6f;

        private float _currentSpeed;

        public float CurrentSpeed => _currentSpeed;
        public float RunningSpeed => _runningSpeed;

        /// <summary>
        /// Реальная горизонтальная скорость игрока.
        /// </summary>
        public float ActualSpeed
        {
            get
            {
                Vector3 velocity = _rb.velocity;
                velocity.y = 0f;

                return velocity.magnitude;
            }
        }

        /// <summary>
        /// Действительно ли игрок движется.
        /// </summary>
        public bool IsMoving => ActualSpeed > 0.05f;

        [Header("FOV")]
        [SerializeField] private float _speedChange = 30f;
        [SerializeField] private float _sprintingFov = 70f;

        private float _defaultFOV;

        [Header("Steps")]
        [SerializeField] private float _stepDistance = 1.7f;

        [SerializeField] private Slider _leftSlider;
        [SerializeField] private Slider _rightSlider;

        private float _distance;
        private bool _leftLeg = true;

        /// <summary>
        /// Расстояние, пройденное текущей ногой.
        /// </summary>
        public float StepDistance => _distance;

        /// <summary>
        /// Прогресс текущего шага от 0 до 1.
        /// </summary>
        public float StepProgress
        {
            get
            {
                if (_stepDistance <= 0f)
                    return 0f;

                return Mathf.Clamp01(_distance / _stepDistance);
            }
        }

        /// <summary>
        /// true = левая нога.
        /// false = правая нога.
        /// </summary>
        public bool IsLeftLeg => _leftLeg;

        /// <summary>
        /// true только в кадре, когда завершился шаг.
        /// </summary>
        public bool IsFootstepMoment { get; private set; }


        private void Awake()
        {
            _inputManager = GetComponent<InputManager>();
            _soundController = GetComponentInChildren<SoundController>();
            _rb = GetComponent<Rigidbody>();
            _playerManager = GetComponent<PlayerManager>();

            _camera = Camera.main;

            if (_camera != null)
            {
                _defaultFOV = _camera.fieldOfView;
            }
        }


        private void Update()
        {
            // Сбрасываем событие шага каждый кадр.
            IsFootstepMoment = false;

            HandleSteps();
        }


        public void HandleAllMovement()
        {
            if (_playerManager != null &&
                _playerManager.isInteracting)
            {
                StopMovement();
                return;
            }

            HandleMovement();
        }


        private void HandleMovement()
        {
            _moveDirection =
                transform.forward * _inputManager.verticalInput;

            _moveDirection +=
                transform.right * _inputManager.horizontalalInput;

            // Не позволяем движению зависеть от наклона объекта.
            _moveDirection.y = 0f;

            // Если диагональное движение превышает 1,
            // ограничиваем его.
            if (_moveDirection.sqrMagnitude > 1f)
            {
                _moveDirection.Normalize();
            }


            // Определяем текущую скорость.
            if (IsSprinting)
            {
                _currentSpeed = _sprintingSpeed;
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


            // Сохраняем вертикальную скорость Rigidbody.
            _rb.velocity = new Vector3(
                _moveDirection.x,
                _rb.velocity.y,
                _moveDirection.z
            );


            HandleFOV();
        }


        private void StopMovement()
        {
            _rb.velocity = new Vector3(
                0f,
                _rb.velocity.y,
                0f
            );

            _currentSpeed = 0f;

            HandleFOV();
        }


        private void HandleFOV()
        {
            if (_camera == null)
                return;

            float targetFOV =
                IsSprinting && IsMoving
                    ? _sprintingFov
                    : _defaultFOV;


            _camera.fieldOfView = Mathf.MoveTowards(
                _camera.fieldOfView,
                targetFOV,
                _speedChange * Time.deltaTime
            );
        }


        private void HandleSteps()
        {
            if (!IsMoving)
            {
                ResetStepSliders();
                return;
            }


            // Скорость в units/sec × время = расстояние.
            float distanceThisFrame =
                ActualSpeed * Time.deltaTime;

            _distance += distanceThisFrame;


            // Игрок прошёл расстояние одного шага.
            if (_distance >= _stepDistance)
            {
                // Сохраняем остаток расстояния.
                _distance -= _stepDistance;

                // Меняем ногу.
                _leftLeg = !_leftLeg;

                // Сообщаем HeadBob и другим системам,
                // что произошёл шаг.
                IsFootstepMoment = true;

                EndStep();
            }


            UpdateStepSliders();
        }


        private void UpdateStepSliders()
        {
            float progress = StepProgress;


            if (_leftLeg)
            {
                if (_leftSlider != null)
                    _leftSlider.value = progress;

                if (_rightSlider != null)
                    _rightSlider.value = 0f;
            }
            else
            {
                if (_rightSlider != null)
                    _rightSlider.value = progress;

                if (_leftSlider != null)
                    _leftSlider.value = 0f;
            }
        }


        private void ResetStepSliders()
        {
            if (_leftSlider != null)
                _leftSlider.value = 0f;

            if (_rightSlider != null)
                _rightSlider.value = 0f;
        }


        private void EndStep()
        {
            if (_soundController != null)
            {
                _soundController.PlaySound();
            }
        }
    }
}