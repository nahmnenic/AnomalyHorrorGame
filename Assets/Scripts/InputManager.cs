using System;
using FMOD;
using Interact;
using Player;
using RoomMananger;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class InputManager : MonoBehaviour
{
        private InputSystem _inputSystem;
        private PlayerLocomotion _playerLococmotion;
        private SoundController _soundController;
        private PlayerLighter _playerLighter;
        private PlayerInteraction _playerInteraction;
        private PlayerInteractionDoor _doorInteraction;
        private RoomController _roomController;

        public Vector2 movementInput;
        public float moveAmount;
        public float verticalInput;
        public float horizontalalInput;

        public bool shift_Input;
        public bool e_Input;
        public bool esc_Input;
        public bool f_Input;
        public bool q_Input;
        
        private void Awake()
        {
            _roomController = FindObjectOfType<RoomController>();
            _soundController = GetComponentInChildren<SoundController>();
            _playerLighter = GetComponent<PlayerLighter>();
            _playerLococmotion = GetComponent<PlayerLocomotion>();
            _playerInteraction = GetComponent<PlayerInteraction>();
            _doorInteraction = GetComponent<PlayerInteractionDoor>();
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
                _inputSystem.PlayerActions.Escape.performed += i => esc_Input = true;
                _inputSystem.PlayerActions.Flash.performed += i => f_Input = true;
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
            HandleEscapeInput();
            if(_playerInteraction.BlockMove) return;
            HandleMovementInput();
            HandleSprintingInput();
            HandleInteractionInput();
            HandleSwitchInput();
            HandleFlashLightInput();
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalalInput) + Mathf.Abs(verticalInput));
            if(moveAmount!=0 && !_soundController.IsPlaying()) _soundController.PlaySound();
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
                if(_playerInteraction.enabled)_playerInteraction.Interact();
                if(_doorInteraction.enabled)_doorInteraction.Interact();
            }
        }
        
        private void HandleEscapeInput()
        {
            if (esc_Input)
            {
                esc_Input = false;
                _playerInteraction.EscSound();
                if(_playerInteraction._gameWindow.activeSelf) _playerInteraction.ShowGameWindow();
                else if (_playerInteraction._settingWindow.activeSelf)
                {
                    _playerInteraction.ShowSettingWindow();
                    _playerInteraction.ShowGameWindow();
                }
                else
                {
                    _playerInteraction.ShowGameWindow();
                }
            }
        }
        
        private void HandleFlashLightInput()
        {
            if (f_Input)
            {
                f_Input = false;
                _playerLighter.Flash();
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
