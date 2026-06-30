using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class RumbleMananger : MonoBehaviour
    {
        public static RumbleMananger instance;

        private Gamepad _pad;
        private InputDeviceManager _inputDeviceManager;

        private Coroutine _currentRumble;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            
            _inputDeviceManager = GetComponentInParent<InputDeviceManager>();
        }

        public void RumblePulse(float low, float high, float duration)
        {
            _pad = Gamepad.current;
            
            if (_pad != null && _inputDeviceManager.UsingGamepad)
            {
                _pad.SetMotorSpeeds(low, high);
                _currentRumble = StartCoroutine(StopRumble(duration, _pad));
            }
        }

        private IEnumerator StopRumble(float duration, Gamepad gamepad)
        {
            yield return new WaitForSeconds(duration);
            gamepad.SetMotorSpeeds(0f,0f);
        }
    }
}
