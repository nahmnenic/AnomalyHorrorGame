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
        
        [SerializeField] private SoundController _escSound;
        public GameObject _gameWindow;
        public GameObject _settingWindow;
        public bool MainMenu;
        [HideInInspector] public IInteractable Focused;
        
        public bool BlockMove = false;

        private void Start()
        {
            _promt.Hide();
        }

        private void Update()
        {
            IInteractable nearest = FindNearestInteractable();
            UpdateFocus(nearest);
        }
        
        private void UpdateFocus(IInteractable nearest)
        {
            if (ReferenceEquals(nearest, Focused)) return;
            
            if (Focused is InteractableComponent oldInteractable) oldInteractable.DisplayNameChanged -= OnDisplayNameChanged;
            Focused = nearest;
            if (Focused is InteractableComponent interactable) interactable.DisplayNameChanged += OnDisplayNameChanged;
            
            if (Focused != null)
            {
                Focused.OnFocusEnter();
                _promt.Show(Focused);
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
            if (Focused != null)
            {
                if (Focused.CanInteract()) Focused.Interact();
            }
        }
        
        public void Hide()
        {
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
                if(!MainMenu) _gameWindow.SetActive(true);
            }
        }

        public void EscSound()
        {
            _escSound.PlaySound();
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
        
        private void OnDisplayNameChanged()
        {
            if (Focused != null) _promt.Show(Focused);
        }

        private void OnDisable()
        {
            Hide();
        }
    }
}
