using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScatterTool
{
    [Serializable]
    public class ItemsJSON
    {
        public List<ItemJSON> items;
    }

    [Serializable]
    public class ItemJSON
    {
        public string IdItem;
        public string Genus;
        public string Specie;
        public string StageSexVar;
        public string TypeOf;
        public float Percentage;
        public float CommonSize;
        public float MaxSize;
        public string CanRotateInX;
        public string CanRotateInY;
        public string CanRotateInZ;
        public string IsAlign;
        public float HeightOffset;
    }

    [Serializable]
    public class Item
    {
        public bool isUsed;
        public string name;
        public GameObject item;
        public string genus;
        public string specie;
        public string stageSexVar;
        public string path;
        public float commonSize;
        public float maxSize;
        public bool canRotateInX;
        public bool canRotateInY;
        public bool canRotateInZ;
        public bool isAlign;
        public float heightOffset;
        public bool useCustomParams;
        public ScatterParams scatterParams;
    }

    public class ScatterParams
    {
        public bool isOpen;
        public int percentage;
        public float scaleX;
        public float scaleY;
        public float offsetX;
        public float offsetY;
        public AnimationCurve curve;
        public Texture2D noiseMap;
    }

    public class Surface
    {
        public bool isUsed;
        public GameObject surface;
    }
}