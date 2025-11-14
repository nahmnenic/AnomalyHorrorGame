using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //private GameSession _session;

    [Header("Interaction")] 
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private GameObject _midBody;
    private Collider[] _interactionResult = new Collider[1];

    private void Start()
    {
        //_session = FindObjectOfType<GameSession>();
        //_session.Data.Inventory.OnChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(string id, int value)
    {
            
    }

    public void AddInInventory(string id, int value)
    {
        //_session.Data.Inventory.Add(id, value);
    }

    public void Interact()
    {
        var size = Physics.OverlapSphereNonAlloc(
            _midBody.transform.position,
            _interactionRadius,
            _interactionResult,
            _interactionLayer);

        for (int i = 0; i < size; i++)
        {
            var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
            if (interactable != null)
                interactable.Interact();    
        }
    }
}
