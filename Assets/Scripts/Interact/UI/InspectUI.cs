using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.UI
{
    public class InspectUI : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _root;

        [Header("Item Name")]
        [SerializeField] private TMP_Text _itemName;

        [Header("Rotate")]
        [SerializeField] private Image _rotateImage;
        [SerializeField] private TMP_Text _rotateText;

        [Header("Zoom")]
        [SerializeField] private Image _zoomImage;
        [SerializeField] private TMP_Text _zoomText;

        [Header("Exit")]
        [SerializeField] private Image _exitImage;
        [SerializeField] private TMP_Text _exitText;

        [Header("Keyboard / Mouse")]
        [SerializeField] private Sprite _mouseLeftSprite;
        [SerializeField] private Sprite _mouseWheelSprite;
        [SerializeField] private Sprite _escapeSprite;

        [Header("Gamepad")]
        [SerializeField] private Sprite _gamepadRotateSprite;
        [SerializeField] private Sprite _gamepadZoomSprite;
        [SerializeField] private Sprite _gamepadExitSprite;

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            if (InputDeviceManager.Instance == null)
                return;

            InputDeviceManager.Instance.OnInputDeviceChanged += SwitchDevice;

            SwitchDevice(InputDeviceManager.Instance.UsingGamepad);
        }

        private void OnDisable()
        {
            if (InputDeviceManager.Instance == null)
                return;

            InputDeviceManager.Instance.OnInputDeviceChanged -= SwitchDevice;
        }

        public void Show(string itemName)
        {
            _root.SetActive(true);

            if (_itemName != null)
                _itemName.text = itemName;
        }

        public void Hide()
        {
            if (_root != null)
                _root.SetActive(false);
        }

        public void SwitchDevice(bool usingGamepad)
        {
            if (usingGamepad)
            {
                if (_rotateImage != null)
                    _rotateImage.sprite = _gamepadRotateSprite;

                if (_zoomImage != null)
                    _zoomImage.sprite = _gamepadZoomSprite;

                if (_exitImage != null)
                    _exitImage.sprite = _gamepadExitSprite;

                if (_rotateText != null)
                    _rotateText.text = "Вращать";

                if (_zoomText != null)
                    _zoomText.text = "Приблизить";

                if (_exitText != null)
                    _exitText.text = "Назад";
            }
            else
            {
                if (_rotateImage != null)
                    _rotateImage.sprite = _mouseLeftSprite;

                if (_zoomImage != null)
                    _zoomImage.sprite = _mouseWheelSprite;

                if (_exitImage != null)
                    _exitImage.sprite = _escapeSprite;

                if (_rotateText != null)
                    _rotateText.text = "Вращать";

                if (_zoomText != null)
                    _zoomText.text = "Приблизить";

                if (_exitText != null)
                    _exitText.text = "Выйти";
            }
        }
    }
}