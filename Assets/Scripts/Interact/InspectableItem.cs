using UnityEngine;

namespace Interact
{
    public class InspectableItem : InteractableComponent
    {
        [Header("Inspect")]
        [SerializeField] private float _rotationSpeed = 5f;
        public string ItemName;

        public float RotationSpeed => _rotationSpeed;

        public override void Interact()
        {
            InspectManager inspectManager = FindFirstObjectByType<InspectManager>();

            if (inspectManager == null)
            {
                Debug.LogError("InspectManager not found!");
                return;
            }

            inspectManager.StartInspect(this);
        }
    }
}