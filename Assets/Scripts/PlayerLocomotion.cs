using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
        private PlayerManager _palyerManager;
        private InputManager inputManager;
        private Vector3 _moveDirection;
        private Transform _cameraObj;
        private Rigidbody _rb;
        private bool _allowDoubleJump;

        [Header("Movement Flags")]
        public bool IsSprinting;
        public bool IsGrounded;
        public bool IsJumping;
        public bool IsCrouching;

        [Header("Movement Speed")]
        public float walkingSpeed;
        public float runningSpeed;
        public float sprintingSpeed;


        private void Awake()
        {
            inputManager = GetComponent<InputManager>();
            _rb = GetComponent<Rigidbody>();
            _palyerManager = GetComponent<PlayerManager>();
            _cameraObj = Camera.main.transform;
        }

        public void HandleAllMovement()
        {
            if (_palyerManager.isInteracting)
                return;
            HandleMovement();
        }
        
        private void HandleMovement()
        {
            if (IsJumping)
                return;
            
            _moveDirection = _cameraObj.forward * inputManager.verticalInput;
            _moveDirection += _cameraObj.right * inputManager.horizontalalInput;
            _moveDirection.Normalize();
            _moveDirection.y = 0;
            if (IsSprinting && IsGrounded && !IsCrouching)
            {
                _moveDirection *= sprintingSpeed;
            }
            else
            {
                if (inputManager.moveAmount >= 0.5f && !IsCrouching)
                {
                    _moveDirection *= runningSpeed;
                }
                else if (IsCrouching)
                {
                    _moveDirection *= walkingSpeed;
                }
                else
                {
                    _moveDirection *= walkingSpeed;
                }
            }

            Vector3 movementVelocity = _moveDirection;
            _rb.velocity = movementVelocity;
        }
}
