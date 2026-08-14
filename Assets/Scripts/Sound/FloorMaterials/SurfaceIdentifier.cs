using UnityEngine;

namespace Sound.FloorMaterials
{
    public class SurfaceIdentifier :  MonoBehaviour
    {
        [SerializeField] private SurfaceType _surfaceType;
        public SurfaceType SurfaceType => _surfaceType;
    }
}