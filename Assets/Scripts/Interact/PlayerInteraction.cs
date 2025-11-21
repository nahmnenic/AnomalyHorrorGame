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
        private Collider[] _interactionResult = new Collider[32];

        private IInteractable _focused;

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
                transform.position,
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
                if (!interactable.CanInteract()) continue;
                float distSq = (col.transform.position - transform.position).sqrMagnitude;
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
    }
}
