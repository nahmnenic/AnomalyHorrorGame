using System;
using Data;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingWindow : MonoBehaviour
    {
        [SerializeField] private Text _volumeTxt;
        [SerializeField] private Slider _volumeValue;
        
        [SerializeField] private Text _sensTxt;
        [SerializeField] private Slider _sensValue;
        
        private CameraController _cameraController;
        private GameSession _gameSession;

        private void Start()
        {
            _gameSession = FindFirstObjectByType<GameSession>();
            _cameraController = FindFirstObjectByType<CameraController>();
            
            _volumeValue.SetValueWithoutNotify(_gameSession.Volume);
            _sensValue.SetValueWithoutNotify(_gameSession.Sensitivity);
            
            ChangeVolume();
            ChangeSensetivity();
        }

        public void ChangeVolume()
        {
            _volumeTxt.text = _volumeValue.value.ToString();
            string vcaPath = "vca:/Master";
            FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
            vca.setVolume(_volumeValue.value / 100);
            _gameSession.Volume = _volumeValue.value;
        }
        
        public void ChangeSensetivity()
        {
            _sensTxt.text = Math.Round(_sensValue.value, 2).ToString();
            _cameraController.mouseSense = _sensValue.value / 10;
            _gameSession.Sensitivity = _sensValue.value;
        }
    }
}
