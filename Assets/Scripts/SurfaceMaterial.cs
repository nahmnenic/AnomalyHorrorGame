using UnityEngine;

namespace Material
{
    public class SurfaceMaterial : MonoBehaviour
    {
        [SerializeField] private SurfaceType _surfaceType = SurfaceType.Default;

        public SurfaceType SurfaceType => _surfaceType;
    }
}