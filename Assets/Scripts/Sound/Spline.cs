using UnityEngine;

namespace Sound
{
    public class Spline : MonoBehaviour
    {
        private Vector3[] _splinePoints;
        private float[] _segmentLengths;
        private float[] _segmentStartDistances;
        private int _splineCount;
        private float _totalLength;
        public bool debug_DrawSpline = false;

        private void Start()
        {
            UpdateSplinePoints();
        }

        private void Update()
        {
            UpdateSplinePoints();

            if (_splineCount > 1 && debug_DrawSpline)
            {
                for (int i = 0; i < _splineCount - 1; i++)
                {
                    Debug.DrawLine(_splinePoints[i], _splinePoints[i + 1], Color.green);
                }
            }
        }

        private void UpdateSplinePoints()
        {
            _splineCount = transform.childCount;
            if (_splineCount == 0) return;

            if (_splinePoints == null || _splinePoints.Length != _splineCount)
            {
                _splinePoints = new Vector3[_splineCount];
                int segCount = Mathf.Max(0, _splineCount - 1);
                _segmentLengths = new float[segCount];
                _segmentStartDistances = new float[segCount];
            }

            for (int i = 0; i < _splineCount; i++)
            {
                _splinePoints[i] = transform.GetChild(i).position;
            }

            _totalLength = 0f;
            for (int i = 0; i < _splineCount - 1; i++)
            {
                _segmentLengths[i] = Vector3.Distance(_splinePoints[i], _splinePoints[i + 1]);
                _segmentStartDistances[i] = _totalLength;
                _totalLength += _segmentLengths[i];
            }
        }

        /// <summary>
        /// Ближайшая точка на сплайне к pos. Плавная, без рывков на стыках.
        /// </summary>
        public Vector3 WhereOnSpline(Vector3 pos)
        {
            if (_splineCount < 2) return _splineCount > 0 ? _splinePoints[0] : pos;
            return GetClosestProjection(pos).point;
        }

        /// <summary>
        /// Расстояние от начала сплайна до ближайшей проекции pos.
        /// </summary>
        public float GetDistanceOnSpline(Vector3 pos)
        {
            if (_splineCount < 2) return 0f;
            return GetClosestProjection(pos).distanceFromStart;
        }

        /// <summary>
        /// Точка на сплайне на заданном расстоянии от начала.
        /// </summary>
        public Vector3 GetPointAtDistance(float distanceFromStart)
        {
            if (_splineCount < 2) return _splineCount > 0 ? _splinePoints[0] : Vector3.zero;

            distanceFromStart = Mathf.Clamp(distanceFromStart, 0f, _totalLength);

            for (int i = 0; i < _splineCount - 1; i++)
            {
                float segLen = _segmentLengths[i];
                if (_segmentStartDistances[i] + segLen >= distanceFromStart || i == _splineCount - 2)
                {
                    float t = segLen > 0.0001f ? (distanceFromStart - _segmentStartDistances[i]) / segLen : 0f;
                    return Vector3.Lerp(_splinePoints[i], _splinePoints[i + 1], t);
                }
            }

            return _splinePoints[_splineCount - 1];
        }

        /// <summary>
        /// Ищет ближайшую проекцию на ВСЕ сегменты сплайна.
        /// </summary>
        private (Vector3 point, float distanceFromStart) GetClosestProjection(Vector3 pos)
        {
            Vector3 bestPoint = _splinePoints[0];
            float bestSqrDist = float.MaxValue;
            int bestSegIndex = 0;
            float bestT = 0f;

            for (int i = 0; i < _splineCount - 1; i++)
            {
                Vector3 projected = ProjectOnSegment(_splinePoints[i], _splinePoints[i + 1], pos, out float t);
                float sqrDist = (projected - pos).sqrMagnitude;

                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    bestPoint = projected;
                    bestSegIndex = i;
                    bestT = t;
                }
            }

            float distFromStart = _segmentStartDistances[bestSegIndex] + _segmentLengths[bestSegIndex] * bestT;
            return (bestPoint, distFromStart);
        }

        /// <summary>
        /// Проекция точки на отрезок с параметром t [0,1].
        /// </summary>
        private Vector3 ProjectOnSegment(Vector3 a, Vector3 b, Vector3 pos, out float t)
        {
            Vector3 ab = b - a;
            Vector3 ap = pos - a;
            float abLenSqr = ab.sqrMagnitude;

            if (abLenSqr < 0.0001f)
            {
                t = 0f;
                return a;
            }

            t = Mathf.Clamp01(Vector3.Dot(ap, ab) / abLenSqr);
            return Vector3.Lerp(a, b, t);
        }
    }
}