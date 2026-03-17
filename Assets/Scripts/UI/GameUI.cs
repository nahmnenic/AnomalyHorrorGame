using System;
using System.Collections;
using Interact;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text _timerText;
        [SerializeField] private float _time;
        private bool _timerFlag;
        
        private PlayerInteraction _playerInteraction;

        [SerializeField] private TMP_Text _promtInteract;
        [SerializeField] private Image _blackScreen;
        [SerializeField] private Image _whiteScreen;
        
        [HideInInspector] public bool Board;
        [HideInInspector] public bool Chair;
        [HideInInspector] public bool Sofa;
        
        [HideInInspector] public bool BoardTrue;
        [HideInInspector] public bool ChairTrue;
        [HideInInspector] public bool SofaTrue;

        private void Start()
        {
            _playerInteraction =  FindObjectOfType<PlayerInteraction>();
            StartCoroutine(Timer());
        }
        

        private IEnumerator Timer()
        {
            while (_time >= 0)
            {
                CheckTimeRound();
                float minutes = Mathf.FloorToInt(_time / 60);
                float seconds = Mathf.FloorToInt(_time % 60);
                _timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
                yield return new WaitForSeconds(1f);
                
                if (!_playerInteraction.BlockMove)
                    _time--;
            }
        }

        private void BlackScreen()
        {
            _promtInteract.gameObject.SetActive(false);
            _blackScreen.gameObject.SetActive(true);
        }
        
        private void WhiteScreen()
        {
            _promtInteract.gameObject.SetActive(false);
            _whiteScreen.gameObject.SetActive(true);
        }

        private void CheckTimeRound()
        {
            if (Board && Chair && Sofa && !_timerFlag)
            {
                _time = 10;
                _timerFlag = true;
            }

            if (_time == 0 && BoardTrue && ChairTrue && SofaTrue)
            {
                WhiteScreen();
                
            }
            else if (_time == 0)
            {
                BlackScreen();
            }
        }
    }
}
