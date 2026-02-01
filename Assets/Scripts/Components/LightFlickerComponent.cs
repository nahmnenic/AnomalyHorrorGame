using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Components
{
    public class LightFlickerComponent : MonoBehaviour
    {
        private Light _light;
        [SerializeField] private float _minLightIntensity;
        [SerializeField] private float _maxLightIntensity;
        [SerializeField] private float _speed;

        private void Start()
        {
            _light = GetComponent<Light>();
            
            InvokeRepeating("Flicker", 0, _speed);
        }

        private void Flicker()
        {
            float randomIntensity = Random.Range(_minLightIntensity, _maxLightIntensity);
            _light.intensity = randomIntensity;
        }
    }
}
