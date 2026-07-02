using System.Collections.Generic;
using FMOD_Acoustic_System.Solvers;
using FMOD_Acoustic_System.Utilities;
using UnityEngine;

namespace FMOD_Acoustic_System.Core
{
    /// <summary>
    /// Главный менеджер акустической системы.
    /// Обновляет источники звука, рассчитывает окклюзию и дифракцию.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class AcousticManager : MonoBehaviour
    {
        #region Singleton

        public static AcousticManager Instance { get; private set; }

        #endregion

        #region Inspector

        [Header("References")]
        [SerializeField] private AcousticSettings settings;
        [SerializeField] private Transform listener;

        #endregion

        #region Runtime

        private static readonly List<AcousticSource> Sources = new();

        private int currentSourceIndex;

        #endregion

        #region Properties

        public static AcousticSettings Settings => Instance.settings;

        public static Transform Listener => Instance.listener;

        #endregion

        //==============================================================

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (listener == null)
            {
                AudioListener audioListener =
                    FindFirstObjectByType<AudioListener>();

                if (audioListener != null)
                    listener = audioListener.transform;
            }

            if (settings == null)
            {
                UnityEngine.Debug.LogError(
                    "[FMOD Acoustics] AcousticSettings не назначен!",
                    this);
            }
        }

        //==============================================================

        private void Update()
        {
            if (listener == null)
                return;

            if (settings == null)
                return;

            if (Sources.Count == 0)
                return;

            int updateCount =
                Mathf.Min(settings.sourcesPerFrame, Sources.Count);

            for (int i = 0; i < updateCount; i++)
            {
                if (currentSourceIndex >= Sources.Count)
                    currentSourceIndex = 0;

                AcousticSource source =
                    Sources[currentSourceIndex];

                currentSourceIndex++;

                if (source == null)
                    continue;

                UpdateSource(source);
            }
        }

        //==============================================================

        private void UpdateSource(AcousticSource source)
        {
            float distance =
                Vector3.Distance(
                    source.Position,
                    listener.position);

            if (distance > settings.cullingDistance)
                return;

            float occlusion =
                OcclusionSolver.Calculate(source);

            float diffraction =
                DiffractionSolver.Calculate(source);

            float material =
                OcclusionSolver.LastMaterialParameter;

            source.SetAcousticValues(
                occlusion,
                diffraction,
                distance,
                material);

            source.ApplyParameters(
                settings.interpolationSpeed);
        }

        //==============================================================

        public static void Register(AcousticSource source)
        {
            if (source == null)
                return;

            if (!Sources.Contains(source))
                Sources.Add(source);
        }

        //==============================================================

        public static void Unregister(AcousticSource source)
        {
            if (source == null)
                return;

            Sources.Remove(source);
        }

        //==============================================================

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (listener == null)
                return;

            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(
                listener.position,
                0.25f);
        }

#endif
    }
}