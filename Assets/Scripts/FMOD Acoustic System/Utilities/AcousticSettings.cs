using UnityEngine;

namespace FMOD_Acoustic_System.Utilities
{
    [CreateAssetMenu(
        fileName = "Acoustic Settings",
        menuName = "FMOD Acoustics/Acoustic Settings")]
    public class AcousticSettings : ScriptableObject
    {
        [Header("Occlusion")]

        [Tooltip("Количество лучей для проверки окклюзии.")]
        [Range(1, 32)]
        public int rayCount = 9;

        [Tooltip("Радиус вокруг источника, в котором будут выпускаться дополнительные лучи.")]
        [Range(0f, 2f)]
        public float rayRadius = 0.35f;

        [Tooltip("Максимальная дистанция проверки.")]
        public float maxDistance = 60f;

        [Tooltip("Какие слои считаются препятствиями.")]
        public LayerMask occlusionLayers;

        [Header("Diffraction")]

        [Tooltip("Максимальный угол поиска края стены.")]
        [Range(5f, 90f)]
        public float diffractionAngle = 35f;

        [Tooltip("Количество лучей поиска края.")]
        [Range(5, 40)]
        public int diffractionRays = 15;

        [Tooltip("Максимальное расстояние поиска края.")]
        public float diffractionDistance = 4f;

        [Header("Update")]

        [Tooltip("Сколько источников обновлять за кадр.")]
        [Range(1, 100)]
        public int sourcesPerFrame = 20;

        [Tooltip("Скорость сглаживания параметров.")]
        [Range(1f, 30f)]
        public float interpolationSpeed = 8f;

        [Header("Performance")]

        [Tooltip("Не обновлять очень дальние источники.")]
        public float cullingDistance = 100f;

        [Tooltip("Минимальное смещение объекта перед пересчетом.")]
        public float movementThreshold = 0.05f;

        [Header("Debug")]

        public bool drawRays = true;

        public bool drawDiffraction = true;

        public bool drawLabels = false;
    }
}