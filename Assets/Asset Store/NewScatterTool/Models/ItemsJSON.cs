using System;
using System.Collections.Generic;

namespace NewScatterTool
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
        public float CommonSize;
        public float MaxSize;
        public string CanRotateInX;
        public string CanRotateInY;
        public string CanRotateInZ;
        public string IsAlign;
        public float HeightOffset;
    }
}