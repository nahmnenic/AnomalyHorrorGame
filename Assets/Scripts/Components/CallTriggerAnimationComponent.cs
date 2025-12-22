using System;
using UnityEngine;

namespace Components
{
    public class CallTriggerAnimationComponent : MonoBehaviour
    {
        [SerializeField] private string _animationName;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void CallTrigger()
        {
            _animator.SetTrigger(_animationName);
        }
    }
}
