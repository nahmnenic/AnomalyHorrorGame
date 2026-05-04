using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
    {
        public UnityEvent onHover;
        public StudioEventEmitter HoverSound;
        
        public float minInterval = 0.6f;
        private float lastPlayTime;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Time.time - lastPlayTime < minInterval)
                return;

            HoverSound.Play();
            lastPlayTime = Time.time;
        }
    }
}
