using UnityEngine;
using FMODUnity;

namespace FMODAcoustics
{
    [DisallowMultipleComponent]
    public class AcousticSource : MonoBehaviour
    {
        [Header("FMOD Event")]
        [SerializeField] private EventReference eventReference;

        public EventReference Event => eventReference;

        public Vector3 Position => transform.position;
    }
}