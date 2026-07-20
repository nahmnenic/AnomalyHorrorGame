using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceMaterialDatabase", menuName = "Game/Surface Material Database")]
public class SurfaceMaterialDatabase : ScriptableObject
{
    public List<SurfaceMaterial> materials = new();
}


[Serializable]
public class SurfaceMaterial
{
    public string materialName;

    public LayerMask layer;

    [Range(0,1)]
    public float occlusionValue;
}