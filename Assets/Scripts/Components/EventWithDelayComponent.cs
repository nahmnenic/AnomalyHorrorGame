using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class EventWithDelayComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _actions;
        [SerializeField] private float _delay;

        public void StartActions()
        {
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
