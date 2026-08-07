using UnityEngine;

namespace Player
{
    public class HeadBob : MonoBehaviour
    {
        [System.Serializable]
        public class BobSettings
        {
            public float amplitude = 0.03f;
            public float smoothSpeed = 10f;
        }


        [Header("Camera")]
        [SerializeField] private Camera _playerCamera;


        [Header("Idle Bob")]
        [SerializeField] private BobSettings _idleBob = new BobSettings
        {
            amplitude = 0.015f,
            smoothSpeed = 5f
        };


        [Header("Walking Bob")]
        [SerializeField] private BobSettings _walkingBob = new BobSettings
        {
            amplitude = 0.035f,
            smoothSpeed = 10f
        };


        [Header("Running Bob")]
        [SerializeField] private BobSettings _runningBob = new BobSettings
        {
            amplitude = 0.055f,
            smoothSpeed = 12f
        };


        [Header("Sprinting Bob")]
        [SerializeField] private BobSettings _sprintingBob = new BobSettings
        {
            amplitude = 0.075f,
            smoothSpeed = 14f
        };


        [Header("Horizontal Bob")]
        [SerializeField] private float _horizontalMultiplier = 0.6f;


        [Header("Strafe Tilt")]
        [SerializeField] private float _strafeTiltAmount = 3f;
        [SerializeField] private float _strafeTiltSmoothSpeed = 8f;


        [Header("Step Tilt")]
        [SerializeField] private float _stepTiltAmount = 2f;
        [SerializeField] private float _stepTiltSmoothSpeed = 8f;


        [Header("Movement Fade")]
        [SerializeField] private float _movementFadeSpeed = 5f;


        [Header("External")]
        [SerializeField] private float _externalAmplitudeScale = 1f;


        private PlayerLocomotion _locomotion;
        private InputManager _inputManager;

        private Vector3 _initialCameraPosition;
        private Vector3 _targetPosition;

        private float _movementIntensity;

        private float _currentStrafeTilt;
        private float _currentStepTilt;

        private Vector3 _externalPositionOffset;

        private float _turnLungeRoll;
        private Vector3 _turnLungeOffset;


        /// <summary>
        /// Произошёл ли шаг в текущем кадре.
        /// </summary>
        public bool IsFootstepMoment
        {
            get
            {
                return _locomotion != null &&
                       _locomotion.IsFootstepMoment;
            }
        }


        /// <summary>
        /// Текущее значение bob.
        /// Можно использовать для FMOD/других систем.
        /// </summary>
        public float CurrentBobValue
        {
            get
            {
                if (_locomotion == null)
                    return 0f;

                float phase =
                    _locomotion.StepProgress *
                    Mathf.PI *
                    2f;

                return Mathf.Sin(phase);
            }
        }


        private void Awake()
        {
            _locomotion =
                GetComponentInParent<PlayerLocomotion>();

            _inputManager =
                GetComponentInParent<InputManager>();


            if (_playerCamera == null)
            {
                _playerCamera = GetComponent<Camera>();
            }


            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;
            }


            if (_playerCamera != null)
            {
                _initialCameraPosition =
                    _playerCamera.transform.localPosition;
            }
        }


        private void Update()
        {
            if (_playerCamera == null ||
                _locomotion == null)
            {
                return;
            }


            UpdateMovementIntensity();
            UpdateHeadBob();
            UpdateTilt();
            ApplyCamera();
        }


        private void UpdateMovementIntensity()
        {
            float target =
                _locomotion.IsMoving
                    ? 1f
                    : 0f;


            _movementIntensity = Mathf.MoveTowards(
                _movementIntensity,
                target,
                _movementFadeSpeed *
                Time.deltaTime
            );
        }


        private void UpdateHeadBob()
        {
            Vector3 idleOffset =
                ComputeIdleOffset();


            Vector3 movementOffset =
                ComputeMovementOffset();


            // Когда игрок стоит:
            // 100% idle bob.
            //
            // Когда движется:
            // idle постепенно исчезает,
            // movement bob появляется.
            Vector3 finalOffset =
                idleOffset *
                (1f - _movementIntensity)
                +
                movementOffset *
                _movementIntensity;


            _targetPosition =
                _initialCameraPosition +
                finalOffset;


            // Внешнее смещение.
            // Например Zoom.
            _targetPosition +=
                _externalPositionOffset;


            // Дополнительное смещение
            // при повороте.
            _targetPosition +=
                _turnLungeOffset;
        }


        private Vector3 ComputeIdleOffset()
        {
            float time =
                Time.time *
                _idleBob.smoothSpeed;


            float vertical =
                Mathf.Sin(time) *
                _idleBob.amplitude *
                _externalAmplitudeScale;


            float horizontal =
                Mathf.Cos(time * 0.5f) *
                _idleBob.amplitude *
                0.3f *
                _externalAmplitudeScale;


            return new Vector3(
                horizontal,
                vertical,
                0f
            );
        }


        private Vector3 ComputeMovementOffset()
        {
            float progress =
                _locomotion.StepProgress;


            /*
             * Один полный цикл =
             * один пройденный шаг.
             *
             * 0.00 = начало шага
             * 0.25 = первая фаза
             * 0.50 = противоположная фаза
             * 0.75 = возврат
             * 1.00 = следующий шаг
             */

            float phase =
                progress *
                Mathf.PI *
                2f;


            float amplitude =
                GetCurrentAmplitude();


            // Вертикальное движение.
            float vertical =
                -Mathf.Sin(phase) *
                amplitude;


            // Горизонтальное движение.
            float horizontal =
                Mathf.Sin(phase) *
                amplitude *
                _horizontalMultiplier;


            return new Vector3(
                horizontal,
                vertical,
                0f
            );
        }


        private float GetCurrentAmplitude()
        {
            if (_locomotion.IsSprinting)
            {
                return _sprintingBob.amplitude *
                       _externalAmplitudeScale;
            }


            if (_locomotion.CurrentSpeed >=
                _locomotion.RunningSpeed)
            {
                return _runningBob.amplitude *
                       _externalAmplitudeScale;
            }


            return _walkingBob.amplitude *
                   _externalAmplitudeScale;
        }


        private float GetCurrentSmoothSpeed()
        {
            if (!_locomotion.IsMoving)
            {
                return _idleBob.smoothSpeed;
            }


            if (_locomotion.IsSprinting)
            {
                return _sprintingBob.smoothSpeed;
            }


            if (_locomotion.CurrentSpeed >=
                _locomotion.RunningSpeed)
            {
                return _runningBob.smoothSpeed;
            }


            return _walkingBob.smoothSpeed;
        }


        private void UpdateTilt()
        {
            float strafeInput = 0f;


            if (_inputManager != null)
            {
                strafeInput =
                    _inputManager.horizontalalInput;
            }


            // Наклон от движения A/D.
            float targetStrafeTilt =
                -strafeInput *
                _strafeTiltAmount *
                _movementIntensity;


            _currentStrafeTilt = Mathf.Lerp(
                _currentStrafeTilt,
                targetStrafeTilt,
                _strafeTiltSmoothSpeed *
                Time.deltaTime
            );


            // Наклон в ритме шага.
            float phase =
                _locomotion.StepProgress *
                Mathf.PI *
                2f;


            float targetStepTilt =
                Mathf.Sin(phase) *
                _stepTiltAmount *
                _movementIntensity;


            _currentStepTilt = Mathf.Lerp(
                _currentStepTilt,
                targetStepTilt,
                _stepTiltSmoothSpeed *
                Time.deltaTime
            );


            float totalTilt =
                _currentStrafeTilt +
                _currentStepTilt +
                _turnLungeRoll;


            totalTilt *=
                _externalAmplitudeScale;


            /*
             * CameraController отвечает
             * за X/Y вращение.
             *
             * HeadBob отвечает только
             * за Z.
             */

            Vector3 euler =
                _playerCamera.transform
                    .localEulerAngles;


            euler.z = totalTilt;


            _playerCamera.transform.localRotation =
                Quaternion.Euler(euler);
        }


        private void ApplyCamera()
        {
            float smoothSpeed =
                GetCurrentSmoothSpeed();


            _playerCamera.transform.localPosition =
                Vector3.Lerp(
                    _playerCamera.transform.localPosition,
                    _targetPosition,
                    smoothSpeed *
                    Time.deltaTime
                );
        }


        // =========================================================
        // EXTERNAL API
        // =========================================================

        /// <summary>
        /// Управление силой HeadBob извне.
        ///
        /// 1 = обычный bob
        /// 0 = полностью отключён
        /// </summary>
        public void SetExternalAmplitudeScale(
            float scale)
        {
            _externalAmplitudeScale =
                Mathf.Clamp01(scale);
        }


        /// <summary>
        /// Внешнее смещение камеры.
        /// Используется Zoom и другими системами.
        /// </summary>
        public void SetExternalPositionOffset(
            Vector3 offset)
        {
            _externalPositionOffset =
                offset;
        }


        /// <summary>
        /// Дополнительное смещение камеры
        /// при повороте.
        /// </summary>
        public void SetTurnLungeOffset(
            Vector3 offset)
        {
            _turnLungeOffset =
                offset;
        }


        /// <summary>
        /// Дополнительный Z-наклон камеры.
        /// </summary>
        public void SetTurnLungeRoll(
            float degrees)
        {
            _turnLungeRoll =
                degrees;
        }


        public float GetStepProgress()
        {
            if (_locomotion == null)
                return 0f;

            return _locomotion.StepProgress;
        }


        /// <summary>
        /// Проверка нижней точки bob.
        /// Можно использовать для синхронизации
        /// других эффектов.
        /// </summary>
        public bool IsAtLowestPoint(
            float threshold = 0.95f)
        {
            if (_locomotion == null)
                return false;


            float value =
                Mathf.Sin(
                    _locomotion.StepProgress *
                    Mathf.PI *
                    2f
                );


            return value <= -threshold;
        }
    }
}