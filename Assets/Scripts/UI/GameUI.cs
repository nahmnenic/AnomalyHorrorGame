using System;
using System.Collections;
using Components;
using Interact;
using Player;
using Props;
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
        
        public TMP_Text _roomName;

        [Header("Start Download")] 
        [SerializeField] private GameObject _settingWindow;
        [SerializeField] private GameObject _downloadScreen;
        [SerializeField] private GameObject _interactUI;
        
        private PlayerInteraction _playerInteraction;
        private RoomController _roomController;
        private LoadObjectGameComponent _loadObjectGameComponent;
        private KeyController _keyController;
        private UIManager _uiMananger;

        [SerializeField] private TMP_Text _promtInteract;
         public Image _blackScreen;
        [SerializeField] private Image _whiteScreen;
        [SerializeField] private Text _winText;

        [SerializeField] private float _fadeSpeed;
        
        [HideInInspector] public bool Board;
        [HideInInspector] public bool Chair;
        public bool Sofa;
        
        public bool BoardTrue;
        public bool ChairTrue;
        public bool SofaTrue;

        public bool SkipWhiteScreen;

        private void Start()
        {
            BlockCursor();
            _playerInteraction =  FindObjectOfType<PlayerInteraction>();
            _roomController =  FindObjectOfType<RoomController>();
            _keyController =  FindObjectOfType<KeyController>();
            _loadObjectGameComponent = GetComponent<LoadObjectGameComponent>();
            _uiMananger =  FindObjectOfType<UIManager>();
            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            _uiMananger.BlockMove = true;
            yield return new WaitForSeconds(1f);
            _settingWindow.SetActive(false);
            _downloadScreen.SetActive(false);
            _interactUI.SetActive(true);
            StartCoroutine(Timer());
            _uiMananger.BlockMove = false;
        }
        
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public void BlockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
                
                if (!_uiMananger.BlockMove)
                    _time--;
            }
        }

        private void BlackScreen()
        {
            Debug.Log("@@@LOSE@@@");
            _promtInteract.gameObject.SetActive(false);
            _blackScreen.gameObject.SetActive(true);
            ReloadGame();
        }
        
        private void WhiteScreen()
        {
            _promtInteract.gameObject.SetActive(false);
            _whiteScreen.gameObject.SetActive(true);
            ReloadGame();
            Debug.Log("@@@WIN@@@");
        }

        private void CheckTimeRound()
        {
            if (Board && Chair && Sofa && !_timerFlag)
            {
                _time = 10;
                _timerFlag = true;
            }

            if (_time == 1)
            {
                Debug.Log("______________________________");
                Debug.Log("______________________________");
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
            _uiMananger.BlockMove = true;
            
            yield return new WaitForSeconds(2f);

            _uiMananger.BlockMove = false;
            AllDebug();
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
            _time = 1000f;
            StartCoroutine(WhiteWinGameScreen());
        }

        private IEnumerator WhiteWinGameScreen()
        {
            _timerText.gameObject.SetActive(false);
            yield return new WaitForSeconds(5f);
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

        private void AllDebug()
        {
            _keyController.DebugAllKeys();
            FindFirstObjectByType<ExitDoorKeys>().DebugAllKeys();
        }
    }
}
