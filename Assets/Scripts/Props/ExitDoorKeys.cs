using Interact;
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
                    key.SetActive(false);
                    key.GetComponentInParent<InteractableComponent>().enabled = false;
                    break;
                }
            }
        }
    }
}
