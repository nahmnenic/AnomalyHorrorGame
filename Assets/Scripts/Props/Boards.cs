using Player;
using UnityEngine;

namespace Props
{
    public class Boards : MonoBehaviour
    {
        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
        }

        public void AddBoard()
        {
            _inventory.AddBoard();
        }
    }
}
