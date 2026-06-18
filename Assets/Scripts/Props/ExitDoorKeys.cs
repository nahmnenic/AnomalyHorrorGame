using Components;
using Interact;
using UI;
using UnityEngine;

namespace Props
{
    public class ExitDoorKeys : MonoBehaviour
    {
        [SerializeField] private GameObject[] _keys;

        public void DeleteRandomKey()
        {
            foreach (var key in _keys)
            {
                if (key.GetComponent<MeshRenderer>().enabled)
                {
                    key.GetComponent<MeshRenderer>().enabled = false;
                    key.GetComponentInParent<InteractableComponent>().enabled = true;
                    break;
                }
            }
        }

        public void EndGame()
        {
            int c = 0;
            foreach (var key in _keys)
            {
                if (key.GetComponent<MeshRenderer>().enabled)
                {
                    c++;
                }
            }

            if (c == 4)
            {
                FindFirstObjectByType<GameUI>().WinGame();
                GetComponent<RotateObjectComponent>().Rotate();
            }
        }

        public void DebugAllKeys()
        {
            int c = 0;
            foreach (var key in _keys)
            {
                if (key.GetComponent<MeshRenderer>().enabled)
                {
                    c++;
                }
            }
            Debug.Log($"ВСЕГО КЛЮЧЕЙ В ДВЕРИ: {c}");
        }
    }
}
