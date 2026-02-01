using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerLighter : MonoBehaviour
    {
        [SerializeField] private GameObject _flash;
        private bool Block = false;
        
        public void Flash()
        {
            if (Block) return;
            if (_flash.activeSelf == false) _flash.SetActive(true);
            else _flash.SetActive(false);
        }

        public void BlockFlash()
        {
            Block = true;
        }
        public void UnblockFlash()
        {
            Block = false;
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
