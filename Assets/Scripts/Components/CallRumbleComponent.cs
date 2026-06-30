using Player;
using UnityEngine;

namespace Components
{
    public class CallRumbleComponent : MonoBehaviour
    {
        [SerializeField] private float _low;
        [SerializeField] private float _high;
        [SerializeField] private float _duration;
        
        public void Pulse()
        {
            RumbleMananger.instance.RumblePulse(_low, _high, _duration);
        }
    }
}