using FMOD_Acoustic_System.Core;
using FMOD_Acoustic_System.Utilities;
using UnityEngine;

namespace FMOD_Acoustic_System.Solvers
{
    public static class DiffractionSolver
    {
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

            // Прямая видимость
            if (!Physics.Raycast(
                start,
                direction,
                distance,
                settings.occlusionLayers,
                QueryTriggerInteraction.Ignore))
            {
                return 0;
            }

            float maxAngle = settings.diffractionAngle;
            int rayCount = settings.diffractionRays;

            for (int i = 1; i <= rayCount; i++)
            {
                float angle =
                    maxAngle * i / rayCount;

                Vector3 left =
                    Quaternion.AngleAxis(
                        -angle,
                        Vector3.up) * direction;

                if (!Physics.Raycast(
                    start,
                    left,
                    settings.diffractionDistance,
                    settings.occlusionLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    UnityEngine.Debug.DrawRay(
                        start,
                        left * settings.diffractionDistance,
                        Color.cyan);

                    return 1f - angle / maxAngle;
                }

                Vector3 right =
                    Quaternion.AngleAxis(
                        angle,
                        Vector3.up) * direction;

                if (!Physics.Raycast(
                    start,
                    right,
                    settings.diffractionDistance,
                    settings.occlusionLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    UnityEngine.Debug.DrawRay(
                        start,
                        right * settings.diffractionDistance,
                        Color.cyan);

                    return 1f - angle / maxAngle;
                }
            }

            return 0;
        }
    }
}