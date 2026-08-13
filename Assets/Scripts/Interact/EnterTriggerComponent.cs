using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        public UnityEvent OnEnterTrigger;
        public UnityEvent OnExitTrigger;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                OnEnterTrigger.Invoke();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                OnExitTrigger.Invoke();
            }
        }
    }
}
