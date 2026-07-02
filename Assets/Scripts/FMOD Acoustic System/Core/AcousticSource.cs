using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FMOD_Acoustic_System.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StudioEventEmitter))]
    public class AcousticSource : MonoBehaviour
    {
        [Header("FMOD")]

        [SerializeField]
        private StudioEventEmitter emitter;

        [Header("Debug")]

        [SerializeField]
        private bool showDebug;

        #region Runtime Values

        public float Occlusion { get; private set; }

        public float Diffraction { get; private set; }

        public float Distance { get; private set; }

        public float Material { get; private set; }

        #endregion

        #region Targets

        private float targetOcclusion;
        private float targetDiffraction;
        private float targetDistance;
        private float targetMaterial;

        #endregion

        #region FMOD

        private EventInstance eventInstance;

        private bool initialized;

        private PARAMETER_ID occlusionID;
        private PARAMETER_ID diffractionID;
        private PARAMETER_ID distanceID;
        private PARAMETER_ID materialID;

        #endregion

        public Vector3 Position => transform.position;

        //---------------------------------------------------------------------

        private void Awake()
        {
            if (emitter == null)
                emitter = GetComponent<StudioEventEmitter>();
        }

        //---------------------------------------------------------------------

        private void OnEnable()
        {
            AcousticManager.Register(this);
        }

        //---------------------------------------------------------------------

        private void OnDisable()
        {
            AcousticManager.Unregister(this);
        }

        //---------------------------------------------------------------------

        public void SetAcousticValues(
            float occlusion,
            float diffraction,
            float distance,
            float material)
        {
            targetOcclusion = Mathf.Clamp01(occlusion);
            targetDiffraction = Mathf.Clamp01(diffraction);
            targetDistance = distance;
            targetMaterial = material;
        }

        //---------------------------------------------------------------------

        public void ApplyParameters(float interpolationSpeed)
        {
            Initialize();

            if (!initialized)
                return;

            Occlusion = Mathf.Lerp(Occlusion, targetOcclusion, Time.deltaTime * interpolationSpeed);
            Diffraction = Mathf.Lerp(Diffraction, targetDiffraction, Time.deltaTime * interpolationSpeed);
            Distance = Mathf.Lerp(Distance, targetDistance, Time.deltaTime * interpolationSpeed);
            Material = Mathf.Lerp(Material, targetMaterial, Time.deltaTime * interpolationSpeed);

            eventInstance.setParameterByID(occlusionID, Occlusion);
            eventInstance.setParameterByID(diffractionID, Diffraction);
            eventInstance.setParameterByID(distanceID, Distance);
            eventInstance.setParameterByID(materialID, Material);

            if (showDebug)
            {
                UnityEngine.Debug.Log(
                    $"[{name}] Occ:{Occlusion:F2} Dif:{Diffraction:F2} Dist:{Distance:F1}");
            }
        }

        //---------------------------------------------------------------------

        private void Initialize()
        {
            if (initialized)
                return;

            eventInstance = emitter.EventInstance;

            if (!eventInstance.isValid())
                return;

            eventInstance.getDescription(out EventDescription desc);

            CacheParameter(desc, "Occlusion", out occlusionID);
            CacheParameter(desc, "Diffraction", out diffractionID);
            CacheParameter(desc, "Distance", out distanceID);
            CacheParameter(desc, "Material", out materialID);

            initialized = true;
        }

        //---------------------------------------------------------------------

        private void CacheParameter(
            EventDescription description,
            string parameterName,
            out PARAMETER_ID id)
        {
            id = default;

            if (description.getParameterDescriptionByName(
                    parameterName,
                    out PARAMETER_DESCRIPTION parameter)
                == FMOD.RESULT.OK)
            {
                id = parameter.id;
            }
        }
    }
}