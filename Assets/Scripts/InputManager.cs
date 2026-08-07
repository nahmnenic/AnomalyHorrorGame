using Interact; 
using Player;
using RoomMananger;
using UI;
using UnityEngine;

public class InputManager : MonoBehaviour
{
        private InputSystem _inputSystem;
        private PlayerLocomotion _playerLococmotion;
        private SoundController _soundController;
        private PlayerInteraction _playerInteraction;
        private PlayerInteractionDoor _doorInteraction;
        private RoomController _roomController;
        private UIManager _uiManager;

        public Vector2 movementInput;
        public float moveAmount;
        public float verticalInput;
        public float horizontalalInput;

        public bool shift_Input;
        public bool e_Input;
        public bool esc_Input;
        public bool q_Input;
        
        private void Awake()
        {
            _roomController = FindObjectOfType<RoomController>();
            _soundController = GetComponentInChildren<SoundController>();
            _playerLococmotion = GetComponent<PlayerLocomotion>();
            _playerInteraction = GetComponent<PlayerInteraction>();
            _doorInteraction = GetComponent<PlayerInteractionDoor>();
            _uiManager = GetComponent<UIManager>();
        }
        
        private void Start()
        {
            _uiManager.OnUIOpened += EnableUI;
            _uiManager.OnUIClosed += EnableGameplay;
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
                
                _inputSystem.Global.Escape.performed += i => esc_Input = true;
            }
            
            _inputSystem.Global.Enable();
            _inputSystem.PlayerMovement.Enable();
            _inputSystem.PlayerActions.Enable();
            _inputSystem.UI.Disable();
        }

        private void OnDisable()
        {
            _inputSystem.Disable();
            
            _uiManager.OnUIOpened -= EnableUI;
            _uiManager.OnUIClosed -= EnableGameplay;
        }

        public void HandleAllInput()
        {
            HandleEscapeInput();
            if (_uiManager.BlockMove)
            {
                BlockMoveInput();
                return;
            }
            HandleMovementInput();
            HandleSprintingInput();
            HandleInteractionInput();
            HandleSwitchInput();
        }

        private void BlockMoveInput()
        {
            movementInput = new Vector2(0, 0);
            moveAmount = 0;
            verticalInput = 0;
            horizontalalInput = 0;
        }
        
        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalalInput) + Mathf.Abs(verticalInput));
        }

        private void HandleSprintingInput()
        {
            if (shift_Input && moveAmount > 0.5f && verticalInput >= 0)
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
            if (!esc_Input) return;
            esc_Input = false;

            _uiManager.HandleEscape();
        }
        
        private void HandleSwitchInput()
        {
            if (q_Input)
            {
                q_Input = false;
                _roomController.SwitchRoom();
            }
        }
        
        public void EnableGameplay()
        {
            _inputSystem.PlayerMovement.Enable();
            _inputSystem.PlayerActions.Enable();
            _inputSystem.UI.Disable();
        }

        public void EnableUI()
        {
            _inputSystem.PlayerMovement.Disable();
            _inputSystem.PlayerActions.Disable();
            _inputSystem.UI.Enable();
        }
}
