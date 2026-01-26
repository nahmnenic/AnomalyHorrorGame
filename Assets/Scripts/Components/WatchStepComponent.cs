using System;
using System.Collections;
using UnityEngine;

namespace Components
{
    public class WatchStepComponent : MonoBehaviour
    {
        [SerializeField] private Transform _minuts;
        [SerializeField] private Transform _seconds;
        public bool Back;
        private int _allSec = 0;

        private void Start()
        {
            StartCoroutine(WatchStep());
        }

        private IEnumerator WatchStep()
        {
            while (true)
            {
                if (Back)
                {
                    _seconds.localEulerAngles = new Vector3(0, _allSec*-6, 0);
                    if (_allSec%60 == 0) _minuts.localEulerAngles = new Vector3(0, _allSec/60 * -6, 0);
                }
                else
                {
                    _seconds.localEulerAngles = new Vector3(0, _allSec*6, 0);
                    if (_allSec%60 == 0) _minuts.localEulerAngles = new Vector3(0, _allSec/60 * 6, 0);
                }
                _allSec++;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
