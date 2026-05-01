using System;
using Components;
using Player;
using UI;
using UnityEngine;

namespace RoomMananger
{
    public class Room : MonoBehaviour
    {
        public GameObject Key;
        
        private bool _closeDoor;
        public bool EntranceRoom = false;

        public Transform Door;
        
        private GameUI _gameUI;
        private LightFlickerComponent _lightFlicker;
        private PlayerLighter _playerLighter;

        private void Awake()
        {
            _gameUI = FindObjectOfType<GameUI>();
            _lightFlicker = FindFirstObjectByType<LightFlickerComponent>();
            _playerLighter = FindFirstObjectByType<PlayerLighter>();
        }

        public void ExitRoomFlahLight()
        {
            _playerLighter.StopFlicker();
            _playerLighter.UnblockFlash();
        }

        public void StartFlicker()
        {
            _playerLighter.StartFlicker();
            _playerLighter.BlockFlash();
        }

        public void StopPlayerFlashLight()
        {
            _playerLighter._flash.SetActive(false);
            _playerLighter._flashFlicker.SetActive(false);
        }

        public bool CloseDoor()
        {
            if (Door.localEulerAngles.y == 0) return true;
            
            return false;
        }
        
        public void Entrance()
        {
            EntranceRoom = true;
        }

        public void ChengeNameRoom(GameObject room)
        {
            _gameUI.ChengeRoomName(room);
        }
        
        public enum Color
        {
            Black = 0,
            Yellow = 1, 
            Red = 2,
            Green = 3,
        }
        
        public enum Name
        {
            Kitchen = 0,
            Bathroom = 1, 
            Badroom = 2,
            Storage = 3,
            Children = 4
        }

        public Color color;
        public Name name;
    }
}
