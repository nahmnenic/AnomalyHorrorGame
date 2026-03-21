using System;
using Player;
using Props;
using UnityEngine;

namespace Components
{
    public class LoadObjectGameComponent : MonoBehaviour
    {
        [Header("Object In Game")]
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _sofa;
        [SerializeField] private GameObject _chair;
        [SerializeField] private GameObject _boards;
        
        [Header("Pos Object")]
        [SerializeField] private GameObject _playerPos;
        [SerializeField] private GameObject _sofaPos;
        [SerializeField] private GameObject _chairPos;
        [SerializeField] private GameObject _boardsPos;
        
        [Header("Object To Delete")] 
        [SerializeField] private GameObject _chairDelete;
        [SerializeField] private GameObject _boardsDelete;

        private Inventory _inventory;
        private SofaController _sofaController;

        private void Start()
        {
            _inventory = FindObjectOfType<Inventory>();
            _sofaController = FindObjectOfType<SofaController>();
            UpdateGameObject();
        }

        public void LoadObject()
        {
            DeleteObject();
            SpawnObject();
        }
        
        private void DeleteObject()
        {
            Destroy(_chairDelete);
            Destroy(_boardsDelete);
        }

        private void SpawnObject()
        {
            GameObject chair =  Instantiate(_chair, _chairPos.transform.position, _chairPos.transform.rotation);
            GameObject boards = Instantiate(_boards, _boardsPos.transform.position, _boardsPos.transform.rotation);
          
            _sofa.transform.position = _sofaPos.transform.position;
            
            _player.transform.position = _playerPos.transform.position;
            _player.transform.rotation = _playerPos.transform.rotation;
            UpdateGameObject();
        }

        private void UpdateGameObject()
        {
            _inventory.UpdateProps();
            UpdateCheckDoor();
            MoveSofaReload();
            _sofaController.ReloadRound();
            _boardsDelete = FindObjectOfType<Boards>().gameObject;
            _chairDelete = FindObjectOfType<Chair>().gameObject;
        }

        private void UpdateCheckDoor()
        {
            var poses = FindObjectsOfType(typeof(CheckDoorComponent)) as CheckDoorComponent[];
            foreach (var pos in poses)
            {
                pos._moving = false;
            }
        }

        private void MoveSofaReload()
        {
            var sofa = FindObjectOfType<SofaController>().gameObject;
            var moving = sofa.GetComponentsInChildren<MovingObjectComponent>();
            foreach (var move in moving)
            {
                move._supDispos = false;
            }
        }
    }
}
