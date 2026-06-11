using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoomMananger
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private Transform[] _roomPos;
        
        [SerializeField] private SpawnerRoom _blackSpawner;
        [SerializeField] private SpawnerRoom _greenSpawner;
        [SerializeField] private SpawnerRoom _yellowSpawner;
        [SerializeField] private SpawnerRoom _redSpawner;

        private List<int> _blackRooms;
        private List<int> _greenRooms;
        private List<int> _yellowRooms;
        private List<int> _redRooms;

        private int _currentCycle;

        private void Start()
        {
            GenerateAllOrders();

            SwitchRoom();
        }

        public void SwitchRoom()
        {
            if (_currentCycle >= 4)
            {
                _currentCycle = 0;
                GenerateAllOrders();
            }

            // =========================
            // РАНДОМ ПОЗИЦИЙ
            // =========================

            Transform[] shuffledPositions = (Transform[])_roomPos.Clone();

            for (int i = 0; i < shuffledPositions.Length; i++)
            {
                int rnd = Random.Range(i, shuffledPositions.Length);

                (shuffledPositions[i], shuffledPositions[rnd]) = (shuffledPositions[rnd], shuffledPositions[i]);
            }

            // =========================
            // ДВИГАЕМ СПАВНЕРЫ
            // =========================

            SetSpawnerTransform(_blackSpawner, shuffledPositions[0]);

            SetSpawnerTransform(_greenSpawner, shuffledPositions[1]);

            SetSpawnerTransform(_yellowSpawner, shuffledPositions[2]);

            SetSpawnerTransform(_redSpawner, shuffledPositions[3]);

            // =========================
            // СПАВНИМ КОМНАТЫ
            // =========================

            _blackSpawner.SpawnRoom(_blackRooms[_currentCycle]);

            _greenSpawner.SpawnRoom(_greenRooms[_currentCycle]);

            _yellowSpawner.SpawnRoom(_yellowRooms[_currentCycle]);

            _redSpawner.SpawnRoom(_redRooms[_currentCycle]);

            _currentCycle++;
        }

        private void SetSpawnerTransform(SpawnerRoom spawner, Transform point)
        {
            spawner.transform.position = point.position;
            spawner.transform.rotation = point.rotation;
        }

        private void GenerateAllOrders()
        {
            bool valid = false;

            while (!valid)
            {
                _blackRooms = GenerateRandomOrder();
                _greenRooms = GenerateRandomOrder();
                _yellowRooms = GenerateRandomOrder();
                _redRooms = GenerateRandomOrder();

                valid = true;

                // Проверяем первые 4 цикла
                for (int i = 0; i < 4; i++)
                {
                    HashSet<int> roomsInCycle = new HashSet<int>()
                    {
                        _blackRooms[i],
                        _greenRooms[i],
                        _yellowRooms[i],
                        _redRooms[i]
                    };

                    if (roomsInCycle.Count != 4)
                    {
                        valid = false;
                        break;
                    }
                }
            }
        }

        private List<int> GenerateRandomOrder()
        {
            List<int> rooms = new List<int>() { 0, 1, 2, 3, 4 };
            
            for (int i = 0; i < rooms.Count; i++)
            {
                int rnd = Random.Range(i, rooms.Count);

                (rooms[i], rooms[rnd]) = (rooms[rnd], rooms[i]);
            }

            return rooms;
        }
        
    }
}