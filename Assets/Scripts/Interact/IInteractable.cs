using UnityEngine;

namespace Interact
{
    public interface IInteractable
    {
        Transform transform { get; }
        string DisplayName { get; }
        bool CanInteract();
        void Interact();
        void OnFocusEnter();
        void OnFocusExit();
    }
}
