using UnityEngine;

namespace Components
{
    public class StableRotateComponent : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private float _rotateTo;
        [SerializeField] private float _rotateFrom;
        private float _copyTo;
        private float _copyFrom;
        public bool Break;
        public bool AllTime;
        
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
            float currentAngle;
            float target;
            float newAngle;
            switch (axis)
            {
                case Axis.X:
                    currentAngle = transform.localEulerAngles.x;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(newAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    if(newAngle == target && AllTime) Rotate();
                    break;
                case Axis.Y:
                    currentAngle = transform.localEulerAngles.y;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newAngle, transform.localEulerAngles.z);
                    if(currentAngle == target && AllTime) Rotate();
                    break;
                case Axis.Z:
                    currentAngle = transform.localEulerAngles.z;
                    target = movingToTarget ? _rotateFrom : _rotateTo;
                    newAngle = Mathf.MoveTowardsAngle(currentAngle, target, _rotateSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newAngle);
                    if(currentAngle == target && AllTime) Rotate();
                    break;
            }
        }
    
        public void Rotate()
        {
            movingToTarget = !movingToTarget;
        }

        public void BlockRotate()
        {
            _copyTo = _rotateTo;
            _rotateTo = _rotateFrom;
        }

        public void UnblockRotate()
        {
            _rotateTo = _copyTo;
        }
    }
}
