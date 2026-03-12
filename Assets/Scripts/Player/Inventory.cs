using Interact;
using RoomMananger;
using UnityEngine;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject ChairPrefab;
        [SerializeField] private GameObject BoardPrefab;

        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private PlayerInteractionDoor _doorInteraction;

        [Header("Cheats")] public bool Skip;
        
        public bool Chair;
        public bool Boards;


        private bool CheckRooms()
        {
            bool flag = true;
            if(Skip) return true;
            var rooms = FindObjectsOfType<Room>();
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].EntranceRoom)
                {
                    Debug.Log($"Посетите все комнаты: {rooms[i].gameObject.name}");
                    flag = false;
                }

                if (!rooms[i].CloseDoor())
                {
                    Debug.Log($"Закройте все двери: {rooms[i].gameObject.name}");
                    flag = false;
                }
            }
            
            return flag;
        }

        private void ChangeMode()
        {
            if (Chair || Boards)
            {
                _playerInteraction.enabled = false;
                _playerInteraction.Hide();
                _doorInteraction.enabled = true;
            }
            else
            {
                _doorInteraction.Hide();
                _playerInteraction.enabled = true;
                _doorInteraction.enabled = false;
            }
        }
        
        public void AddChair()
        {
            if (!CheckRooms()) return;
            
            if(Boards) return;
            Chair = true;
            ChairPrefab.SetActive(false);
            ChangeMode();
        }

        public void DeleteChair()
        {
            Chair = false;
            ChangeMode();
        }

        public void AddBoard()
        {
            if (!CheckRooms()) return;
            
            if(Chair) return;
            Boards = true;
            BoardPrefab.SetActive(false);
            ChangeMode();
        }

        public void DeleteBoards()
        {
            Boards = false;
            ChangeMode();
        }
    }
}
