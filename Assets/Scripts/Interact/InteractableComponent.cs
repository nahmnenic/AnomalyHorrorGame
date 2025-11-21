using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class InteractableComponent : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _actions;
        [SerializeField] private string _displayName = "E";
        [SerializeField] private bool _isEnabled = true;

        public string DisplayName => _displayName;
        public bool CanInteract() => _isEnabled;

        private Outline _outline;

        private void Awake()
        {
            _outline = gameObject.AddComponent<Outline>();
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            _outline.OutlineColor = Color.yellow;
            _outline.OutlineWidth = 10f;
            _outline.enabled = false;
        }

        public void Interact()
        {
            _actions?.Invoke();
        }

        public void OnFocusEnter()
        {
            _outline.enabled = true;
        }

        public void OnFocusExit()
        {
            _outline.enabled = false;
        }
    }
}
