using UnityEngine;

namespace FMODAcoustics
{
    [DisallowMultipleComponent]
    public class AcousticMaterial : MonoBehaviour
    {
        [Header("Type")]
        [SerializeField] private AcousticSurfaceType surfaceType;

        [Header("Database")]
        [SerializeField] private AcousticMaterialDatabase database;

        public float VolumeLoss
        {
            get
            {
                if (database == null) return 1f;

                database.Get(surfaceType,
                    out float volume,
                    out _,
                    out _);

                return volume;
            }
        }

        public float LowPassStrength
        {
            get
            {
                if (database == null) return 1f;

                database.Get(surfaceType,
                    out _,
                    out float low,
                    out _);

                return low;
            }
        }

        public float DiffractionFactor
        {
            get
            {
                if (database == null) return 0.5f;

                database.Get(surfaceType,
                    out _,
                    out _,
                    out float diff);

                return diff;
            }
        }
    }
}