using System;
using System.Collections;
using Components;
using Interact;
using Player;
using RoomMananger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text _timerText;
        [SerializeField] public float _time;
        private bool _timerFlag;
        
        [SerializeField] private TMP_Text _roomName;
        
        private PlayerInteraction _playerInteraction;
        private RoomController _roomController;
        private LoadObjectGameComponent _loadObjectGameComponent;
        private KeyController _keyController;

        [SerializeField] private TMP_Text _promtInteract;
         public Image _blackScreen;
        [SerializeField] private Image _whiteScreen;
        [SerializeField] private Text _winText;

        [SerializeField] private float _fadeSpeed;
        
        [HideInInspector] public bool Board;
        [HideInInspector] public bool Chair;
        [HideInInspector] public bool Sofa;
        
        [HideInInspector] public bool BoardTrue;
        [HideInInspector] public bool ChairTrue;
        [HideInInspector] public bool SofaTrue;

        public bool SkipWhiteScreen;

        private void Start()
        {
            _playerInteraction =  FindObjectOfType<PlayerInteraction>();
            _roomController =  FindObjectOfType<RoomController>();
            _keyController =  FindObjectOfType<KeyController>();
            _loadObjectGameComponent = GetComponent<LoadObjectGameComponent>();
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
            ReloadGame();
        }
        
        private void WhiteScreen()
        {
            _promtInteract.gameObject.SetActive(false);
            _whiteScreen.gameObject.SetActive(true);
            ReloadGame();
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
            else if (SkipWhiteScreen && _time == 0)
            {
                WhiteScreen();
            }
            else if (_time == 0 && !SkipWhiteScreen)
            {
                _keyController.DeleteKey();
                BlackScreen();
            }
        }

        private void ReloadGame()
        {
            StopAllCoroutines();
            StartCoroutine(LoadGame());
        }

        private void ReturnBool()
        {
            Sofa = false;
            Board =  false;
            Chair = false;
            SofaTrue = false;
            ChairTrue = false;
            BoardTrue = false;
            _timerFlag = false;
        }
        
        private IEnumerator LoadGame()
        {
            _roomController.SwitchRoom();
            ReturnBool();
            _time = 300f;
            _loadObjectGameComponent.LoadObject();
            _playerInteraction.BlockMove = true;
            
            yield return new WaitForSeconds(2f);

            _playerInteraction.BlockMove = false;
            _blackScreen.gameObject.SetActive(false);
            _whiteScreen.gameObject.SetActive(false);
            _keyController.UpdateKeyRoom();
            _playerInteraction.Hide();
            StartCoroutine(Timer());
            yield return null;
        }

        public void ChengeRoomName(GameObject room)
        {
            _roomName.text = room.name;
        }

        public void WinGame()
        {
            StartCoroutine(WhiteWinGameScreen());
        }

        private IEnumerator WhiteWinGameScreen()
        {
            Color c = _whiteScreen.color;
            c.a = 0;
            _whiteScreen.color = c;
            _whiteScreen.gameObject.SetActive(true);
            var color = _whiteScreen.color;

            _winText.gameObject.SetActive(true);
            while (color.a < 1f)
            {
                color.a += _fadeSpeed * Time.deltaTime;
                _whiteScreen.color = color;
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            GetComponent<LoadSceneComponent>().LoadScene();
        }
    }
}
