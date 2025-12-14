using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ControlDoor()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) OpenDoor();
        else CloseDoor();
    }
    
    private void OpenDoor()
    {
        _animator.SetTrigger("Open");
    }

    private void CloseDoor()
    {
        _animator.SetTrigger("Close");
    }
}
