using System.Collections;
using Player;
using UnityEngine;

namespace Components
{
    public class StressControlComponent : MonoBehaviour
    {
        [Header("Stress Parameters")]
        [Tooltip("Целевой уровень стресса.")]
        [Range(0f, 1f)]
        [SerializeField] private float _stressTarget;

        [Tooltip("Сколько секунд удерживать стресс.")]
        [SerializeField] private float _stressDelay;

        private PlayerStamina _playerStamina;
        private Coroutine _stressCoroutine;


        private void Awake()
        {
            _playerStamina = FindFirstObjectByType<PlayerStamina>();
        }


        /// <summary>
        /// Добавляет стресс и удерживает его заданное время.
        /// </summary>
        public void AddStress()
        {
            if (_playerStamina == null)
                return;

            if (_stressCoroutine != null)
                StopCoroutine(_stressCoroutine);

            _stressCoroutine = StartCoroutine(StressRoutine());
        }


        private IEnumerator StressRoutine()
        {
            _playerStamina.SetStress(_stressTarget);

            yield return new WaitForSeconds(_stressDelay);

            _playerStamina.ReleaseStress();

            _stressCoroutine = null;
        }

        /// <summary>
        /// Постоянное удержание стресса
        /// </summary>
        
        public void StartStress()
        {
            _playerStamina.SetStress(_stressTarget);
        }

        public void StopStress()
        {
            _playerStamina.ReleaseStress();
        }

        /// <summary>
        /// Немедленно снимает удержание стресса.
        /// </summary>
        public void RemoveStress()
        {
            if (_playerStamina == null)
                return;

            if (_stressCoroutine != null)
            {
                StopCoroutine(_stressCoroutine);
                _stressCoroutine = null;
            }

            _playerStamina.ReleaseStress();
        }


        private void OnDisable()
        {
            if (_stressCoroutine != null)
            {
                StopCoroutine(_stressCoroutine);
                _stressCoroutine = null;
            }
        }
    }
}