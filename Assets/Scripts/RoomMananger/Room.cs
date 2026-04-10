using System;
using Player;
using UI;
using UnityEngine;

namespace RoomMananger
{
    public class Room : MonoBehaviour
    {
        public GameObject Key;

        [HideInInspector] public bool HaveKey;
        
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
