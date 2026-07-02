using UnityEngine;

namespace FMOD_Acoustic_System.Materials
{
    public enum AcousticSurfaceType
    {
        Concrete,     
        Brick,
        Wood,
        Glass,
        Metal,
        Drywall,
        Fabric,
        Foliage,
        Custom
    }

    [DisallowMultipleComponent]
    public class AcousticMaterial : MonoBehaviour
    {
        [Header("Surface")]

        [SerializeField]
        private AcousticSurfaceType surfaceType = AcousticSurfaceType.Concrete;

        [Header("Acoustic Properties")]

        [Tooltip("Насколько сильно материал уменьшает громкость.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float volumeLoss = 0.8f;

        [Tooltip("Насколько сильно материал режет высокие частоты.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float lowPassStrength = 0.9f;

        [Tooltip("Насколько легко звук огибает препятствие.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float diffractionFactor = 0.25f;

        [Tooltip("FMOD Material Parameter.")]
        [Range(0f, 32f)]
        [SerializeField]
        private float materialParameter = 0;

        public AcousticSurfaceType SurfaceType => surfaceType;

        public float VolumeLoss => volumeLoss;

        public float LowPassStrength => lowPassStrength;

        public float DiffractionFactor => diffractionFactor;

        public float MaterialParameter => materialParameter;

        private void Reset()
        {
            ApplyPreset(surfaceType);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (surfaceType != AcousticSurfaceType.Custom)
                ApplyPreset(surfaceType);
        }
#endif

        private void ApplyPreset(AcousticSurfaceType type)
        {
            switch (type)
            {
                case AcousticSurfaceType.Concrete:
                    volumeLoss = 0.85f;
                    lowPassStrength = 0.95f;
                    diffractionFactor = 0.20f;
                    materialParameter = 0;
                    break;

                case AcousticSurfaceType.Brick:
                    volumeLoss = 0.75f;
                    lowPassStrength = 0.85f;
                    diffractionFactor = 0.30f;
                    materialParameter = 1;
                    break;

                case AcousticSurfaceType.Wood:
                    volumeLoss = 0.45f;
                    lowPassStrength = 0.45f;
                    diffractionFactor = 0.60f;
                    materialParameter = 2;
                    break;

                case AcousticSurfaceType.Glass:
                    volumeLoss = 0.30f;
                    lowPassStrength = 0.25f;
                    diffractionFactor = 0.80f;
                    materialParameter = 3;
                    break;

                case AcousticSurfaceType.Metal:
                    volumeLoss = 0.70f;
                    lowPassStrength = 0.70f;
                    diffractionFactor = 0.35f;
                    materialParameter = 4;
                    break;

                case AcousticSurfaceType.Drywall:
                    volumeLoss = 0.35f;
                    lowPassStrength = 0.40f;
                    diffractionFactor = 0.75f;
                    materialParameter = 5;
                    break;

                case AcousticSurfaceType.Fabric:
                    volumeLoss = 0.15f;
                    lowPassStrength = 0.15f;
                    diffractionFactor = 1.00f;
                    materialParameter = 6;
                    break;

                case AcousticSurfaceType.Foliage:
                    volumeLoss = 0.25f;
                    lowPassStrength = 0.30f;
                    diffractionFactor = 0.90f;
                    materialParameter = 7;
                    break;
            }
        }
    }
}