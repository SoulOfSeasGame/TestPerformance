using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    [System.Serializable]
    public class QuadrantsJSON
    {
        public List<QuadrantJSON> scene;
    }

    [System.Serializable]
    public class QuadrantJSON
    {
        public string idQuadrant;
        public List<string> sectors;
    }
}