using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class Section
    {
        public GameObject IdSection { get; set; }
        public List<Renderer> Items { get; set; }

        public Section() { }

        public Section(GameObject idSection, List<Renderer> items)
        {
            this.IdSection = idSection;
            this.Items = items;
        }
    }
}