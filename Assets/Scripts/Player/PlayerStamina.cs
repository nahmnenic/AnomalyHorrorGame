using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Player
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("FMOD")]
        [SerializeField] private EventReference _breathEvent;
        [SerializeField] private string _fmodPlayerStateParam = "playerState";
        [SerializeField] private string _fmodStaminaParam = "stamina";
        [SerializeField] private string _fmodStressParam = "stress";
        [SerializeField] private bool _quantizeStressForFmod;

        [Header("Stamina — расход")]
        [SerializeField] private float _walkDrainPerSec = 0.045f;
        [SerializeField] private float _runDrainPerSec = 0.16f;
        [SerializeField] private float _sprintDrainPerSec = 0.25f;

        [Header("Stamina — восстановление")]
        [SerializeField] private float _idleRecoverPerSec = 0.12f;
        [SerializeField] private float _moveRecoverPerSec = 0.03f;
        [SerializeField] private float _recoverDelay = 0.6f;

        [Header("Stamina — порог одышки")]
        [Range(0f, 1f)]
        [SerializeField] private float _breathlessThreshold = 0.2f;

        [Header("Stress")]
        [SerializeField] private float _stressDecayPerSec = 0.15f;
        [SerializeField] private float _stressRisePerSec = 2f;

        [Header("Сглаживание FMOD")]
        [SerializeField] private float _fmodSmoothSpeed = 4f;

        [Header("FMOD 3D")]
        [SerializeField] private float _fmod3DUpdateThreshold = 0.05f;

        [Header("Debug")]
        [SerializeField] private bool _showDebugLogs;
        [SerializeField] private Color _debugColor = new Color(0.6f, 0.8f, 1f);
        [SerializeField] private bool _showDebugHUD;
        [SerializeField] private Vector2 _hudPosition = new Vector2(16f, 16f);

        public float Stamina { get; private set; } = 1f;
        public float Stress { get; private set; }

        public bool IsBreathless => Stamina < _breathlessThreshold;

        public int StressLevel
        {
            get
            {
                if (Stress >= 1f) return 100;
                if (Stress >= 0.5f) return 50;

                return 0;
            }
        }

        private PlayerLocomotion _locomotion;
        private Rigidbody _rb;

        private float _stressTarget;
        private bool _stressHeld;
        private float _timeSinceLoad;

        private float _fmodStaminaSmoothed = 1f;
        private float _fmodStressSmoothed;

        private EventInstance _breathInstance;
        private bool _instanceValid;

        private Vector3 _lastFmod3DPos;
        private bool _fmod3DPosInit;

        private GUIStyle _hudBoxStyle;
        private GUIStyle _hudLabelStyle;


        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _rb = GetComponent<Rigidbody>();

            if (_locomotion == null)
                Debug.LogError("[PlayerStamina] PlayerLocomotion не найден.");

            if (_rb == null)
                Debug.LogError("[PlayerStamina] Rigidbody не найден.");
        }


        private void Start()
        {
            CreateBreathInstance();
            DebugLog("PlayerStamina initialized", _debugColor);
        }


        private void Update()
        {
            float dt = Time.deltaTime;

            UpdateStamina(dt);
            UpdateStress(dt);
            PushToFmod(dt);
        }


        // =========================================================
        // STAMINA
        // =========================================================

        private void UpdateStamina(float dt)
        {
            if (_locomotion == null)
                return;

            bool isMoving = _locomotion.IsMoving;

            if (isMoving && _locomotion.IsSprinting)
            {
                Stamina -= _sprintDrainPerSec * dt;
                _timeSinceLoad = 0f;
            }
            else if (isMoving && _locomotion.CurrentSpeed >= _locomotion.RunningSpeed)
            {
                Stamina -= _runDrainPerSec * dt;
                _timeSinceLoad = 0f;
            }
            else if (isMoving)
            {
                Stamina -= _walkDrainPerSec * dt;
                _timeSinceLoad = 0f;
            }
            else
            {
                _timeSinceLoad += dt;

                if (_timeSinceLoad >= _recoverDelay)
                    Stamina += _idleRecoverPerSec * dt;
            }

            Stamina = Mathf.Clamp01(Stamina);
        }


        // =========================================================
        // STRESS
        // =========================================================

        private void UpdateStress(float dt)
        {
            Stress = Mathf.MoveTowards(Stress, _stressTarget, _stressRisePerSec * dt);

            if (!_stressHeld)
                _stressTarget = Mathf.MoveTowards(_stressTarget, 0f, _stressDecayPerSec * dt);

            Stress = Mathf.Clamp01(Stress);
        }


        // =========================================================
        // PUBLIC API
        // =========================================================

        public void AddStress(float amount)
        {
            _stressTarget = Mathf.Clamp01(_stressTarget + Mathf.Abs(amount));
            _stressHeld = false;

            DebugLog($"AddStress({amount:F2}) → target {_stressTarget:F2}", _debugColor);
        }


        public void SetStress(float level)
        {
            _stressTarget = Mathf.Clamp01(level);
            _stressHeld = true;

            DebugLog($"SetStress({level:F2}) held", _debugColor);
        }


        public void ReleaseStress()
        {
            _stressHeld = false;
            DebugLog("ReleaseStress — stress will decay", _debugColor);
        }


        public void SetStamina(float value)
        {
            Stamina = Mathf.Clamp01(value);
        }


        // =========================================================
        // FMOD
        // =========================================================

        private void CreateBreathInstance()
        {
            if (_breathEvent.IsNull)
            {
                DebugLog("Breath event is not assigned.", new Color(1f, 0.7f, 0.2f));
                return;
            }

            _breathInstance = RuntimeManager.CreateInstance(_breathEvent);
            _breathInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            _breathInstance.start();

            _instanceValid = true;

            DebugLog("Breath FMOD instance created and started.", _debugColor);
        }


        private void PushToFmod(float dt)
        {
            float t = 1f - Mathf.Exp(-_fmodSmoothSpeed * dt);

            _fmodStaminaSmoothed = Mathf.Lerp(_fmodStaminaSmoothed, Stamina, t);
            _fmodStressSmoothed = Mathf.Lerp(_fmodStressSmoothed, Stress, t);

            if (!_instanceValid)
                return;

            float stressOut = _fmodStressSmoothed;

            if (_quantizeStressForFmod)
            {
                if (_fmodStressSmoothed >= 1f)
                    stressOut = 1f;
                else if (_fmodStressSmoothed >= 0.5f)
                    stressOut = 0.5f;
                else
                    stressOut = 0f;
            }

            UpdateFmodPosition();

            _breathInstance.setParameterByName(_fmodPlayerStateParam, GetFmodPlayerState());
            _breathInstance.setParameterByName(_fmodStaminaParam, _fmodStaminaSmoothed);
            _breathInstance.setParameterByName(_fmodStressParam, stressOut);
        }


        private void UpdateFmodPosition()
        {
            Vector3 position = transform.position;

            if (_fmod3DPosInit &&
                (position - _lastFmod3DPos).sqrMagnitude <
                _fmod3DUpdateThreshold * _fmod3DUpdateThreshold)
            {
                return;
            }

            _breathInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

            _lastFmod3DPos = position;
            _fmod3DPosInit = true;
        }


        private float GetFmodPlayerState()
        {
            if (_locomotion == null || !_locomotion.IsMoving)
                return 0f;

            if (_locomotion.IsSprinting)
                return 2f;

            return 1f;
        }


        // =========================================================
        // DEBUG HUD
        // =========================================================

        private void OnGUI()
        {
            if (!_showDebugHUD)
                return;

            CreateGUIStyles();

            float x = _hudPosition.x;
            float y = _hudPosition.y;
            float width = 310f;
            float lineHeight = 18f;
            float height = 11 * lineHeight + 16f;

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.Box(new Rect(x, y, width, height), GUIContent.none, _hudBoxStyle);
            GUI.color = Color.white;

            float currentY = y + 8f;

            DrawHUDLine(x, ref currentY, width, lineHeight,
                "<color=#99CCFF><b>[ PlayerStamina ]</b></color>");

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#DDDDDD>Stamina:</color> {Stamina:F3}" +
                (IsBreathless ? " <color=#FF4444>[BREATHLESS]</color>" : ""));

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#888888>→ FMOD:</color> {_fmodStaminaSmoothed:F3}");

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#DDDDDD>Stress:</color> {Stress:F3}" +
                (_stressHeld ? " <color=#FFAA00>[HELD]</color>" : ""));

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#888888>Stress level:</color> {StressLevel}");

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#888888>→ FMOD:</color> {_fmodStressSmoothed:F3}");

            float actualSpeed = _locomotion != null ? _locomotion.ActualSpeed : 0f;

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#DDDDDD>Actual speed:</color> {actualSpeed:F2} m/s");

            float currentSpeed = _locomotion != null ? _locomotion.CurrentSpeed : 0f;

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#DDDDDD>Target speed:</color> {currentSpeed:F2} m/s");

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#DDDDDD>State:</color> " +
                $"<color=#88DDFF>{GetStateName()}</color> " +
                $"({GetFmodPlayerState():F0})");

            DrawHUDLine(x, ref currentY, width, lineHeight,
                $"<color=#888888>Recovery:</color> " +
                $"{_timeSinceLoad:F2} / {_recoverDelay:F2}s");
        }


        private void DrawHUDLine(float x, ref float y, float width, float height, string text)
        {
            GUI.Label(
                new Rect(x + 10f, y, width - 20f, height),
                text,
                _hudLabelStyle
            );

            y += height;
        }


        private string GetStateName()
        {
            if (_locomotion == null || !_locomotion.IsMoving)
                return "Idle";

            if (_locomotion.IsSprinting)
                return "Sprint";

            if (_locomotion.CurrentSpeed >= _locomotion.RunningSpeed)
                return "Run";

            return "Walk";
        }


        private void CreateGUIStyles()
        {
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
        }


        // =========================================================
        // CLEANUP
        // =========================================================

        private void OnDestroy()
        {
            if (!_instanceValid)
                return;

            _breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _breathInstance.release();

            _instanceValid = false;
        }


        // =========================================================
        // VALIDATION
        // =========================================================

        private void OnValidate()
        {
            _walkDrainPerSec = Mathf.Max(0f, _walkDrainPerSec);
            _runDrainPerSec = Mathf.Max(0f, _runDrainPerSec);
            _sprintDrainPerSec = Mathf.Max(0f, _sprintDrainPerSec);
            _idleRecoverPerSec = Mathf.Max(0f, _idleRecoverPerSec);
            _moveRecoverPerSec = Mathf.Max(0f, _moveRecoverPerSec);
            _recoverDelay = Mathf.Max(0f, _recoverDelay);
            _stressDecayPerSec = Mathf.Max(0f, _stressDecayPerSec);
            _stressRisePerSec = Mathf.Max(0.01f, _stressRisePerSec);
            _fmodSmoothSpeed = Mathf.Max(0.1f, _fmodSmoothSpeed);
            _fmod3DUpdateThreshold = Mathf.Max(0f, _fmod3DUpdateThreshold);
        }


        private void DebugLog(string message, Color color)
        {
            if (!_showDebugLogs)
                return;

            Debug.Log(
                $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[PlayerStamina]</color> {message}"
            );
        }
    }
}