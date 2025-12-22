using UnityEngine;

namespace Components
{
    public class MovingObjectComponent : MonoBehaviour
    {
        [SerializeField] private Transform[] _allPos;
        [SerializeField] private Transform _objToMove;
        private int _currentPos;
        public bool Disposable;
        private bool _supDispos;

        public void MoveObject()
        {
            if (Disposable)
            {
                _objToMove.position = _allPos[_currentPos+1].position;
                _objToMove.rotation = _allPos[_currentPos+1].rotation;
                _supDispos = false;
            }
            else if (_supDispos)
            {
                if (_currentPos <= _allPos.Length-2)
                {
                    _objToMove.position = _allPos[_currentPos+1].position;
                    _objToMove.rotation = _allPos[_currentPos+1].rotation;
                    _currentPos++;
                }
                else
                {
                    _objToMove.position = _allPos[0].position;
                    _objToMove.rotation = _allPos[0].rotation;
                    _currentPos = 0;
                }
            }
        }
    }
}
