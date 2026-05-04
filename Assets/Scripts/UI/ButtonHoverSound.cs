using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ButtonHoverSound : MonoBehaviour
    {
        public UnityEvent onSelected;

        public void OnSelect(BaseEventData eventData)
        {
            onSelected?.Invoke();
            Debug.Log("SELECT");
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("HOVER");
        }
    }
}
