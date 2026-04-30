using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class EventWithDelayComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _actions;
        [SerializeField] private float _delay;
        [SerializeField] private bool _disposable;

        private bool _disposed;
        
        public void StartActions()
        {
            if(_disposed) return;
            if (_disposable) _disposed = true;
            StopAllCoroutines();
            StartCoroutine(Actions());
        }
        
        private IEnumerator Actions()
        {
            yield return new WaitForSeconds(_delay);
            _actions?.Invoke();
            yield return null;
        }
    }
}
