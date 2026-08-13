using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Система стамины и стресса игрока.
    ///
    /// Unity отвечает только за расчёт игровых параметров:
    ///
    /// Stamina 0..1  → FMOD stamina
    /// Stress  0..1  → FMOD stress
    /// Idle/Walk/Run → FMOD playerState
    ///
    /// Вся аудиологика, пороги отдышки, переходы и сглаживание
    /// обрабатываются непосредственно в FMOD.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [Header("FMOD")]
        [Tooltip("FMOD Event дыхания.")]
        [SerializeField] private EventReference _breathEvent;

        [Tooltip("Параметр состояния движения: Idle = 0, Walk = 1, Run = 2.")]
        [SerializeField] private string _fmodPlayerStateParam = "playerState";

        [Tooltip("Параметр стамины 0..1.")]
        [SerializeField] private string _fmodStaminaParam = "stamina";

        [Tooltip("Параметр стресса 0..1.")]
        [SerializeField] private string _fmodStressParam = "stress";


        [Header("Stamina — расход")]
        [Tooltip("Расход стамины в секунду при обычной ходьбе.")]
        [SerializeField] private float _walkDrainPerSec = 0.045f;

        [Tooltip("Расход стамины в секунду при беге.")]
        [SerializeField] private float _runDrainPerSec = 0.16f;

        [Tooltip("Расход стамины в секунду при спринте.")]
        [SerializeField] private float _sprintDrainPerSec = 0.3f;

        [Tooltip("Скорость, выше которой движение считается бегом.")]
        [SerializeField] private float _runSpeedThreshold = 4f;


        [Header("Stamina — восстановление")]
        [Tooltip("Восстановление стамины в секунду в покое.")]
        [SerializeField] private float _idleRecoverPerSec = 0.12f;

        [Tooltip("Восстановление стамины в секунду при движении.")]
        [SerializeField] private float _moveRecoverPerSec = 0.03f;

        [Tooltip("Задержка перед началом восстановления стамины.")]
        [SerializeField] private float _recoverDelay = 0.6f;


        [Header("Stress")]
        [Tooltip("Скорость естественного снижения стресса.")]
        [SerializeField] private float _stressDecayPerSec = 0.15f;

        [Tooltip("Скорость изменения стресса к целевому значению.")]
        [SerializeField] private float _stressRisePerSec = 2f;


        [Header("Running")]
        [Tooltip("Учитывать расход стамины во время бега.")]
        [SerializeField] private bool _enableRunDrain = false;

        [Tooltip("Учитывать расход стамины во время спринта.")]
        [SerializeField] private bool _enableSprintDrain = true;


        [Header("Debug")]
        [SerializeField] private bool _showDebugLogs = false;
        [SerializeField] private bool _showDebugHUD = false;
        [SerializeField] private Vector2 _hudPosition = new Vector2(16f, 16f);


        // ============================================================
        // PUBLIC STATE
        // ============================================================

        /// <summary>
        /// Текущая стамина 0..1.
        /// </summary>
        public float Stamina { get; private set; } = 1f;


        /// <summary>
        /// Текущий стресс 0..1.
        /// </summary>
        public float Stress { get; private set; }


        /// <summary>
        /// true, если игрок сейчас находится под нагрузкой.
        /// </summary>
        public bool IsUnderLoad => _playerLocomotion != null &&
                                   _playerLocomotion.IsMoving;


        /// <summary>
        /// Текущий уровень стресса:
        /// 0 = низкий
        /// 50 = средний
        /// 100 = высокий
        ///
        /// Это значение можно использовать другими системами Unity.
        /// На FMOD напрямую не отправляется.
        /// </summary>
        public int StressLevel
        {
            get
            {
                if (Stress >= 1f)
                    return 100;

                if (Stress >= 0.5f)
                    return 50;

                return 0;
            }
        }


        // ============================================================
        // REFERENCES
        // ============================================================

        private PlayerLocomotion _playerLocomotion;
        private Rigidbody _rb;


        // ============================================================
        // INTERNAL STATE
        // ============================================================

        private float _stressTarget;
        private bool _stressHeld;

        private float _timeSinceLoad;

        private EventInstance _breathInstance;
        private bool _instanceValid;


        // ============================================================
        // UNITY
        // ============================================================

        private void Awake()
        {
            _playerLocomotion = GetComponent<PlayerLocomotion>();
            _rb = GetComponent<Rigidbody>();
        }


        private void Start()
        {
            CreateBreathInstance();

            DebugLog("PlayerStamina initialized.");
        }


        private void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateStamina(deltaTime);
            UpdateStress(deltaTime);
            PushToFmod();
        }


        // ============================================================
        // STAMINA
        // ============================================================

        private void UpdateStamina(float deltaTime)
        {
            if (_playerLocomotion == null)
                return;

            float speed = _playerLocomotion.ActualSpeed;
            bool isMoving = speed > 0.05f;

            bool isSprinting = _playerLocomotion.IsSprinting;
            bool isRunning = !isSprinting &&
                             speed >= _runSpeedThreshold;

            if (isSprinting && _enableSprintDrain)
            {
                Stamina -= _sprintDrainPerSec * deltaTime;
                _timeSinceLoad = 0f;
            }
            else if (isRunning && _enableRunDrain)
            {
                Stamina -= _runDrainPerSec * deltaTime;
                _timeSinceLoad = 0f;
            }
            else if (isMoving)
            {
                Stamina -= _walkDrainPerSec * deltaTime;
                _timeSinceLoad = 0f;
            }
            else
            {
                _timeSinceLoad += deltaTime;

                if (_timeSinceLoad >= _recoverDelay)
                {
                    Stamina += _idleRecoverPerSec * deltaTime;
                }
            }

            Stamina = Mathf.Clamp01(Stamina);
        }


        // ============================================================
        // STRESS
        // ============================================================

        private void UpdateStress(float deltaTime)
        {
            if (_stressHeld)
            {
                Stress = Mathf.MoveTowards(Stress, _stressTarget, _stressRisePerSec * deltaTime);
            }
            else
            {
                Stress = Mathf.MoveTowards(Stress, _stressTarget, _stressRisePerSec * deltaTime);

                _stressTarget = Mathf.MoveTowards(_stressTarget, 0f, _stressDecayPerSec * deltaTime);
            }

            Stress = Mathf.Clamp01(Stress);
        }


        // ============================================================
        // PUBLIC STRESS API
        // ============================================================

        /// <summary>
        /// Добавляет всплеск стресса.
        /// После этого стресс постепенно возвращается к нулю.
        /// </summary>
        public void AddStress(float amount)
        {
            _stressTarget = Mathf.Clamp01(_stressTarget + Mathf.Abs(amount));

            _stressHeld = false;

            DebugLog($"AddStress({amount:F2}) → target {_stressTarget:F2}");
        }


        /// <summary>
        /// Устанавливает стресс и удерживает его на этом уровне.
        /// </summary>
        public void SetStress(float level)
        {
            _stressTarget = Mathf.Clamp01(level);
            _stressHeld = true;

            DebugLog(
                $"SetStress({level:F2})"
            );
        }


        /// <summary>
        /// Снимает удержание стресса.
        /// После этого стресс начинает спадать.
        /// </summary>
        public void ReleaseStress()
        {
            _stressHeld = false;

            DebugLog("ReleaseStress()");
        }


        /// <summary>
        /// Принудительно устанавливает стамину.
        /// </summary>
        public void SetStamina(float value)
        {
            Stamina = Mathf.Clamp01(value);
        }


        // ============================================================
        // FMOD
        // ============================================================

        private void CreateBreathInstance()
        {
            if (_breathEvent.IsNull)
            {
                DebugLog("Breath Event is not assigned.");
                return;
            }

            _breathInstance = RuntimeManager.CreateInstance(
                _breathEvent
            );

            _breathInstance.start();

            _instanceValid = true;

            DebugLog("Breath FMOD instance started.");
        }


        private void PushToFmod()
        {
            if (!_instanceValid)
                return;

            // Stamina 0..1
            _breathInstance.setParameterByName(
                _fmodStaminaParam,
                Stamina
            );

            // Stress 0..1
            _breathInstance.setParameterByName(
                _fmodStressParam,
                Stress
            );

            // Idle = 0
            // Walk = 1
            // Run = 2
            _breathInstance.setParameterByName(
                _fmodPlayerStateParam,
                GetFmodPlayerState()
            );
        }


        private float GetFmodPlayerState()
        {
            if (_playerLocomotion == null || !_playerLocomotion.IsMoving)
                return 0f;

            if (_playerLocomotion.IsSprinting)
                return 2f;

            if (_playerLocomotion.ActualSpeed >= _playerLocomotion.RunningSpeed)
                return 2f;

            return 1f;
        }


        // ============================================================
        // DEBUG HUD
        // ============================================================

        private GUIStyle _hudBoxStyle;
        private GUIStyle _hudLabelStyle;


        private void OnGUI()
        {
            if (!_showDebugHUD)
                return;

            if (_hudBoxStyle == null)
            {
                _hudBoxStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 13,
                    alignment = TextAnchor.UpperLeft,
                    padding = new RectOffset(10, 10, 8, 8)
                };
            }

            if (_hudLabelStyle == null)
            {
                _hudLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    richText = true
                };
            }

            float x = _hudPosition.x;
            float y = _hudPosition.y;
            float width = 260f;
            float lineHeight = 18f;

            float height = 8 * lineHeight + 16f;

            GUI.color = new Color(0f, 0f, 0f, 0.55f);

            GUI.Box(
                new Rect(x, y, width, height),
                GUIContent.none,
                _hudBoxStyle
            );

            GUI.color = Color.white;

            float currentY = y + 8f;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                "<color=#99CCFF><b>[ PlayerStamina ]</b></color>",
                _hudLabelStyle
            );

            currentY += lineHeight + 2f;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Stamina:</color> {Stamina:F3}",
                _hudLabelStyle
            );

            currentY += lineHeight;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Stress:</color> {Stress:F3}",
                _hudLabelStyle
            );

            currentY += lineHeight;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Stress Level:</color> {StressLevel}",
                _hudLabelStyle
            );

            currentY += lineHeight;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Speed:</color> {_playerLocomotion?.ActualSpeed:F2}",
                _hudLabelStyle
            );

            currentY += lineHeight;

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Under Load:</color> {IsUnderLoad}",
                _hudLabelStyle
            );

            currentY += lineHeight;

            float playerState = GetFmodPlayerState();

            GUI.Label(
                new Rect(x + 10f, currentY, width - 20f, lineHeight),
                $"<color=#DDDDDD>Player State:</color> {playerState:F0}",
                _hudLabelStyle
            );
        }


        // ============================================================
        // LIFECYCLE
        // ============================================================

        private void OnDestroy()
        {
            if (!_instanceValid)
                return;

            _breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            _breathInstance.release();

            _instanceValid = false;
        }


        // ============================================================
        // VALIDATION
        // ============================================================

        private void OnValidate()
        {
            _walkDrainPerSec = Mathf.Max(0f, _walkDrainPerSec);
            _runDrainPerSec = Mathf.Max(0f, _runDrainPerSec);
            _sprintDrainPerSec = Mathf.Max(0f, _sprintDrainPerSec);

            _runSpeedThreshold = Mathf.Max(0f, _runSpeedThreshold);

            _idleRecoverPerSec = Mathf.Max(0f, _idleRecoverPerSec);
            _moveRecoverPerSec = Mathf.Max(0f, _moveRecoverPerSec);

            _recoverDelay = Mathf.Max(0f, _recoverDelay);

            _stressDecayPerSec = Mathf.Max(0f, _stressDecayPerSec);
            _stressRisePerSec = Mathf.Max(0.01f, _stressRisePerSec);
        }


        // ============================================================
        // DEBUG
        // ============================================================

        private void DebugLog(string message)
        {
            if (!_showDebugLogs)
                return;

            Debug.Log($"<color=#99CCFF>[PlayerStamina]</color> {message}");
        }
    }
}
