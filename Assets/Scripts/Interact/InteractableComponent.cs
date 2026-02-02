using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class InteractableComponent : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _actionsOn;
        [SerializeField] private UnityEvent _actionsOff;
        [SerializeField] private string _displayName = "E";
        [SerializeField] private bool _isEnabled = true;
        public bool On;
        public bool BlockSwitch;
        private bool _supBlock = false;

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
            if (!On)
            {
                if(_supBlock) return;
                _actionsOn?.Invoke();
                if (BlockSwitch)
                {
                    _supBlock = true;
                    return;
                }
                On = true;
            }
            else
            {
                if(_supBlock) return;
                _actionsOff?.Invoke();
                if (BlockSwitch)
                {
                    _supBlock = true;
                    return;
                }
                On = false;
            }
        }

        public void OnFocusEnter()
        {
            _outline.enabled = true;
        }

        public void OnFocusExit()
        {
            _outline.enabled = false;
        }

        public void SwitchOn()
        {
            if(On) On = false;
            else On = true;
        }
    }
}
