using FMODUnity;
using UnityEngine;

namespace Sound
{
    public class Occlusion : MonoBehaviour
    {
        [Header("Player")]
        public Transform PlayerHead;

        [Header("Rays")]
        [SerializeField] private int _rayCount = 9;
        [SerializeField] private float _rayWidth = 1f;
        
        [Header("Data")]
        [SerializeField] private SurfaceMaterialDatabase database;

        private StudioEventEmitter _emitter;
        
        [HideInInspector] public float CurrentOcclusion;
        
        
        private void Start()
        {
            _emitter = GetComponent<StudioEventEmitter>();
        }

        private void Update()
        {
            float occlusion = CalculateOcclusion();
            CurrentOcclusion = occlusion;
            _emitter.EventInstance.setParameterByName("Occlusion", occlusion);
        }

        private float DrawOcclusionRays()
        {
            if (PlayerHead == null) return 0;

            float totalOcclusion = 0;

            Vector3 center = PlayerHead.position;
            Vector3 directionToHead = (center - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, directionToHead).normalized;


            for (int i = 0; i < _rayCount; i++)
            {
                float t = (float)i / (_rayCount - 1);
                float offset = Mathf.Lerp(-_rayWidth * 0.5f, _rayWidth * 0.5f, t);

                Vector3 target = center + right * offset;
                totalOcclusion += ShootRay(target);
            }


            return totalOcclusion / _rayCount;
        }

        private float ShootRay(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            float distance = direction.magnitude;

            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance))
            {
                int surface = hit.collider.gameObject.layer;
                float occlusion = GetOcclusion(surface);
                
                if (occlusion == 0)
                {
                    Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.red);
                }
                
                return occlusion;
            }


            Debug.DrawRay(transform.position, direction, Color.green);

            return 0;
        }
        
        private float GetOcclusion(int layer)
        {
            foreach(var material in database.materials)
            {
                if(material.layer == (1 << layer))
                    return material.occlusionValue;
            }

            return 0;
        }

        private float CalculateOcclusion()
        {
            return DrawOcclusionRays();
        }
    }
}
