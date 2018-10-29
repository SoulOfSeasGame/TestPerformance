using System.Collections.Generic;
using UnityEngine;
using System;

namespace OldScatterTool
{
    [Serializable]
    public enum TypeOfItem { Rock = 0, Coral = 10, StarFish = 20 }

    [Serializable]
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

    [Serializable]
    internal class ItemModel : TreeElement
    {
        public GameObject item;
        public TypeOfItem classification;
        public bool isUsed;
        public string idName;
        public string genus;
        public string specie;
        public float commonSize;
        public float maxSize;
        public bool canRotateInX;
        public bool canRotateInY;
        public bool canRotateInZ;
        public bool isAlign;
        public float heightOffset;
        public bool useCustomParams;
        public ScatterParams scatterParams;
        public List<Material> materials;

        public ItemModel(string name, int depth, int id) : base(name, depth, id)
        {
        }
    }
}