using UnityEngine;

namespace RoomMananger
{
    public class SpawnerRoom : MonoBehaviour
    {
        [SerializeField] private GameObject[] _roomsPrefabs;

        private GameObject _currentRoom = null;
        
        public void SpawnRoom(int roomNumber)
        {
            if(_currentRoom != null) Destroy(_currentRoom);
            
            _currentRoom = Instantiate(_roomsPrefabs[roomNumber], transform.position, transform.rotation);
        }
    }
}
