using System.Collections;
using Interact.UI;
using UnityEngine;
using UI;

namespace Interact
{
    public class InspectManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private Transform _inspectPoint;
        [SerializeField] private InspectUI _inspectUI;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _returnSpeed = 10f;

        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 0.2f;

        [Header("Zoom")]
        [SerializeField] private float _zoomSpeed = 0.1f;
        [SerializeField] private float _gamepadZoomSpeed = 1f;
        [SerializeField] private float _zoomSmoothSpeed = 8f;
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 3f;

        private InspectableItem _currentItem;

        private Transform _originalParent;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private Collider[] _colliders;
        private bool[] _colliderStates;

        private Rigidbody _rigidbody;
        private bool _wasKinematic;
        private bool _useGravity;

        private float _defaultInspectDistance;
        private float _currentDistance;
        private float _targetDistance;

        private float _rotationX;
        private float _rotationY;

        private bool _isInspecting;
        private bool _isMoving;
        private bool _isReturning;

        public bool IsInspecting => _isInspecting;

        private void Awake()
        {
            _defaultInspectDistance = _inspectPoint.localPosition.z;
        }

        private void Update()
        {
            if (!_isInspecting)
                return;

            HandleRotation();
            HandleZoom();
            HandleExit();
        }

        public void StartInspect(InspectableItem item)
        {
            if (_isInspecting || _isMoving || _isReturning)
                return;

            _currentItem = item;

            Transform itemTransform = item.transform;

            _originalParent = itemTransform.parent;
            _originalPosition = itemTransform.position;
            _originalRotation = itemTransform.rotation;

            SetupRigidbody();
            DisableColliders();

            _currentDistance = _defaultInspectDistance;
            _targetDistance = _defaultInspectDistance;

            Vector3 inspectPosition = _inspectPoint.localPosition;
            inspectPosition.z = _currentDistance;
            _inspectPoint.localPosition = inspectPosition;

            _rotationX = 0f;
            _rotationY = 0f;

            _isInspecting = true;
            _isMoving = true;

            _inputManager.SetInspecting(true);

            _uiManager.BlockMove = true;
            _playerInteraction.SetInteractionBlocked(true);
            
            _inspectUI.Show(_currentItem.DisplayName);

            StartCoroutine(MoveToInspectPoint());
        }

        private void SetupRigidbody()
        {
            _rigidbody = _currentItem.GetComponent<Rigidbody>();

            if (_rigidbody == null)
                return;

            _wasKinematic = _rigidbody.isKinematic;
            _useGravity = _rigidbody.useGravity;

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void DisableColliders()
        {
            _colliders = _currentItem.GetComponentsInChildren<Collider>();

            _colliderStates = new bool[_colliders.Length];

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] == null)
                    continue;

                _colliderStates[i] = _colliders[i].enabled;
                _colliders[i].enabled = false;
            }
        }

        private IEnumerator MoveToInspectPoint()
        {
            Transform item = _currentItem.transform;

            item.SetParent(_inspectPoint, true);

            while (_isInspecting && _isMoving)
            {
                item.position = Vector3.Lerp(
                    item.position,
                    _inspectPoint.position,
                    _moveSpeed * Time.deltaTime
                );

                item.rotation = Quaternion.Lerp(
                    item.rotation,
                    _inspectPoint.rotation,
                    _moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(item.position, _inspectPoint.position) < 0.005f)
                    break;

                yield return null;
            }

            if (!_isInspecting || _currentItem == null)
                yield break;

            item.position = _inspectPoint.position;
            item.rotation = _inspectPoint.rotation;

            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;

            _rotationX = 0f;
            _rotationY = 0f;

            _isMoving = false;
        }

        private void HandleRotation()
        {
            if (_isMoving)
                return;

            if (!_inputManager.inspectRotateButton)
                return;

            Vector2 input = _inputManager.inspectRotateInput;

            if (input.sqrMagnitude < 0.0001f)
                return;

            _rotationY += input.x * _rotationSpeed;
            _rotationX -= input.y * _rotationSpeed;

            _rotationX = Mathf.Clamp(_rotationX, -80f, 80f);

            _currentItem.transform.localRotation = Quaternion.Euler(
                _rotationX,
                _rotationY,
                0f
            );
        }

        private void HandleZoom()
        {
            if (_isMoving)
                return;

            float mouseZoom = _inputManager.inspectZoomInput;

            if (Mathf.Abs(mouseZoom) > 0.01f)
            {
                _targetDistance -= mouseZoom * _zoomSpeed;
            }

            float gamepadZoom =
                _inputManager.inspectZoomInInput -
                _inputManager.inspectZoomOutInput;

            if (Mathf.Abs(gamepadZoom) > 0.01f)
            {
                _targetDistance +=
                    gamepadZoom *
                    _gamepadZoomSpeed *
                    Time.deltaTime;
            }

            _targetDistance = Mathf.Clamp(
                _targetDistance,
                _minDistance,
                _maxDistance
            );

            _currentDistance = Mathf.Lerp(
                _currentDistance,
                _targetDistance,
                _zoomSmoothSpeed * Time.deltaTime
            );

            Vector3 position = _inspectPoint.localPosition;
            position.z = _currentDistance;
            _inspectPoint.localPosition = position;
        }

        private void HandleExit()
        {
            if (!_inputManager.ConsumeEscapeInput())
                return;

            ExitInspect();
        }

        private void ExitInspect()
        {
            if (!_isInspecting || _isReturning)
                return;

            _isInspecting = false;
            _isReturning = true;
            _isMoving = false;

            _inspectUI.Hide();
            
            StartCoroutine(ReturnItem());
        }

        private IEnumerator ReturnItem()
        {
            if (_currentItem == null)
            {
                FinishInspect();
                yield break;
            }

            Transform item = _currentItem.transform;

            item.SetParent(_originalParent, true);

            while (_isReturning)
            {
                item.position = Vector3.Lerp(
                    item.position,
                    _originalPosition,
                    _returnSpeed * Time.deltaTime
                );

                item.rotation = Quaternion.Lerp(
                    item.rotation,
                    _originalRotation,
                    _returnSpeed * Time.deltaTime
                );

                if (Vector3.Distance(item.position, _originalPosition) < 0.005f)
                    break;

                yield return null;
            }

            item.position = _originalPosition;
            item.rotation = _originalRotation;

            FinishInspect();
        }

        private void FinishInspect()
        {
            RestoreColliders();
            RestoreRigidbody();

            Vector3 inspectPosition = _inspectPoint.localPosition;
            inspectPosition.z = _defaultInspectDistance;
            _inspectPoint.localPosition = inspectPosition;

            _uiManager.BlockMove = false;
            _playerInteraction.SetInteractionBlocked(false);

            _inputManager.SetInspecting(false);

            _currentItem = null;
            _rigidbody = null;

            _colliders = null;
            _colliderStates = null;

            _isReturning = false;
            _isMoving = false;
        }

        private void RestoreColliders()
        {
            if (_colliders == null || _colliderStates == null)
                return;

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] == null)
                    continue;

                _colliders[i].enabled = _colliderStates[i];
            }
        }

        private void RestoreRigidbody()
        {
            if (_rigidbody == null)
                return;

            _rigidbody.isKinematic = _wasKinematic;
            _rigidbody.useGravity = _useGravity;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (_currentItem != null)
            {
                Transform item = _currentItem.transform;

                item.SetParent(_originalParent, true);
                item.position = _originalPosition;
                item.rotation = _originalRotation;
            }

            RestoreColliders();
            RestoreRigidbody();

            if (_uiManager != null)
                _uiManager.BlockMove = false;

            if (_playerInteraction != null)
                _playerInteraction.SetInteractionBlocked(false);

            if (_inputManager != null)
                _inputManager.SetInspecting(false);

            _currentItem = null;
            _rigidbody = null;

            _colliders = null;
            _colliderStates = null;

            _isInspecting = false;
            _isMoving = false;
            _isReturning = false;
        }
    }
}