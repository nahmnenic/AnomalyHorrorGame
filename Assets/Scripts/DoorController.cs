using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator _animator;
    public bool IsBlocToClose;
    public bool IsBlocToOpen;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ControlDoor()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !IsBlocToOpen) OpenDoor();
        else if(!IsBlocToClose) CloseDoor();
    }
    
    private void OpenDoor()
    {
        _animator.SetTrigger("Open");
        if (IsBlocToClose) IsBlocToOpen = true;
    }

    public void CloseDoor()
    {
        _animator.SetTrigger("Close");
    }
}
