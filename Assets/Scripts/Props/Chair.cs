using System;
using Player;
using UnityEngine;

namespace Props
{
    public class Chair : MonoBehaviour
    {
        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
        }

        public void AddChair()
        {
            _inventory.AddChair();
        }
    }
}
