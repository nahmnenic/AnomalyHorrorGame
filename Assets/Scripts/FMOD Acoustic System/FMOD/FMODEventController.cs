using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace FMODAcoustics
{
    [RequireComponent(typeof(AcousticSource))]
    public class FMODEventController : MonoBehaviour
    {
        private AcousticSource source;

        private EventInstance directInstance;
        private EventInstance diffractionInstance;

        private bool hasDiffraction;

        private Vector3 diffractionPoint;

        private void Awake()
        {
            source = GetComponent<AcousticSource>();

            directInstance = RuntimeManager.CreateInstance(source.Event);
            diffractionInstance = RuntimeManager.CreateInstance(source.Event);
        }

        private void OnEnable()
        {
            directInstance.start();
        }

        private void OnDisable()
        {
            directInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            diffractionInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            directInstance.release();
            diffractionInstance.release();
        }

        public void SetDirectPosition(Vector3 pos)
        {
            directInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        }

        public void SetDiffraction(bool active, Vector3 pos)
        {
            hasDiffraction = active;
            diffractionPoint = pos;

            if (active)
            {
                diffractionInstance.set3DAttributes(
                    RuntimeUtils.To3DAttributes(pos));

                diffractionInstance.start();
            }
            else
            {
                diffractionInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}