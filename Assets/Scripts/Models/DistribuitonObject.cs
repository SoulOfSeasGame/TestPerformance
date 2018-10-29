using UnityEngine;
using System;

[Serializable]
public class DistribuitonObject
{
    public GameObject prefab;
    public Vector2 scaleOffset = new Vector2(0.8f, 1.2f);
    public float percent;
    public bool align = false;

}