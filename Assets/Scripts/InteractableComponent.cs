using UnityEngine;
using UnityEngine.Events;

public class InteractableComponent : MonoBehaviour
{
    [SerializeField] private UnityEvent _actions;

    public void Interact()
    {
        _actions?.Invoke();
    }
}
