using System;
using UI;
using UnityEngine;

namespace RoomMananger
{
    public class Room : MonoBehaviour
    {
        private bool _closeDoor;
        public bool EntranceRoom = false;

        public Transform Door;
        
        private GameUI _gameUI;

        private void Awake()
        {
            _gameUI = FindObjectOfType<GameUI>();
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
    }
}
