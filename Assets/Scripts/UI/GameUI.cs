using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text _timerText;
        [SerializeField] private float _time;

        private void Start()
        {
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
                _time--;  
            }
        }
    }
}
