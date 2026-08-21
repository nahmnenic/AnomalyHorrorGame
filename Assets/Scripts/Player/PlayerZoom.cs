using System;
using UnityEngine;

namespace Player
{
    public class PlayerZoom : MonoBehaviour
    {
        [SerializeField] private float _zoomFov;
        [SerializeField] private float _zoomSpeed;
        [HideInInspector] public bool IsZooming;
        private PlayerFOVController _fovController;

        private void Awake()
        {
            _fovController = GetComponent<PlayerFOVController>();
        }

        private void Update()
        {
            if (!IsZooming)
            {
                _fovController.Zoom = false;
                return;
            }
            _fovController.Zoom = true;
            _fovController.ChangeParametersFov(_zoomFov, _zoomSpeed);
        }
    }
}
