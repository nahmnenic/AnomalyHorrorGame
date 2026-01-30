using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class RotateObjectComponent : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private float _rotateTo;
        [SerializeField] private float _rotateFrom;
        [SerializeField] private float _delay;
        private bool _delayApproved = true;
        private Coroutine _rotationCoroutine;
        public bool Stable;
        public bool Spin;
        
        private bool movingToTarget = true;

        public enum Axis
        {
            X = 0,
            Y = 1, 
            Z = 2
        }

        public Axis axis;

        private void Update()
        {
            if (_delay != 0)
            {
                _delayApproved = false;
                StartCoroutine(DelayImitate());
            }
            if (!_delayApproved) return;
            float currentAngle;
            float target;
            float newAngle;
            switch (axis)
            {
                case Axis.X:
                    if (Spin && !movingToTarget && _rotationCoroutine == null) _rotationCoroutine = StartCoroutine(RotateContinuously(0));
                    if (Spin) return;
                    currentAngle = transform.localEulerAngles.x;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(newAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    if(newAngle == target && Stable) Rotate();
                    break;
                case Axis.Y:
                    if (Spin && !movingToTarget && _rotationCoroutine == null) _rotationCoroutine = StartCoroutine(RotateContinuously(1));
                    if (Spin) return;
                    currentAngle = transform.localEulerAngles.y;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newAngle, transform.localEulerAngles.z);
                    if(currentAngle == target && Stable) Rotate();
                    break;
                case Axis.Z:
                    if (Spin && !movingToTarget && _rotationCoroutine == null) _rotationCoroutine = StartCoroutine(RotateContinuously(2));
                    if (Spin) return;
                    currentAngle = transform.localEulerAngles.z;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newAngle);
                    if(currentAngle == target && Stable) Rotate();
                    break;
            }
        }
    
        public void Rotate()
        {
            movingToTarget = !movingToTarget;
            if (Spin && movingToTarget)
            {
                StopCoroutine(RotateContinuously(1));
                _rotationCoroutine = null;
            }
        }

        private IEnumerator DelayImitate()
        {
            yield return new WaitForSeconds(_delay);
            _delay = 0;
            _delayApproved = true;
        }
        
        private IEnumerator RotateContinuously(int axis)
        {
            while (!movingToTarget)
            {
                switch (axis)
                {
                    case 0:
                        transform.Rotate(_rotateSpeed * Time.deltaTime, 0, 0);
                        yield return null;
                        break;
                    case 1:
                        transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);
                        yield return null;
                        break;
                    case 2:
                        transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
                        yield return null;
                        break;
                }
            }
        }
    }
}
