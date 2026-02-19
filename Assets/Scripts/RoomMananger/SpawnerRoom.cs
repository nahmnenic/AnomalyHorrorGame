using UnityEngine;

namespace RoomMananger
{
    public class SpawnerRoom : MonoBehaviour
    {
        [SerializeField] private GameObject[] _roomsPrefabs;

        public void SpawnRoom(int roomNumber)
        {
            foreach (GameObject room in _roomsPrefabs)
            {
                if (room.activeSelf) room.SetActive(false);
            }
            
            
            _roomsPrefabs[roomNumber].SetActive(true);
            _roomsPrefabs[roomNumber].transform.position = transform.position;
            _roomsPrefabs[roomNumber].transform.rotation = transform.rotation;
        }
    }
}
