using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    [System.Serializable]
    public class Sector
    {
        public string idSector;// { get; set; }
        public Vector3 position;// { get; set; }

        public Sector() { }

        public Sector(string id, Vector3 pos)
        {
            idSector = id;
            position = pos;
        }
    }
}