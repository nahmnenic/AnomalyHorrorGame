using System;
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

        private void Start()
        {
            ChangeVolume();
        }

        public void ChangeVolume()
        {
            _volumeTxt.text = _volumeValue.value.ToString();
        }
        
        public void ChangeSensetivity()
        {
            _sensTxt.text = Math.Round(_sensValue.value, 2).ToString();
        }
    }
}
