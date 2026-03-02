using UnityEngine;

namespace RoomMananger
{
    public class Room : MonoBehaviour
    {
        private bool _closeDoor;
        public bool EntranceRoom = false;

        public Transform Door;

        public bool CloseDoor()
        {
            if (Door.localEulerAngles.y == 0) return true;
            
            return false;
        }
        
        public void Entrance()
        {
            EntranceRoom = true;
        }
        
        
    }
}
