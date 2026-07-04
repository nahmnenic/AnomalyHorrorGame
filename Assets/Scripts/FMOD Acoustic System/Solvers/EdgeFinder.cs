using UnityEngine;

namespace FMODAcoustics
{
    public class EdgeFinder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask geometryMask = ~0;

        [SerializeField] private float stepSize = 0.25f;

        [SerializeField] private int maxSteps = 8;

        [SerializeField] private float heightOffset = 1.2f;

        //======================================================

        public bool TryFindEdge(
            AcousticSource source,
            Transform listener,
            out Vector3 edgePoint)
        {
            edgePoint = Vector3.zero;

            Vector3 origin = source.Position + Vector3.up * heightOffset;
            Vector3 target = listener.position + Vector3.up * heightOffset;

            Vector3 dir = (target - origin).normalized;

            // 1. проверяем есть ли вообще препятствие
            if (!Physics.Raycast(origin, dir, out RaycastHit hit, 100f, geometryMask))
                return false;

            // 2. берем нормаль стены
            Vector3 wallNormal = hit.normal;

            // 3. строим направление вдоль стены
            Vector3 tangent = Vector3.Cross(wallNormal, Vector3.up).normalized;

            Vector3 bestPoint = Vector3.zero;
            float bestScore = float.MinValue;

            // 4. сканируем вдоль края
            for (int i = -maxSteps; i <= maxSteps; i++)
            {
                Vector3 candidate =
                    hit.point +
                    tangent * (i * stepSize);

                candidate += Vector3.up * 0.1f;

                // проверяем есть ли видимость до игрока
                if (!Physics.Raycast(candidate, target - candidate, Vector3.Distance(candidate, target), geometryMask))
                {
                    float score = Vector3.Distance(candidate, target);

                    // ближе к игроку = лучше (обычно звучит естественнее)
                    if (score < bestScore || bestScore == float.MinValue)
                    {
                        bestScore = score;
                        bestPoint = candidate;
                    }
                }
            }

            if (bestScore == float.MinValue)
                return false;

            edgePoint = bestPoint;
            return true;
        }
    }
}