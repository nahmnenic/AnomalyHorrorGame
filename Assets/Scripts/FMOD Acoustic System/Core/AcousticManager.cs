using UnityEngine;

namespace FMODAcoustics
{
    public class AcousticManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OcclusionSolver occlusionSolver;
        [SerializeField] private DiffractionSolver diffractionSolver;

        [Header("Listener")]
        [SerializeField] private Transform listener;

        private AcousticSource[] sources;

        private bool initialized;

        //======================================================

        private void Awake()
        {
            RefreshSources();
        }

        private void OnEnable()
        {
            RefreshSources();
        }

        //======================================================

        public void RefreshSources()
        {
            sources = FindObjectsOfType<AcousticSource>();
            initialized = sources != null;
        }

        //======================================================

        private void Update()
        {
            if (!initialized)
                return;

            if (listener == null)
                return;

            ProcessSources();
        }

        //======================================================

        private void ProcessSources()
        {
            foreach (var source in sources)
            {
                if (source == null)
                    continue;

                var fmod = source.GetComponent<FMODEventController>();

                if (fmod == null)
                    continue; // ВАЖНО: не все объекты звуковые

                occlusionSolver.Solve(source, fmod);
                diffractionSolver.Solve(source, fmod);
            }
        }

        //======================================================

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (sources == null)
                return;

            Gizmos.color = Color.yellow;

            foreach (var s in sources)
            {
                if (s == null) continue;

                Gizmos.DrawWireSphere(s.Position, 0.1f);
            }
        }

#endif
    }
}