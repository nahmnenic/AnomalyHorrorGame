using System;
using Interact;
using Props;
using RoomMananger;
using UnityEngine;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public GameObject ChairPrefab;
        public GameObject BoardPrefab;

        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private PlayerInteractionDoor _doorInteraction;

        [Header("Cheats")] public bool Skip;
        
        public bool Chair;
        public bool Boards;
        public bool Key;


        private void Start()
        {
            UpdateProps();
        }

        private void ChangeMode()
        {
            if (Chair || Boards)
            {
                _playerInteraction.enabled = false;
                _playerInteraction.Hide();
                _doorInteraction.enabled = true;
            }
            else
            {
                _doorInteraction.Hide();
                _playerInteraction.enabled = true;
                _doorInteraction.enabled = false;
            }
        }
        
        public void AddChair()
        {
            if(Boards) return;
            Chair = true;
            ChairPrefab.SetActive(false);
            ChangeMode();
        }

        public void DeleteChair()
        {
            Chair = false;
            ChangeMode();
        }

        public void AddBoard()
        {
            if(Chair) return;
            Boards = true;
            BoardPrefab.SetActive(false);
            ChangeMode();
        }

        public void DeleteBoards()
        {
            Boards = false;
            ChangeMode();
        }

        public void UpdateProps()
        {
            BoardPrefab = FindObjectOfType<Boards>().gameObject;
            ChairPrefab = FindObjectOfType<Chair>().gameObject;
        }
    }
}
