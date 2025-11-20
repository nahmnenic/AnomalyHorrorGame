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
        if (gameObject.transform.rotation.y == 0) OpenDoor();
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
