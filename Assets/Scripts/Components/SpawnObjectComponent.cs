using System.Collections;
using UnityEngine;

namespace Components
{
    public class SpawnObjectComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _spawnObject;
        [SerializeField] private Transform _spawnPos;
        [SerializeField] private GameObject _objToDelete;

        [SerializeField] private float _delay;
        
        
        [ContextMenu("SpawnObject")]
        public void SpawnObject()
        {
            StartCoroutine(SpawnObjectCoroutine());
        }

        private IEnumerator SpawnObjectCoroutine()
        {
            yield return new WaitForSeconds(_delay);
            Instantiate(_spawnObject, _spawnPos.position, _spawnPos.rotation);
            if(_objToDelete!=null) Destroy(_objToDelete);
            yield return null;
        }
    }
}
