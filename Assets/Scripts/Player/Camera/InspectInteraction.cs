using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InspectInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private InspectManager _inspectManager;

        [Header("Raycast")]
        [SerializeField] private float _interactionDistance = 3f;
        [SerializeField] private LayerMask _interactionLayer;

        private void Update()
        {
            if (_inspectManager.IsInspecting)
                return;

            if (Keyboard.current.eKey.wasPressedThisFrame)
                TryInspect();
        }

        private void TryInspect()
        {
            Ray ray = new Ray(
                _camera.transform.position,
                _camera.transform.forward
            );

            if (!Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    _interactionDistance,
                    _interactionLayer))
                return;

            InspectableItem item = hit.collider.GetComponentInParent<InspectableItem>();

            if (item == null)
                return;

            _inspectManager.StartInspect(item);
        }
    }
}