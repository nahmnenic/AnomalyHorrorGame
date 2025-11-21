using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
        private InputSystem _inputSystem;
        private PlayerLocomotion _playerLococmotion;
        private PlayerInteraction _player;
        private RoomController _roomController;

        public Vector2 movementInput;
        public float moveAmount;
        public float verticalInput;
        public float horizontalalInput;

        public bool shift_Input;
        public bool e_Input;
        public bool q_Input;

        private void Awake()
        {
            _roomController = FindObjectOfType<RoomController>();
            _playerLococmotion = GetComponent<PlayerLocomotion>();
            _player = GetComponent<PlayerInteraction>();
        }

        private void OnEnable()
        {
            if (_inputSystem == null)
            {
                _inputSystem = new InputSystem();
                
                _inputSystem.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

                _inputSystem.PlayerActions.Shift.performed += i => shift_Input = true;
                _inputSystem.PlayerActions.Shift.canceled += i => shift_Input = false;
                _inputSystem.PlayerActions.Interaction.performed += i => e_Input = true;
                _inputSystem.PlayerActions.Switch.performed += i => q_Input = true;
            }
            
            _inputSystem.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
        }

        public void HandleAllInput()
        {
            HandleMovementInput();
            HandleSprintingInput();
            HandleInteractionInput();
            HandleSwitchInput();
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalalInput) + Mathf.Abs(verticalInput));
        }

        private void HandleSprintingInput()
        {
            if (shift_Input && moveAmount > 0.5f && _playerLococmotion.IsGrounded && verticalInput >= 0)
            {
                _playerLococmotion.IsSprinting = true;
            }
            else
            {
                _playerLococmotion.IsSprinting = false;
            }
        }

        private void HandleInteractionInput()
        {
            if (e_Input)
            {
                e_Input = false;
                _player.Interact();
            }
        }
        
        private void HandleSwitchInput()
        {
            if (q_Input)
            {
                q_Input = false;
                _roomController.SwitchRoom();
            }
        }
}
