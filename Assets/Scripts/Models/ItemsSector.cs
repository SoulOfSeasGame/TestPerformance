using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    [System.Serializable]
    public class ItemsSector
    {
        public List<ItemSector> items;
    }

    [System.Serializable]
    public class ItemSector
    {
        public string idName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}

