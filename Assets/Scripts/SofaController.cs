using System;
using Components;
using Player;
using UnityEngine;

public class SofaController : MonoBehaviour
{
    [Header("Sofa Pos Interact")]
    [SerializeField] private GameObject _pos1;
    [SerializeField] private GameObject _pos2;
    [SerializeField] private GameObject _pos3;
    [SerializeField] private GameObject _pos4;
    
    [Header("Sofa Pos To Move")]
    [SerializeField] private GameObject _pos11;
    [SerializeField] private GameObject _pos22;
    [SerializeField] private GameObject _pos33;
    [SerializeField] private GameObject _pos44;

    private Inventory _inventory;

    private void Start()
    {
        _inventory = FindObjectOfType<Inventory>();
    }

    public void MoveSofa(int number)
    {
        if (!_inventory.CheckRooms()) return;
        switch (number)
        {
            case 1:
                _pos1.GetComponent<MovingObjectComponent>().MoveObject();
                _pos11.GetComponent<CheckDoorComponent>().MoveSofa();
                break;
            case 2:
                _pos2.GetComponent<MovingObjectComponent>().MoveObject();
                _pos22.GetComponent<CheckDoorComponent>().MoveSofa();
                break;
            case 3:
                _pos3.GetComponent<MovingObjectComponent>().MoveObject();
                _pos33.GetComponent<CheckDoorComponent>().MoveSofa();
                break;
            case 4:
                _pos4.GetComponent<MovingObjectComponent>().MoveObject();
                _pos44.GetComponent<CheckDoorComponent>().MoveSofa();
                break;
        }
        
        _pos1.SetActive(false);
        _pos2.SetActive(false);
        _pos3.SetActive(false);
        _pos4.SetActive(false);
    }
}
