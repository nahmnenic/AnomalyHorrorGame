using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerLighter : MonoBehaviour
    {
        public GameObject _flash;
        public GameObject _flashFlicker;
        [SerializeField] private FMODUnity.StudioEventEmitter _clickSound;
        [SerializeField] private FMODUnity.StudioEventEmitter _flickerSound;
        private bool Block = false;
        
        public void Flash()
        {
            if (Block) return;
            _clickSound.Play();
            if (_flash.activeSelf == false) _flash.SetActive(true);
            else _flash.SetActive(false);
        }

        public void BlockFlash()
        {
            Block = true;
            _flash.GetComponent<Light>().enabled = false;
        }
        public void UnblockFlash()
        {
            Block = false;
            _flash.GetComponent<Light>().enabled = true;
        }

        public void StartFlicker()
        {
            _flickerSound.Play();
            _flashFlicker.SetActive(true);
        }

        public void StopFlicker()
        {
            _flickerSound.Stop();
            _flashFlicker.SetActive(false);
        }

        public void StopFlash(float _delay)
        {
            StartCoroutine(StopFlashing(_delay));
        }

        private IEnumerator StopFlashing(float _delay)
        {
            yield return new WaitForSeconds(_delay);
            _flash.SetActive(false);
        }
    }
}
