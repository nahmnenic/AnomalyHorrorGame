using FMOD_Acoustic_System.Core;
using FMOD_Acoustic_System.Materials;
using FMOD_Acoustic_System.Utilities;
using UnityEngine;

namespace FMOD_Acoustic_System.Solvers
{
    public static class OcclusionSolver
    {
        public static float LastMaterialParameter { get; private set; }

        private static readonly Vector3[] RayOffsets =
        {
            Vector3.zero,

            Vector3.up,
            Vector3.down,

            Vector3.left,
            Vector3.right,

            new Vector3(1,1,0).normalized,
            new Vector3(-1,1,0).normalized,

            new Vector3(1,-1,0).normalized,
            new Vector3(-1,-1,0).normalized
        };

        public static float Calculate(AcousticSource source)
        {
            Transform listener = AcousticManager.Listener;

            if (listener == null)
                return 0;

            AcousticSettings settings = AcousticManager.Settings;

            Vector3 start = source.Position;
            Vector3 end = listener.position;

            Vector3 direction = (end - start).normalized;

            float distance = Vector3.Distance(start, end);

            LastMaterialParameter = 0;

            int blocked = 0;

            Vector3 right = Vector3.Cross(direction, Vector3.up);

            if (right.sqrMagnitude < 0.001f)
                right = Vector3.right;

            right.Normalize();

            Vector3 up = Vector3.Cross(right, direction);

            float radius = settings.rayRadius;

            for (int i = 0; i < RayOffsets.Length; i++)
            {
                Vector3 offset =
                    right * RayOffsets[i].x * radius +
                    up * RayOffsets[i].y * radius;

                Vector3 origin = start + offset;

                bool hitSomething =
                    Physics.Raycast(
                        origin,
                        direction,
                        out RaycastHit hit,
                        distance,
                        settings.occlusionLayers,
                        QueryTriggerInteraction.Ignore);

                if (hitSomething)
                {
                    blocked++;

                    AcousticMaterial material =
                        hit.collider.GetComponent<AcousticMaterial>();

                    if (material != null)
                    {
                        LastMaterialParameter =
                            material.MaterialParameter;
                    }

                    UnityEngine.Debug.DrawLine(
                        origin,
                        hit.point,
                        Color.red);
                }
                else
                {
                    UnityEngine.Debug.DrawLine(
                        origin,
                        origin + direction * distance,
                        Color.green);
                }
            }

            return blocked / (float)RayOffsets.Length;
        }
    }
}