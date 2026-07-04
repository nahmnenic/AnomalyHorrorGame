using UnityEngine;

namespace FMODAcoustics
{
    [CreateAssetMenu(
        fileName = "AcousticMaterialDatabase",
        menuName = "FMOD Acoustic System/Material Database")]
    public class AcousticMaterialDatabase : ScriptableObject
    {
        [Header("Concrete")]
        public float concreteVolumeLoss = 1f;
        public float concreteLowPass = 1f;
        public float concreteDiffraction = 0.2f;

        [Header("Glass")]
        public float glassVolumeLoss = 0.3f;
        public float glassLowPass = 0.2f;
        public float glassDiffraction = 0.85f;

        [Header("Wood")]
        public float woodVolumeLoss = 0.5f;
        public float woodLowPass = 0.5f;
        public float woodDiffraction = 0.6f;

        [Header("Fabric")]
        public float fabricVolumeLoss = 0.1f;
        public float fabricLowPass = 0.1f;
        public float fabricDiffraction = 1f;

        public void Get(AcousticSurfaceType type,
            out float volumeLoss,
            out float lowPass,
            out float diffraction)
        {
            switch (type)
            {
                case AcousticSurfaceType.Concrete:
                    volumeLoss = concreteVolumeLoss;
                    lowPass = concreteLowPass;
                    diffraction = concreteDiffraction;
                    break;

                case AcousticSurfaceType.Glass:
                    volumeLoss = glassVolumeLoss;
                    lowPass = glassLowPass;
                    diffraction = glassDiffraction;
                    break;

                case AcousticSurfaceType.Wood:
                    volumeLoss = woodVolumeLoss;
                    lowPass = woodLowPass;
                    diffraction = woodDiffraction;
                    break;

                case AcousticSurfaceType.Fabric:
                    volumeLoss = fabricVolumeLoss;
                    lowPass = fabricLowPass;
                    diffraction = fabricDiffraction;
                    break;

                default:
                    volumeLoss = 1f;
                    lowPass = 1f;
                    diffraction = 0.5f;
                    break;
            }
        }
    }
}