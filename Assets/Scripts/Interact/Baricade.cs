using System;
using Components;
using Player;
using UnityEngine;

namespace Interact
{
    public class Baricade : MonoBehaviour
    {
        [SerializeField] private GameObject ChairPrefab;
        [SerializeField] private GameObject BoardPrefab;
        [SerializeField] private InteractableComponent _doorHandle1;
        [SerializeField] private InteractableComponent _doorHandle2;
        
        private Baricade _baricade;
        private InteractableComponent _interactableComponent;
        
        public bool Bariccade;
        
        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
            _baricade = GetComponent<Baricade>();
            _interactableComponent =  GetComponent<InteractableComponent>();
        }

        public void Barricade()
        {
            if (_baricade.Bariccade) return;
            if (_inventory.Boards)
            {
                BoardPrefab.SetActive(true);
                DropBoard();
                DisableBaricade();
            }
            else
            {
                ChairPrefab.SetActive(true);
                DropChair();
                DisableBaricade();
            }
            Bariccade = true;
            CheckDoorComponent[] PosSofa = FindObjectsOfType<CheckDoorComponent>();
            foreach (CheckDoorComponent CheckDoor in PosSofa)
            {
                CheckDoor.CheckDoor();
            }
        }

        public void DisableBaricade()
        {
            _doorHandle1.enabled = false;
            _doorHandle2.enabled = false;
            _baricade.enabled = false;
            _interactableComponent.enabled = false;
        }
        
        private void DropBoard()
        {
            _inventory.DeleteBoards();
        }
        
        private void DropChair()
        {
            _inventory.DeleteChair();
        }
    }
}
