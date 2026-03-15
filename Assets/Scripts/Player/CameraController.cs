using System;
using Interact;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public Transform _currentCamFollowTransform;
        
        private InputSystem _inputSystem;
        private PlayerInteraction _playerInteraction;
        
        public Vector2 rightStickInput;
        
        public float mouseSense;
        public float xAxis, yAxis;
        public float minCam, maxCam;
        private float _progressRotate;
        
        private GameObject _enemyMid;
        

        private void Awake()
        {
            _playerInteraction =  GetComponent<PlayerInteraction>();
        }

        private void Update()
        {
            if(_playerInteraction.BlockMove) return;
            xAxis += rightStickInput.x * mouseSense;
            yAxis -= rightStickInput.y * mouseSense;
            yAxis = Mathf.Clamp(yAxis,minCam, maxCam);
        }

        private void LateUpdate()
        {
            _currentCamFollowTransform.localEulerAngles = new Vector3(yAxis,  _currentCamFollowTransform.localEulerAngles.y,
                _currentCamFollowTransform.localEulerAngles.z);
            
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
