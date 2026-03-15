using System;
using System.Collections;
using Interact;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text _timerText;
        [SerializeField] private float _time;
        
        private PlayerInteraction _playerInteraction;

        private void Start()
        {
            _playerInteraction =  FindObjectOfType<PlayerInteraction>();
            StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            while (_time > 0)
            {
                float minutes = Mathf.FloorToInt(_time / 60);
                float seconds = Mathf.FloorToInt(_time % 60);
                _timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

                yield return new WaitForSeconds(1f);

                if (!_playerInteraction.BlockMove)
                    _time--; 
            }
        }
    }
}
