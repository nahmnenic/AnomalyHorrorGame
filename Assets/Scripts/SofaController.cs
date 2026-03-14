using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SofaController : MonoBehaviour
{
    [SerializeField] private GameObject _pos1;
    [SerializeField] private GameObject _pos2;
    [SerializeField] private GameObject _pos3;
    [SerializeField] private GameObject _pos4;

    public void MoveSofa()
    {
        _pos1.SetActive(false);
        _pos2.SetActive(false);
        _pos3.SetActive(false);
        _pos4.SetActive(false);
        
    }
}
