using RoomMananger;
using UnityEngine;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject ChairPrefab;
        [SerializeField] private GameObject BoardPrefab;
        public bool Chair;
        public bool Boards;


        private bool CheckRooms()
        {
            bool flag = true;
            var rooms = FindObjectsOfType<Room>();
            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i].EntranceRoom)
                {
                    Debug.Log("Посетите все комнаты");
                    flag = false;
                }

                if (!rooms[i].CloseDoor())
                {
                    Debug.Log("Закройте все двери");
                    flag = false;
                }
            }
            
            return flag;
        }
        
        public void AddChair()
        {
            if (!CheckRooms()) return;
            
            if(Boards) return;
            Chair = true;
            ChairPrefab.SetActive(false);
        }

        public void DeleteChair()
        {
            Chair = false;
        }

        public void AddBoard()
        {
            if (!CheckRooms()) return;
            
            if(Chair) return;
            Boards = true;
            BoardPrefab.SetActive(false);
        }

        public void DeleteBoards()
        {
            Boards = false;
        }
    }
}
