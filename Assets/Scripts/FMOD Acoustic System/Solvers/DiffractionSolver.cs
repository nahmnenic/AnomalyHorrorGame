using UnityEngine;

namespace FMODAcoustics
{
    public class DiffractionSolver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EdgeFinder edgeFinder;

        [SerializeField] private Transform listener;

        [Header("Settings")]
        [SerializeField] private float maxDistance = 30f;

        [SerializeField] private float minVolume = 0.05f;

        [SerializeField] private float maxVolume = 0.6f;
        
        [Header("Geometry")]
        [SerializeField] private LayerMask geometryMask;

        //======================================================

        public void Solve(AcousticSource source, FMODEventController fmod)
        {
            if (listener == null || edgeFinder == null)
                return;

            Vector3 origin = source.Position;
            Vector3 target = listener.position;

            // 1. нет прямого пути
            if (!Physics.Raycast(origin, target - origin, Vector3.Distance(origin, target), geometryMask))
                return;

            // 2. ищем edge
            if (!edgeFinder.TryFindEdge(source, listener, out Vector3 edge))
                return;

            // 3. ВАЖНАЯ ПРОВЕРКА: source → edge
            if (Physics.Raycast(origin, edge - origin, Vector3.Distance(origin, edge), geometryMask))
                return;

            // 4. ВАЖНАЯ ПРОВЕРКА: edge → listener
            if (Physics.Raycast(edge, target - edge, Vector3.Distance(edge, target), geometryMask))
                return;

            // 5. только теперь дифракция валидна
            fmod.SetDiffraction(true, edge);
        }
    }
}