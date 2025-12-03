using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RoomMananger
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private Transform[] _roomPos;
        public int CurrentLevel = 1;
        
        [Header("LEVEL2")]
        
        [Header("Rooms Green")]
        [SerializeField] private Room[] RoomsGreenKitchen;
        [SerializeField] private Room[] RoomsGreenStorage;
        [SerializeField] private Room[] RoomsGreenBadRoom;
        [SerializeField] private Room[] RoomsGreenBathRoom;
        [SerializeField] private Room[] RoomsGreenChildren;
        
        [Header("Rooms Red")]
        [SerializeField] private Room[] RoomsRedKitchen;
        [SerializeField] private Room[] RoomsRedStorage;
        [SerializeField] private Room[] RoomsRedBadRoom;
        [SerializeField] private Room[] RoomsRedBathRoom;
        [SerializeField] private Room[] RoomsRedChildren;
        
        [Header("Rooms Black")]
        [SerializeField] private Room[] RoomsBlackKitchen;
        [SerializeField] private Room[] RoomsBlackStorage;
        [SerializeField] private Room[] RoomsBlackBadRoom;
        [SerializeField] private Room[] RoomsBlackBathRoom;
        [SerializeField] private Room[] RoomsBlackChildren;
        
        [Header("Rooms Yellow")]
        [SerializeField] private Room[] RoomsYellowKitchen;
        [SerializeField] private Room[] RoomsYellowStorage;
        [SerializeField] private Room[] RoomsYellowBadRoom;
        [SerializeField] private Room[] RoomsYellowBathRoom;
        [SerializeField] private Room[] RoomsYellowChildren;

        // Динамические списки доступных префабов
        private Dictionary<RoomColor, Dictionary<RoomTarget, List<Room>>> availableRooms;
        
        // Перечисление для цветов
        public enum RoomColor
        {
            Green = 0,
            Red = 1,
            Black = 2,
            Yellow = 3
        }

        // Перечисление для назначений комнат
        public enum RoomTarget
        {
            Kitchen = 0,
            Storage = 1,
            BadRoom = 2,
            BathRoom = 3,
            Children = 4
        }

        private void Start()
        {
            InitializeAvailableRooms();
        }

        private void InitializeAvailableRooms()
        {
            availableRooms = new Dictionary<RoomColor, Dictionary<RoomTarget, List<Room>>>();
            
            // Инициализация для каждого цвета
            InitializeColorRooms(RoomColor.Green, 
                RoomsGreenKitchen, RoomsGreenStorage, RoomsGreenBadRoom, RoomsGreenBathRoom, RoomsGreenChildren);
            InitializeColorRooms(RoomColor.Red, 
                RoomsRedKitchen, RoomsRedStorage, RoomsRedBadRoom, RoomsRedBathRoom, RoomsRedChildren);
            InitializeColorRooms(RoomColor.Black, 
                RoomsBlackKitchen, RoomsBlackStorage, RoomsBlackBadRoom, RoomsBlackBathRoom, RoomsBlackChildren);
            InitializeColorRooms(RoomColor.Yellow, 
                RoomsYellowKitchen, RoomsYellowStorage, RoomsYellowBadRoom, RoomsYellowBathRoom, RoomsYellowChildren);
        }

        private void InitializeColorRooms(RoomColor color, 
            Room[] kitchens, Room[] storages, Room[] badRooms, Room[] bathRooms, Room[] childrens)
        {
            var colorDict = new Dictionary<RoomTarget, List<Room>>();
            
            // Копируем массивы в списки для возможности удаления
            colorDict[RoomTarget.Kitchen] = kitchens != null ? kitchens.ToList() : new List<Room>();
            colorDict[RoomTarget.Storage] = storages != null ? storages.ToList() : new List<Room>();
            colorDict[RoomTarget.BadRoom] = badRooms != null ? badRooms.ToList() : new List<Room>();
            colorDict[RoomTarget.BathRoom] = bathRooms != null ? bathRooms.ToList() : new List<Room>();
            colorDict[RoomTarget.Children] = childrens != null ? childrens.ToList() : new List<Room>();
            
            availableRooms[color] = colorDict;
        }

        public void SwitchRoom()
        {
            ClearExistingRooms();
            
            switch (CurrentLevel)
            {
                case 0:
                    SpawnLevel0();
                    break;
                case 1:
                    SpawnLevel1();
                    break;
                case 2:
                    SpawnLevel2();
                    break;
            }
        }

        private void SpawnLevel1()
        {
            // Создаем списки для рандомизации
            List<RoomColor> colors = new List<RoomColor> 
                { RoomColor.Green, RoomColor.Red, RoomColor.Black, RoomColor.Yellow };
            
            List<RoomTarget> targets = new List<RoomTarget> 
                { RoomTarget.Kitchen, RoomTarget.Storage, RoomTarget.BadRoom, RoomTarget.BathRoom, RoomTarget.Children };
            
            // Перемешиваем списки
            ShuffleList(colors);
            ShuffleList(targets);
            
            // Берем только 4 назначения (по количеству позиций)
            List<RoomTarget> selectedTargets = targets.Take(_roomPos.Length).ToList();
            
            // Спавним комнаты на каждой позиции
            for (int i = 0; i < _roomPos.Length; i++)
            {
                RoomColor color = colors[i];
                RoomTarget target = selectedTargets[i];
                
                SpawnRoomAtPosition(i, color, target);
            }
        }

        private void SpawnRoomAtPosition(int positionIndex, RoomColor color, RoomTarget target)
        {
            if (positionIndex >= _roomPos.Length)
            {
                Debug.LogError("Position index out of range!");
                return;
            }
            
            // Получаем список доступных префабов для данного цвета и назначения
            if (availableRooms.TryGetValue(color, out var colorDict) && 
                colorDict.TryGetValue(target, out var roomList))
            {
                if (roomList.Count == 0)
                {
                    Debug.LogWarning($"No more rooms available for {color} {target}. Resetting pool.");
                    ResetRoomPool(color, target);
                    
                    // Если после сброса все еще пусто, выходим
                    if (roomList.Count == 0)
                    {
                        Debug.LogError($"No rooms found for {color} {target}");
                        return;
                    }
                }
                
                // Выбираем случайный префаб из списка
                int randomIndex = Random.Range(0, roomList.Count);
                Room selectedRoom = roomList[randomIndex];
                
                // Удаляем использованный префаб
                roomList.RemoveAt(randomIndex);
                
                // Создаем комнату
                Room newRoom = Instantiate(selectedRoom, _roomPos[positionIndex].position, 
                    _roomPos[positionIndex].rotation, _roomPos[positionIndex]);
                
                Debug.Log($"Spawned {color} {target} at position {positionIndex}");
            }
            else
            {
                Debug.LogError($"No room configuration found for {color} {target}");
            }
        }

        private void ResetRoomPool(RoomColor color, RoomTarget target)
        {
            // Сброс пула префабов для конкретного сочетания цвет/назначение
            switch (color)
            {
                case RoomColor.Green:
                    switch (target)
                    {
                        case RoomTarget.Kitchen: availableRooms[color][target] = RoomsGreenKitchen.ToList(); break;
                        case RoomTarget.Storage: availableRooms[color][target] = RoomsGreenStorage.ToList(); break;
                        case RoomTarget.BadRoom: availableRooms[color][target] = RoomsGreenBadRoom.ToList(); break;
                        case RoomTarget.BathRoom: availableRooms[color][target] = RoomsGreenBathRoom.ToList(); break;
                        case RoomTarget.Children: availableRooms[color][target] = RoomsGreenChildren.ToList(); break;
                    }
                    break;
                    
                case RoomColor.Red:
                    switch (target)
                    {
                        case RoomTarget.Kitchen: availableRooms[color][target] = RoomsRedKitchen.ToList(); break;
                        case RoomTarget.Storage: availableRooms[color][target] = RoomsRedStorage.ToList(); break;
                        case RoomTarget.BadRoom: availableRooms[color][target] = RoomsRedBadRoom.ToList(); break;
                        case RoomTarget.BathRoom: availableRooms[color][target] = RoomsRedBathRoom.ToList(); break;
                        case RoomTarget.Children: availableRooms[color][target] = RoomsRedChildren.ToList(); break;
                    }
                    break;
                    
                case RoomColor.Black:
                    switch (target)
                    {
                        case RoomTarget.Kitchen: availableRooms[color][target] = RoomsBlackKitchen.ToList(); break;
                        case RoomTarget.Storage: availableRooms[color][target] = RoomsBlackStorage.ToList(); break;
                        case RoomTarget.BadRoom: availableRooms[color][target] = RoomsBlackBadRoom.ToList(); break;
                        case RoomTarget.BathRoom: availableRooms[color][target] = RoomsBlackBathRoom.ToList(); break;
                        case RoomTarget.Children: availableRooms[color][target] = RoomsBlackChildren.ToList(); break;
                    }
                    break;
                    
                case RoomColor.Yellow:
                    switch (target)
                    {
                        case RoomTarget.Kitchen: availableRooms[color][target] = RoomsYellowKitchen.ToList(); break;
                        case RoomTarget.Storage: availableRooms[color][target] = RoomsYellowStorage.ToList(); break;
                        case RoomTarget.BadRoom: availableRooms[color][target] = RoomsYellowBadRoom.ToList(); break;
                        case RoomTarget.BathRoom: availableRooms[color][target] = RoomsYellowBathRoom.ToList(); break;
                        case RoomTarget.Children: availableRooms[color][target] = RoomsYellowChildren.ToList(); break;
                    }
                    break;
            }
        }

        // Метод для полного сброса всех пулов
        public void ResetAllPools()
        {
            InitializeAvailableRooms();
            Debug.Log("All room pools have been reset");
        }

        // Метод для перемешивания списка
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        // Очистка существующих комнат
        private void ClearExistingRooms()
        {
            foreach (Transform roomPos in _roomPos)
            {
                foreach (Transform child in roomPos)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Методы для других уровней
        private void SpawnLevel0()
        {
            Debug.Log("Level 0 spawning logic");
            // Здесь можно задать фиксированную комбинацию для уровня 0
            // Например:
            // SpawnRoomAtPosition(0, RoomColor.Green, RoomTarget.Kitchen);
            // SpawnRoomAtPosition(1, RoomColor.Red, RoomTarget.BathRoom);
            // и т.д.
        }

        private void SpawnLevel2()
        {
            Debug.Log("Level 2 spawning logic");
            // Специфичная логика для уровня 2
            // Можно использовать другой алгоритм распределения
        }

        [ContextMenu("Test Spawn Rooms")]
        public void TestSpawnRooms()
        {
            SwitchRoom();
        }
        
        [ContextMenu("Reset All Pools")]
        public void ContextResetPools()
        {
            ResetAllPools();
        }
    }
}