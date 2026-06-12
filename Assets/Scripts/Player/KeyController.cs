using System.Collections.Generic;
using Props;
using RoomMananger;
using UnityEngine;

namespace Player
{
    public class KeyController : MonoBehaviour
    {
        private bool _blackKitchenKey = true;
        private bool _blackStorageKey = true;
        private bool _blackBathroomKey = true;
        private bool _blackChildrenKey = true;
        public bool _exampleKey = true;
        private bool _mainRoomKey = true;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
        }

        public void ChangeKeyState(GameObject room)
        {
            var roomName = room.GetComponent<Room>().name;
            switch (roomName)
            {
                case Room.Name.Kitchen:
                    _blackKitchenKey = false;
                    break;
                case Room.Name.Storage:
                    _blackStorageKey = false;
                    break;
                case Room.Name.Bathroom:
                    _blackBathroomKey = false;
                    break;
                case Room.Name.Children:
                    _blackChildrenKey = false;
                    break;
            }

            _inventory.Key = true;
        }
        
        /*public void AddExampleKey()
        {
            _inventory.Key = true;
        }*/

        public void UpdateKeyRoom()
        {
            var rooms = FindObjectsOfType<Room>();
            foreach (var room in rooms)
            {
                if(room.color == Room.Color.Black) Debug.Log(room.name);
                if (room.color == Room.Color.Black && room.name == Room.Name.Kitchen && !_blackKitchenKey)
                {
                    room.Key.SetActive(false);
                }
                else if (room.color == Room.Color.Black && room.name == Room.Name.Storage && !_blackStorageKey)
                {
                    room.Key.SetActive(false);
                }
                else if (room.color == Room.Color.Black && room.name == Room.Name.Bathroom && !_blackBathroomKey)
                {
                    room.Key.SetActive(false);
                }
                else if (room.color == Room.Color.Black && room.name == Room.Name.Children && !_blackChildrenKey)
                {
                    room.Key.SetActive(false);
                }
            }
        }
        
        public bool HaveKeyInInventory()
        {
            return _inventory.Key;
        }

        public void DeleteExampleKey()
        {
            _exampleKey = false;
        }

        public void SetKey()
        {
            if (!HaveKeyInInventory()) return;
            _inventory.Key = false;
        }

        public void DeleteKey()
        {
            /*if (!_exampleKey)
            {
                FindFirstObjectByType<ExitDoorKeys>().DeleteRandomKey();
                _exampleKey = true;
            }*/
            
            if (_blackBathroomKey && _blackStorageKey && _blackKitchenKey && _blackChildrenKey) return;
            List<int> falseIndexes = new List<int>();

            if (!_blackBathroomKey) falseIndexes.Add(0);
            if (!_blackStorageKey) falseIndexes.Add(1);
            if (!_blackKitchenKey) falseIndexes.Add(2);
            if (!_blackChildrenKey) falseIndexes.Add(3);

            int randomIndex = falseIndexes[Random.Range(0, falseIndexes.Count)];

            switch (randomIndex)
            {
                case 0: _blackBathroomKey = true; break;
                case 1: _blackStorageKey = true; break;
                case 2: _blackKitchenKey = true; break;
                case 3: _blackChildrenKey = true; break;
            }
            
            FindFirstObjectByType<ExitDoorKeys>().DeleteRandomKey();
        }
    }
}
