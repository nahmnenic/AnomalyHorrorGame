using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Windows")]
        [SerializeField] private GameObject _gameWindow;
        [SerializeField] private GameObject _settingWindow;

        [Header("Sound")]
        [SerializeField] private SoundController _escSound;

        [Header("Settings")]
        [SerializeField] private bool _mainMenu;
        
        [Header("EventSystem")]
        [SerializeField] private GameObject _pauseFirstButton;
        [SerializeField] private GameObject _settingsFirstButton;
        private GameObject _lastSelected;

        private GameUI _gameUI;

        public bool UIisOpen { get; private set; }
        public bool BlockMove { get; set; }
        
        public System.Action OnUIOpened;
        public System.Action OnUIClosed;

        private void Awake()
        {
            _gameUI = FindFirstObjectByType<GameUI>();

            if (_gameWindow != null) _gameWindow.SetActive(false);
            if (_settingWindow != null) _settingWindow.SetActive(false);

            SetUIState(false);
        }
        
        private void Update()
        {
            if (EventSystem.current == null) return;

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                _lastSelected = EventSystem.current.currentSelectedGameObject;
            }
        }
        
        private void Start()
        {
            if (InputDeviceManager.Instance != null)
            {
                InputDeviceManager.Instance.OnInputDeviceChanged += DeviceChanged;
            }
        }

        private void OnDestroy()
        {
            if (InputDeviceManager.Instance != null)
            {
                InputDeviceManager.Instance.OnInputDeviceChanged -= DeviceChanged;
            }
        }

        private void DeviceChanged(bool usingGamepad)
        {
            EventSystem.current.sendNavigationEvents = usingGamepad;

            if (usingGamepad)
            {
                SelectButtonForCurrentWindow();
            }
            else
            {
                ClearSelection();
            }
        }
        
        public void SelectButtonForCurrentWindow()
        {
            if (_lastSelected != null && _lastSelected.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelected);
                return;
            }

            if (_settingWindow.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(_settingsFirstButton);
                return;
            }

            if (_gameWindow.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(_pauseFirstButton);
            }
        }
        
        public void ClearSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void SetUIState(bool isOpen)
        {
            UIisOpen = isOpen;
            BlockMove = isOpen;

            if (isOpen)
            {
                _gameUI.UnlockCursor();
                OnUIOpened?.Invoke();
            }
            else
            {
                _gameUI.BlockCursor();
                OnUIClosed?.Invoke();
            }
        }

        private void OpenWindow(GameObject window)
        {
            if (_gameWindow != null) _gameWindow.SetActive(false);
            if (_settingWindow != null) _settingWindow.SetActive(false);

            if (window != null) window.SetActive(true);
            _lastSelected = null;
            SetUIState(true);

            if (InputDeviceManager.Instance.UsingGamepad)
            {
                SelectButtonForCurrentWindow();
            }
        }

        public void CloseAllWindows()
        {
            if (_gameWindow != null) _gameWindow.SetActive(false);
            if (_settingWindow != null) _settingWindow.SetActive(false);

            SetUIState(false);
        }

        public void TogglePauseMenu()
        {
            if (_mainMenu) return;

            if (UIisOpen) CloseAllWindows();
            else OpenWindow(_gameWindow);
        }

        public void OpenSettings()
        {
            OpenWindow(_settingWindow);
        }

        public void CloseSettings()
        {
            OpenWindow(_gameWindow);
        }

        public void HandleEscape()
        {
            PlayEscapeSound();

            if (_settingWindow != null && _settingWindow.activeSelf)
            {
                CloseSettings();
                return;
            }

            TogglePauseMenu();
        }

        public void PlayEscapeSound()
        {
            if (_escSound != null) _escSound.PlaySound();
        }
    }
}