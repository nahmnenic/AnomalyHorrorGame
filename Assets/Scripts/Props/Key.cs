using System;
using Interact;
using Player;
using UnityEngine;

namespace Props
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private GameObject _roomName;
        [SerializeField] private GameObject _currentKey;
        
        private KeyController _keyController;
        
        private void Start()
        {
            _keyController = FindObjectOfType<KeyController>();
        }

        public void TakeKay()
        {
            if(_keyController.HaveKeyInInventory()) return;
            _keyController.ChangeKeyState(_roomName.gameObject);
            Destroy(_currentKey);
        }

        public void SetKey()
        {
            if (!_keyController.HaveKeyInInventory())
            {
                Debug.Log("Сначала возьмите ключ");
                return;
            }
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            GetComponentInParent<InteractableComponent>().enabled = false;
            _keyController.SetKey();
        }
        
        public void UnSetKey()
        {
            gameObject.SetActive(false);
            GetComponentInParent<InteractableComponent>().enabled = false;
        }
    }
}
