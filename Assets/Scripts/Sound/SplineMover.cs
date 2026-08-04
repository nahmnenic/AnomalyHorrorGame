using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SplineMover : MonoBehaviour
    {
        [Header("Ссылки")]
        public Spline spline;
        [Tooltip("Игрок. Если не назначен — ищет PlayerLocomotion")]
        public Transform followTarget;

        [Header("Источники")]
        [Tooltip("Твой GameObject из проекта (Assets). НЕ объект со сцены!")]
        public GameObject sourcePrefab;
        [Tooltip("Сколько копий создать")]
        public int sourceCount = 1;

        [Header("Распределение")]
        [Tooltip("Расстояние между соседними источниками вдоль сплайна")]
        public float distanceStep = 5f;

        private List<GameObject> _spawned = new List<GameObject>();

        private void Start()
        {
            if (followTarget == null)
            {
                var player = FindFirstObjectByType<Player.PlayerLocomotion>();
                if (player != null) followTarget = player.transform;
            }

            if (spline == null || followTarget == null)
            {
                Debug.LogError("[SplineMover] Не назначены ссылки!", this);
                enabled = false;
                return;
            }

            SpawnSources();
        }

        private void SpawnSources()
        {
            if (sourcePrefab == null)
            {
                Debug.LogError("[SplineMover] Source Prefab не назначен!", this);
                return;
            }

            if (sourcePrefab.GetComponent<SplineMover>() != null)
            {
                Debug.LogError("[SplineMover] Prefab содержит SplineMover — будет рекурсия!", this);
                return;
            }

            if (sourcePrefab == gameObject)
            {
                Debug.LogError("[SplineMover] Prefab не может быть этим объектом!", this);
                return;
            }

            foreach (var s in _spawned)
                if (s != null) Destroy(s);
            _spawned.Clear();

            for (int i = 0; i < sourceCount; i++)
            {
                GameObject copy = Instantiate(sourcePrefab, transform);
                copy.name = $"{sourcePrefab.name}_copy_{i}";
                _spawned.Add(copy);
            }
        }

        private void Update()
        {
            if (spline == null || followTarget == null || _spawned.Count == 0)
                return;

            float centerDist = spline.GetDistanceOnSpline(followTarget.position);
            int middle = _spawned.Count / 2;

            for (int i = 0; i < _spawned.Count; i++)
            {
                if (_spawned[i] == null) continue;

                int offset = i - middle;
                float targetDist = centerDist + offset * distanceStep;
                Vector3 pos = spline.GetPointAtDistance(targetDist);
                _spawned[i].transform.position = pos;
            }
        }

        private void OnDestroy()
        {
            foreach (var s in _spawned)
                if (s != null) Destroy(s);
        }
    }
}