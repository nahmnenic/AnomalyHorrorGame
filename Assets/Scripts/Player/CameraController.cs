using System;
using Interact;
using UI;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public Transform _currentCamFollowTransform;
        public Transform InteractionPointParent;
        
        private InputSystem _inputSystem;
        private UIManager _uiMananger;
        
        public Vector2 rightStickInput;
        
        public float mouseSense;
        public float xAxis, yAxis;
        public float minCam, maxCam;
        private float _progressRotate;
        
        private GameObject _enemyMid;
        

        private void Awake()
        {
            _uiMananger = GetComponent<UIManager>();
        }

        private void Update()
        {
            if(_uiMananger.BlockMove) return;
            xAxis += rightStickInput.x * mouseSense;
            yAxis -= rightStickInput.y * mouseSense;
            yAxis = Mathf.Clamp(yAxis,minCam, maxCam);
        }

        private void LateUpdate()
        {
            _currentCamFollowTransform.localEulerAngles = new Vector3(yAxis,  _currentCamFollowTransform.localEulerAngles.y,
                _currentCamFollowTransform.localEulerAngles.z);
            InteractionPointParent.localEulerAngles = _currentCamFollowTransform.localEulerAngles;
            
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
        }

        private void OnEnable()
        {
            if (_inputSystem == null)
            {
                _inputSystem = new InputSystem();
                _inputSystem.PlayerMovement.Camera.performed += i => rightStickInput = i.ReadValue<Vector2>();
            }
            
            _inputSystem.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
        }
    }
}
