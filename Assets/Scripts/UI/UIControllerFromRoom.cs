using System;
using UnityEngine;

namespace UI
{
    public class UIControllerFromRoom : MonoBehaviour
    {
        private GameUI _gameUI;

        private void Awake()
        {
            _gameUI =  FindFirstObjectByType<GameUI>();
        }

        public void ChangeBlackScreen()
        {
            if(_gameUI._blackScreen.gameObject.activeSelf) _gameUI._blackScreen.gameObject.SetActive(false);
            else _gameUI._blackScreen.gameObject.SetActive(true);
        }
    }
}
