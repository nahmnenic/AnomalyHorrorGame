using UnityEngine;
using UnityEngine.Events;

namespace Interact
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        public UnityEvent OnEnterTrigger;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                OnEnterTrigger.Invoke();
            }
        }
    }
}
