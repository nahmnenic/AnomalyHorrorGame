using Interact.UI;
using UnityEngine;

namespace Interact
{
    public class PlayerInteractionDoor : MonoBehaviour
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
        
        [SerializeField] private GameObject _gameWindow;
        [HideInInspector] public IInteractable Focused;

        private void Update()
        {
            IInteractable nearest = FindNearestInteractable();
            UpdateFocus(nearest);
        }

        private void UpdateFocus(IInteractable nearest)
        {
            
            if (ReferenceEquals(nearest, Focused)) return;
            Focused?.OnFocusExit();
            Focused = nearest;
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
        
        public void ShowGameWindow()
        {
            _gameWindow.SetActive(true);
        }

        public void Hide()
        {
            Focused.OnFocusExit();
            _promt.Hide();
        }
    }
}
