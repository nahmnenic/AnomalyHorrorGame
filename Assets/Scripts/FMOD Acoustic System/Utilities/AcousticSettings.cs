using UnityEngine;

namespace FMODAcoustics
{
    [CreateAssetMenu(
        fileName = "AcousticSettings",
        menuName = "FMOD Acoustic System/Acoustic Settings")]
    public class AcousticSettings : ScriptableObject
    {
        [Header("Occlusion")]

        [Range(0f, 20f)]
        public float occlusionSmoothSpeed = 8f;

        [Range(0f, 1f)]
        public float maxOcclusion = 1f;

        [Header("Diffraction")]

        [Range(0f, 20f)]
        public float diffractionSmoothSpeed = 6f;

        [Range(0f, 1f)]
        public float minDiffractionVolume = 0.05f;

        [Range(0f, 1f)]
        public float maxDiffractionVolume = 0.6f;

        [Header("Raycasting")]

        public float maxRayDistance = 50f;

        public LayerMask geometryMask = ~0;

        [Header("Performance")]

        [Range(1, 8)]
        public int edgeSearchSteps = 4;

        [Header("Debug")]

        public bool enableDebug = true;

        public bool drawRays = true;
        public bool drawDiffractionPoints = true;
    }
}