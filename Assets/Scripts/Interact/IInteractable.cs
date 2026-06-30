using UnityEngine;

namespace Interact
{
    public interface IInteractable
    {
        Transform transform { get; }
        
        string DisplayName { get; }
        bool CanInteract();
        void Interact();
        GameObject TextTransform();
        void OnFocusEnter();
        void OnFocusExit();
        bool Enabled();
    }
}
