using System;
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
        

        public bool Bariccade;
        
        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
        }

        public void Barricade()
        {
            Debug.Log("Barricade");
            if (_inventory.Boards)
            {
                BoardPrefab.SetActive(true);
                _doorHandle1.enabled = false;
                _doorHandle2.enabled = false;
                DropBoard();
            }
            else
            {
                ChairPrefab.SetActive(true);
                _doorHandle1.enabled = false;
                _doorHandle2.enabled = false;
                DropChair();
            }

            Bariccade = true;
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
