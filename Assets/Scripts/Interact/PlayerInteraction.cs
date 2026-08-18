using System;
using Interact.UI;
using RoomMananger;
using UI;
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
        [SerializeField] private Transform _interactionPointParent;
        private Collider[] _interactionResult = new Collider[32];
        private bool _interactionBlocked;

        [Header("Raycast")] 
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _layerMask = ~0;
        
        [HideInInspector] public IInteractable Focused;

        private void Start()
        {
            _promt.Hide();
        }

        private void Update()
        {
            if (_interactionBlocked)
                return;

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
                _promt.Show(Focused);
            }
            else
            {
                _promt.Hide();
            }
        }

        private IInteractable FindNearestInteractable()
        {
            int count = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius, _interactionResult, _interactionLayer);
            IInteractable nearst = null;
            float bestDistSq = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = _interactionResult[i];
                if (col == null) continue;

                IInteractable interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;
                if (!interactable.Enabled()) continue;
                if (!interactable.CanInteract()) continue;

                Vector3 origin = _interactionPointParent.position;
                Vector3 target = col.bounds.center;
                Vector3 dir = target - origin;
                float distance = dir.magnitude;

                // Есть ли стена?
                if (Physics.Raycast(origin, dir.normalized, distance, _layerMask))
                    continue;

                float distSq = dir.sqrMagnitude;
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
        
        public void RefreshFocus()
        {
            UpdateFocus(FindNearestInteractable());
        }
        
        public void Hide()
        {
            _promt.Hide();
        }
        
        public void SetInteractionBlocked(bool blocked)
        {
            _interactionBlocked = blocked;

            if (blocked)
            {
                Focused = null;
                _promt.Hide();
            }
            else
            {
                RefreshFocus();
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
