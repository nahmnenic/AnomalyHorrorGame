using System;
using Interact.UI;
using UnityEngine;

namespace Interact
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction")] 
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private InteractPromt _promt;
        [SerializeField] private Transform _interactionPoint;
        private Collider[] _interactionResult = new Collider[32];

        [Header("Raycast")] 
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _layerMask = ~0;
        
        public GameObject _gameWindow;
        public GameObject _settingWindow;
        private IInteractable _focused;
        
        public bool BlockMove = false;

        private void Update()
        {
            IInteractable nearest = FindNearestInteractable();
            UpdateFocus(nearest);
        }

        private void UpdateFocus(IInteractable nearest)
        {
            if (ReferenceEquals(nearest, _focused)) return;
            _focused?.OnFocusExit();
            _focused = nearest;
            
            if (_focused != null)
            {
                _focused.OnFocusEnter();
                _promt.Show(_focused);
            }
            else
            {
                _promt.Hide();
            }
        }

        private IInteractable FindNearestInteractable()
        {
            int count = Physics.OverlapSphereNonAlloc(
                _interactionPoint.position,
                _interactionRadius,
                _interactionResult,
                _interactionLayer);
            IInteractable nearst = null;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col =  _interactionResult[i];
                if (col == null) continue;
                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;
                if(!interactable.Enabled()) continue;
                if (!interactable.CanInteract()) continue;
                float distSq = (col.transform.position - _interactionPoint.position).sqrMagnitude;
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    nearst = interactable;
                }
            }
            return nearst;
        }

        public void Interact()
        {
            if (_focused != null)
            {
                if (_focused.CanInteract()) _focused.Interact();
            }
        }
        
        public void Hide()
        {
            _focused.OnFocusExit();
            _promt.Hide();
        }
        
        public void ShowGameWindow()
        {
            if(_gameWindow == null) return;
            if (_gameWindow.activeSelf)
            {
                BlockMove = false;
                _gameWindow.SetActive(false);
            }
            else
            {
                BlockMove = true;
                _gameWindow.SetActive(true);
            }
        }
        
        public void ShowSettingWindow()
        {
            if(_settingWindow == null) return;
            if (_settingWindow.activeSelf)
            {
                BlockMove = false;
                _settingWindow.SetActive(false);
            }
            else
            {
                _settingWindow.SetActive(true);
                BlockMove = true;
            }
        }
    }
}
