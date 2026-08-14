using Sound.FloorMaterials;
using UnityEngine;

namespace Player
{
    public class SurfaceDetector : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private Transform _rayOrigin;
        [SerializeField] private float _rayDistance = 1.5f;
        [SerializeField] private LayerMask _surfaceMask;

        public SurfaceType CurrentSurface { get; private set; }

        private void Update()
        {
            DetectSurface();
        }

        private void DetectSurface()
        {
            if (Physics.Raycast(_rayOrigin.position, Vector3.down, out RaycastHit hit, _rayDistance, _surfaceMask))
            {
                SurfaceIdentifier surface = hit.collider.GetComponentInParent<SurfaceIdentifier>();

                if (surface != null)
                {
                    CurrentSurface = surface.SurfaceType;
                    return;
                }
            }
            
            CurrentSurface = SurfaceType.concrete;
        }

        private void OnDrawGizmosSelected()
        {
            if (_rayOrigin == null)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_rayOrigin.position, Vector3.down * _rayDistance);
        }
    }
}