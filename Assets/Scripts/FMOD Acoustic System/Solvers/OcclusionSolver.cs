using UnityEngine;

namespace FMODAcoustics
{
    public class OcclusionSolver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform listener;

        [SerializeField] private LayerMask geometryMask = ~0;

        [Header("Settings")]
        [SerializeField] private float maxDistance = 50f;

        [SerializeField] private int raySteps = 3;

        
        private AcousticSource debugSource;
        //======================================================

        public void Solve(AcousticSource source, FMODEventController fmod)
        {
            if (source == null || fmod == null)
                return;

            debugSource = source; //  ВАЖНО

            if (listener == null)
                return;

            Vector3 origin = source.Position;
            Vector3 target = listener.position;

            Vector3 dir = target - origin;

            if (dir.sqrMagnitude < 0.0001f)
                return;

            float distance = dir.magnitude;
            dir /= distance;

            if (!Physics.Raycast(origin, dir, distance, geometryMask))
            {
                fmod.SetDirectPosition(origin);
                fmod.SetDiffraction(false, Vector3.zero);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (listener == null || debugSource == null)
                return;

            Gizmos.color = Color.yellow;

            Vector3 origin = debugSource.Position;
            Vector3 target = listener.position;

            Vector3 dir = (target - origin).normalized;

            Gizmos.DrawLine(origin, target);

            Vector3 right = Vector3.Cross(dir, Vector3.up);

            for (int i = 0; i < raySteps; i++)
            {
                float angle = (i / (float)(raySteps - 1) - 0.5f) * 0.15f;

                Vector3 spreadDir = (dir + right * angle).normalized;

                Gizmos.DrawRay(origin, spreadDir * 3f);
            }
        }
    }
}