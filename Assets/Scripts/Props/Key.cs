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
        
        [SerializeField] private SoundController _setKeySound;
        
        public bool ExampleKey;
        
        private KeyController _keyController;
        
        private void Start()
        {
            _keyController = FindObjectOfType<KeyController>();
        }

        public void TakeKay()
        {
            if (_keyController.HaveKeyInInventory())
            {
                Debug.Log("У вас уже есть ключ");
                return;
            }

            /*if (ExampleKey)
            {
                _keyController.AddExampleKey();
                Destroy(_currentKey);
                return;
            }*/
            
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
            _setKeySound.PlaySound();
        }
        
        public void UnSetKey()
        {
            gameObject.SetActive(false);
            GetComponentInParent<InteractableComponent>().enabled = false;
        }
    }
}
