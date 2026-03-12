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
            if (gameObject.GetComponent<Baricade>().Bariccade) return;
            if (_inventory.Boards)
            {
                BoardPrefab.SetActive(true);
                _doorHandle1.enabled = false;
                _doorHandle2.enabled = false;
                DropBoard();
                gameObject.GetComponent<Baricade>().enabled = false;
                gameObject.GetComponent<InteractableComponent>().enabled = false;
            }
            else
            {
                ChairPrefab.SetActive(true);
                _doorHandle1.enabled = false;
                _doorHandle2.enabled = false;
                DropChair();
                gameObject.GetComponent<Baricade>().enabled = false;
                gameObject.GetComponent<InteractableComponent>().enabled = false;
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
