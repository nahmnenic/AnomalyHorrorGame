using System.Collections;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Player;

namespace Sound
{
    public class AmbZone : MonoBehaviour
    {
        [Header("Events")]
        public EventReference Amb;
        public EventReference Reverb;
        private EventInstance _ambInstance;
        private EventInstance _reverbInstance;

        [Header("Properties")]
        [SerializeField] private float _hieght;
        [SerializeField] private float _fadeInTime;
        [SerializeField] private AnimationCurve _curveIn = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float _fadeOutTime;
        [SerializeField] private AnimationCurve _curveOut = AnimationCurve.Linear(0, 1, 1, 0);

        private float _currentProgress;
        private SoundManager _soundManager;
        
        [Header("Priority")]
        public int Priority;
        public enum GameZone
        {
            Basement,
            FirstFloor,
            SecondFloor
        }
        public GameZone zone;

        private void Awake()
        {
            _ambInstance = RuntimeManager.CreateInstance(Amb);
            _reverbInstance = RuntimeManager.CreateInstance(Reverb);
            _soundManager = FindFirstObjectByType<SoundManager>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _soundManager.AddZone(this);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _soundManager.RemoveZone(this);
        }
        
        private IEnumerator Fade(float from, float to, float duration, AnimationCurve curve)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float curveT = curve.Evaluate(t);

                _currentProgress = Mathf.Lerp(from, to, curveT);
                _ambInstance.setVolume(_currentProgress);
                _reverbInstance.setVolume(_currentProgress);

                yield return null;
            }

            _currentProgress = to;
            _ambInstance.setVolume(_currentProgress);
            _reverbInstance.setVolume(_currentProgress);
        }
        
        public void FadeIn()
        {
            PLAYBACK_STATE state;
            _ambInstance.getPlaybackState(out state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                _ambInstance.start();
                _reverbInstance.start();
            }

            StopAllCoroutines();
            StartCoroutine(Fade(_currentProgress, 1f, _fadeInTime, _curveIn));
        }

        public void FadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(_currentProgress, 0f, _fadeOutTime, _curveOut));
        }
    }
}
