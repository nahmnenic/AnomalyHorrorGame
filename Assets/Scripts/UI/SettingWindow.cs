using System;
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

        private void Start()
        {
            ChangeVolume();
        }

        public void ChangeVolume()
        {
            _volumeTxt.text = _volumeValue.value.ToString();
            string vcaPath = "vca:/Master";
            FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
            vca.setVolume(_volumeValue.value / 100);
        }
        
        public void ChangeSensetivity()
        {
            _sensTxt.text = Math.Round(_sensValue.value, 2).ToString();
            FindFirstObjectByType<CameraController>().mouseSense = _sensValue.value / 10;
        }
    }
}
