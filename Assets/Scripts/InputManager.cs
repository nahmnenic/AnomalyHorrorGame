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

    public Vector2 inspectRotateInput;
    public bool inspectRotateButton;
    public float inspectZoomInput;
    public float inspectZoomInInput;
    public float inspectZoomOutInput;
    
    public bool IsInspecting { get; private set; }

    public void SetInspecting(bool value)
    {
        IsInspecting = value;
    }

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

            _inputSystem.PlayerActions.InspectRotate.performed += i => inspectRotateInput = i.ReadValue<Vector2>();
            _inputSystem.PlayerActions.InspectRotate.canceled += i => inspectRotateInput = Vector2.zero;

            _inputSystem.PlayerActions.InspectRotateButton.performed += i => inspectRotateButton = true;
            _inputSystem.PlayerActions.InspectRotateButton.canceled += i => inspectRotateButton = false;

            _inputSystem.PlayerActions.InspectZoom.performed += i => inspectZoomInput = i.ReadValue<float>();
            _inputSystem.PlayerActions.InspectZoom.canceled += i => inspectZoomInput = 0f;

            _inputSystem.PlayerActions.InspectZoomIn.performed += i => inspectZoomInInput = i.ReadValue<float>();
            _inputSystem.PlayerActions.InspectZoomIn.canceled += i => inspectZoomInInput = 0f;

            _inputSystem.PlayerActions.InspectZoomOut.performed += i => inspectZoomOutInput = i.ReadValue<float>();
            _inputSystem.PlayerActions.InspectZoomOut.canceled += i => inspectZoomOutInput = 0f;

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
        HandleInteractionInput();

        if (_uiManager.BlockMove)
        {
            BlockMoveInput();
            return;
        }

        HandleMovementInput();
        HandleSprintingInput();
        HandleSwitchInput();
    }

    private void BlockMoveInput()
    {
        movementInput = Vector2.zero;
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
        if (IsInspecting)
        {
            e_Input = false;
            return;
        }

        if (!e_Input)
            return;

        e_Input = false;

        if (_playerInteraction.enabled)
            _playerInteraction.Interact();

        if (_doorInteraction.enabled)
            _doorInteraction.Interact();
    }

    private void HandleEscapeInput()
    {
        if (!esc_Input)
            return;

        if (_uiManager.UIisOpen)
        {
            esc_Input = false;
            _uiManager.HandleEscape();
        }
    }

    private void HandleSwitchInput()
    {
        if (!q_Input)
            return;

        q_Input = false;
        _roomController.SwitchRoom();
    }

    public bool ConsumeEscapeInput()
    {
        if (!esc_Input)
            return false;

        esc_Input = false;
        return true;
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