using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private Animator _animator;
    private InputManager _inputManager;
    private PlayerLocomotion _playerLocomotion;

    public bool isInteracting;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        _inputManager.HandleAllInput();
    }

    private void FixedUpdate()
    {
        _playerLocomotion.HandleAllMovement();
    }
}
