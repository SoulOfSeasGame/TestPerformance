using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    [System.Serializable]
    public class Quadrant
    {
        public Transform idQuadrant { get; set; }
        public List<Sector> sectors { get; set; }

        public Quadrant() { }

        public Quadrant(Transform id, List<Sector> pos)
        {
            idQuadrant = id;
            sectors = pos;
        }
    }
}