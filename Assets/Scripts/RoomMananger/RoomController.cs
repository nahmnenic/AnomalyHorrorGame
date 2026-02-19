using System.Collections.Generic;
using UnityEngine;

namespace RoomMananger
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private Transform[] _roomPos;
        
        [SerializeField] private SpawnerRoom _blackSpawner;
        [SerializeField] private SpawnerRoom _greenSpawner;
        [SerializeField] private SpawnerRoom _yellowSpawner;
        [SerializeField] private SpawnerRoom _redSpawner;

        private const int RoomsCount = 5;
        
        public void SwitchRoom()
        {
            Transform[] shuffledPositions = (Transform[])_roomPos.Clone();
            
            for (int i = 0; i < shuffledPositions.Length; i++)
            {
                int randomIndex = Random.Range(i, shuffledPositions.Length);
                (shuffledPositions[i], shuffledPositions[randomIndex]) =
                    (shuffledPositions[randomIndex], shuffledPositions[i]);
            }
            
            _blackSpawner.transform.position  = shuffledPositions[0].position;
            _blackSpawner.transform.rotation  = shuffledPositions[0].rotation;
            
            _greenSpawner.transform.position  = shuffledPositions[1].position;
            _greenSpawner.transform.rotation  = shuffledPositions[1].rotation;
            
            _yellowSpawner.transform.position = shuffledPositions[2].position;
            _yellowSpawner.transform.rotation = shuffledPositions[2].rotation;
            
            _redSpawner.transform.position    = shuffledPositions[3].position;
            _redSpawner.transform.rotation    = shuffledPositions[3].rotation;
            
            Spawn();
        }

        private void Spawn()
        {
            List<int> roomNumbers = new List<int>();
            
            for (int i = 0; i < RoomsCount; i++)
                roomNumbers.Add(i);
            
            for (int i = 0; i < roomNumbers.Count; i++)
            {
                int rnd = Random.Range(i, roomNumbers.Count);
                (roomNumbers[i], roomNumbers[rnd]) = (roomNumbers[rnd], roomNumbers[i]);
            }
            
            _blackSpawner.SpawnRoom(roomNumbers[0]);
            _greenSpawner.SpawnRoom(roomNumbers[1]);
            _yellowSpawner.SpawnRoom(roomNumbers[2]);
            _redSpawner.SpawnRoom(roomNumbers[3]);
        }
        
    }
}