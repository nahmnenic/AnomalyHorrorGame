using System;
using UnityEngine;

namespace Components
{
    public class OpenCabinetComponent : MonoBehaviour
    {
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private float _speed;
        
        public bool _move = false;
        public bool _vector = true;
        public float _progress;
    
        public enum Axis
        {
            X = 0,
            Y = 1, 
            Z = 2
        }

        public Axis axis;
        

        private void Update()
        {
            if(!_move) return;
            float currentPos;
            _progress += Time.fixedDeltaTime * _speed;
            switch (axis)
            {
                case Axis.X:
                    if (_vector) currentPos = Mathf.Lerp(_pointA.localPosition.x, _pointB.localPosition.x, _progress);
                    else currentPos = Mathf.Lerp(_pointB.localPosition.x, _pointA.localPosition.x, _progress);
                    transform.localPosition = new Vector3(currentPos, transform.localPosition.y, transform.localPosition.z);
                    if (_progress >= 1)
                    {
                        _move = false;
                        _progress = 0;
                    }
                    break;
                case Axis.Y:
                    if (_vector) currentPos = Mathf.Lerp(_pointA.localPosition.y, _pointB.localPosition.y, _progress);
                    else currentPos = Mathf.Lerp(_pointB.localPosition.y, _pointA.localPosition.y, _progress);
                    transform.localPosition = new Vector3(transform.localPosition.x, currentPos, transform.localPosition.z);
                    if (_progress >= 1)
                    {
                        _move = false;
                        _progress = 0;
                    }
                    break;
                case Axis.Z:
                    if (_vector) currentPos = Mathf.Lerp(_pointA.localPosition.z, _pointB.localPosition.z, _progress);
                    else currentPos = Mathf.Lerp(_pointB.localPosition.z, _pointA.localPosition.z, _progress);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, currentPos);
                    if (_progress >= 1)
                    {
                        _move = false;
                        _progress = 0;
                    }
                    break;
            }
            
        }

        public void Move()
        {
            if(_move) return;
            _vector = !_vector;
            _move = true;
        }
    }
}
