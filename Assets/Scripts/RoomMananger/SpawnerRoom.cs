using UnityEngine;

namespace RoomMananger
{
    public class SpawnerRoom : MonoBehaviour
    {
        [Header("0 Bedroom")]
        [SerializeField] private GameObject _bedroom;

        [Header("1 Kitchen")]
        [SerializeField] private GameObject _kitchen;

        [Header("2 KidsRoom")]
        [SerializeField] private GameObject _children;

        [Header("3 Storage")]
        [SerializeField] private GameObject _storage;

        [Header("4 Bathroom")]
        [SerializeField] private GameObject _bathroom;

        private GameObject _currentRoom;

        public void SpawnRoom(int roomType)
        {
            if (_currentRoom != null)
            {
                Destroy(_currentRoom);
            }

            GameObject prefab = GetRoomPrefab(roomType);

            _currentRoom = Instantiate(prefab, transform);

            _currentRoom.transform.localPosition = Vector3.zero;
            _currentRoom.transform.localRotation = Quaternion.identity;
        }

        private GameObject GetRoomPrefab(int index)
        {
            switch (index)
            {
                case 0:
                    return _bedroom;

                case 1:
                    return _kitchen;

                case 2:
                    return _children;

                case 3:
                    return _storage;

                case 4:
                    return _bathroom;
            }

            return null;
        }
    }
}
