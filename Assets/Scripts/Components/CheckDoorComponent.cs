using System;
using Interact;
using Player;
using UI;
using UnityEngine;

namespace Components
{
    public class CheckDoorComponent : MonoBehaviour
    {
        [SerializeField] private Transform _interactionPoint;

        private bool _moving = false;
        private Inventory _inventory;
        private GameUI _gameUI;
        
        [Header("Object to hide")]
        [SerializeField] private GameObject Pos;
        
        private Collider[] _interactionResult = new Collider[32];

        private void Awake()
        {
            _inventory = FindObjectOfType<Inventory>();
            _gameUI =  FindObjectOfType<GameUI>();
        }

        public void CheckDoor()
        {
            _inventory.CheckRooms();
            if(!_inventory.CheckRooms()) return;
            int count = Physics.OverlapSphereNonAlloc(_interactionPoint.position, 5, _interactionResult);

            for (int i = 0; i < count; i++)
            {
                Collider col = _interactionResult[i];
                Baricade baricade = col.GetComponent<Baricade>();
                if (col == null) continue;
                if(baricade == null) continue;
                if (baricade.Bariccade)
                {
                    DeleteChance();
                    return;
                }

                if(_moving)
                {
                    baricade.Bariccade = true;
                    baricade.DisableBaricade();
                }
                
            }
        }

        public void MoveSofa()
        {
            _moving = true;
            CheckDoor();
        }

        private void DeleteChance()
        {
            Pos.SetActive(false);
        }

        public void ColorDoor()
        {
            int count = Physics.OverlapSphereNonAlloc(_interactionPoint.position, 5, _interactionResult);

            for (int i = 0; i < count; i++)
            {
                Collider col = _interactionResult[i];
                Baricade baricade = col.GetComponent<Baricade>();
                if (col == null) continue;
                if(baricade == null) continue;
                if (baricade.color == Baricade.ColorOption.Black) _gameUI.SofaTrue = true;
            }
        }
    }
}
