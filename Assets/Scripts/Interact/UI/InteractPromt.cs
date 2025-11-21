using System;
using TMPro;
using UnityEngine;

namespace Interact.UI
{
    public class InteractPromt : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private string _keyHint = "[E]";
        private Camera _camera;
        private Transform _target;
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private RectTransform _labelRect;

        private void Awake()
        {
            _camera = Camera.main;
            _labelRect = _label.rectTransform;
            _canvas = _label.GetComponentInParent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
            Hide();
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            if (!_label.gameObject.activeSelf)
            {
                _label.gameObject.SetActive(true);
            }
            Vector3 worldPos = _target.position + _worldOffset;
            Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
            Camera uiCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, uiCam,
                    out Vector2 localPoint))
            {
                _labelRect.anchoredPosition = localPoint;
            }
            
        }

        public void Show(IInteractable interactable)
        {
            if (interactable == null)
            {
                Hide();
                return;
            }
            _target = interactable.transform;
            _label.text = $"{_keyHint}{interactable.DisplayName}";
            _label.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _label.gameObject.SetActive(false);
            _target = null;
        }
    }
}
