using UnityEngine;

namespace FMOD_Acoustic_System.Debug
{
    public static class AcousticDebug
    {
        public static void DrawRay(
            Vector3 start,
            Vector3 end,
            bool blocked)
        {
#if UNITY_EDITOR

            UnityEngine.Debug.DrawLine(
                start,
                end,
                blocked ? Color.red : Color.green);

#endif
        }

        public static void DrawSphere(
            Vector3 position,
            float radius,
            Color color)
        {
#if UNITY_EDITOR

            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, radius);

#endif
        }

        public static void DrawPoint(
            Vector3 position,
            Color color)
        {
#if UNITY_EDITOR

            UnityEngine.Debug.DrawRay(
                position,
                Vector3.up * 0.2f,
                color);

#endif
        }
    }
}