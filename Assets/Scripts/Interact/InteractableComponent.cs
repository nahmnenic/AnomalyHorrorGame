using System;
using System.Collections;
using Components;
using Interact.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class InteractableComponent : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _actionsOn;
        [SerializeField] private UnityEvent _actionsOff;
        [SerializeField] private string _displayNameOn = "E";
        [SerializeField] private string _displayNameOff = "E";
        private string _displayName = "E";
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private float _minTimeToSwitch;
        private float _nextUseTime;
        [SerializeField] private Transform _promptPoint;
        public Transform PromptPoint => _promptPoint;
        [SerializeField] private bool _mainRoom;

        public bool mainRoom
        {
            get => _mainRoom;
            set => _mainRoom = value;
        }

        public event Action DisplayNameChanged;
        
        public bool On;
        public bool AntiSpam;
        public bool BlockSwitch;
        private bool _supBlock = false;
        
        public string DisplayName => _displayName;
        public bool CanInteract() => _isEnabled;

        private void Start()
        {
            SwitchText();
        }

        private void SwitchText()
        {
            if (On) _displayName = _displayNameOn;
            else _displayName = _displayNameOff;
            
            DisplayNameChanged?.Invoke();
        }

        [ContextMenu("Close Door")]
        public void FastCloseDoor()
        {
            if (On)
            {
                GetComponent<EventWithDelayComponent>().StartActions();
            }
            else
            {
                Interact();
                StartCoroutine(CloseDoor());
            }
        }

        private IEnumerator CloseDoor()
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<EventWithDelayComponent>().StartActions();
        }
        
        public virtual void Interact()
        {
            if (AntiSpam)
            {
                if (Time.time < _nextUseTime) return;
                _nextUseTime = Time.time + _minTimeToSwitch;
            }
            
            if (!Enabled()) return;
            if (!On)
            {
                if(_supBlock) return;
                _actionsOn?.Invoke();
                if (BlockSwitch)
                {
                    _supBlock = true;
                    return;
                }
                _displayName = _displayNameOn;
                On = true;
                SwitchText();
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
                _displayName = _displayNameOff;
                On = false;
                SwitchText();
            }
            
        }

        public void SwitchOn()
        {
            if(On) On = false;
            else On = true;

            SwitchText();
        }

        public bool Enabled()
        {
            return gameObject.GetComponent<InteractableComponent>().enabled;
        }
        
    }
}
