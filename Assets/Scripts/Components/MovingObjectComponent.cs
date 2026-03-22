using System;
using Player;
using UnityEngine;

namespace Components
{
    public class MovingObjectComponent : MonoBehaviour
    {
        [SerializeField] private Transform[] _allPos;
        [SerializeField] private Transform _objToMove;
        [SerializeField] private SoundController _soundController;
        private int _currentPos = 0;
        
        public bool Disposable;
        [HideInInspector] public bool _supDispos =false;

        public void MoveObject()
        {
            if(_supDispos) return;
            _objToMove.position = _allPos[_currentPos+1].position;
            _objToMove.rotation = _allPos[_currentPos+1].rotation;
            if(_soundController!=null) _soundController.PlaySound();
            if(Disposable) _supDispos = true;
        }
    }
}
