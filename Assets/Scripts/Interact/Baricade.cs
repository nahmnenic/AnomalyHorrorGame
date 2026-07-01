using Components;
using Player;
using UI;
using UnityEngine;

namespace Interact
{
    public class Baricade : MonoBehaviour
    {
        [SerializeField] private GameObject ChairPrefab;
        [SerializeField] private GameObject BoardPrefab;
        [SerializeField] private InteractableComponent _doorHandle1;
        [SerializeField] private InteractableComponent _doorHandle2;

        [SerializeField] private FMODUnity.StudioEventEmitter _chairSound;
        [SerializeField] private FMODUnity.StudioEventEmitter _boardSound;
        
        private Baricade _baricade;
        private InteractableComponent _interactableComponent;
        private GameUI _gameUI;
        
        public bool Bariccade;
        
        private Inventory _inventory;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
            _baricade = GetComponent<Baricade>();
            _gameUI = FindObjectOfType<GameUI>();
            _interactableComponent = GetComponent<InteractableComponent>();
        }

        public void Barricade()
        {
            if (_baricade.Bariccade) return;
            CloseDoor();
            if (_inventory.Boards)
            {
                BoardPrefab.SetActive(true);
                _gameUI.Board = true;
                if(color == ColorOption.Red) _gameUI.BoardTrue = true;
                else _gameUI.BoardTrue = false;
                DropBoard();
                DisableBaricade();
            }
            else
            {
                ChairPrefab.SetActive(true);
                _gameUI.Chair = true;
                if(color == ColorOption.Yellow) _gameUI.ChairTrue = true;
                else _gameUI.ChairTrue = false;
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

        private void CloseDoor()
        {
            if(_doorHandle1.On) _doorHandle1.Interact();
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
            _boardSound.Play();
            _inventory.DeleteBoards();
        }
        
        private void DropChair()
        {
            _chairSound.Play();
            _inventory.DeleteChair();
        }
        
        public enum ColorOption
        {
            Green,
            Red,
            Black,
            Yellow
        }
        
        public ColorOption color;
    }
}
