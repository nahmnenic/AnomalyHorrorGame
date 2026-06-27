using System;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance { get; private set; }

    public bool UsingGamepad { get; private set; }

    public event Action<bool> OnInputDeviceChanged;

    private Mouse _mouse;

    private void Awake()
    {
        Instance = this;
        _mouse = Mouse.current;
    }

    private void Update()
    {
        bool gamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;

        bool mouseKeyboard = Keyboard.current.anyKey.wasPressedThisFrame || (_mouse != null && _mouse.delta.ReadValue() != Vector2.zero);

        if (gamepad && !UsingGamepad)
        {
            UsingGamepad = true;
            OnInputDeviceChanged?.Invoke(true);
        }
        else if (mouseKeyboard && UsingGamepad)
        {
            UsingGamepad = false;
            OnInputDeviceChanged?.Invoke(false);
        }
    }
}