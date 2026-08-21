using System;
using UnityEngine;

namespace Player
{
    public class PlayerFOVController : MonoBehaviour
    {
        private Camera _camera;
        private float _defaultFOV;
        
        public bool Zoom;
        public bool Sprinting;
        
        public float _targetFov;
        private float _speedChange;

        private void Awake()
        {
            _camera = Camera.main;
            if (_camera != null) _defaultFOV = _camera.fieldOfView;
            _targetFov = _defaultFOV;
        }

        private void FixedUpdate()
        {
            if (_camera == null) return;
            if (!Zoom && !Sprinting)
            {
                _targetFov = _defaultFOV;
            }
            _camera.fieldOfView = Mathf.MoveTowards(_camera.fieldOfView, _targetFov, _speedChange * Time.deltaTime);
        }

        public void ChangeParametersFov(float targetFOV, float speedChange)
        {
            _targetFov = targetFOV;
            if(Math.Abs(_camera.fieldOfView - _defaultFOV) > 0.03f) return;
            _speedChange = speedChange;
        }
    }
}