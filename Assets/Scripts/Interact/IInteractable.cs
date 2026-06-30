using UnityEngine;

namespace Interact
{
    public interface IInteractable
    {
        Transform transform { get; }
        Transform PromptPoint { get; }
        bool mainRoom { get; set; }
        string DisplayName { get; }
        bool CanInteract();
        void Interact();
        bool Enabled();
    }
}
