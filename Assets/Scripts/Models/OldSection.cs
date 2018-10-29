using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class OldSection
    {
        public GameObject IdSection { get; set; }
        public ItemsSector Items { get; set; }

        public OldSection() { }

        public OldSection(GameObject IdSection, ItemsSector items)
        {
            this.IdSection = IdSection;
            this.Items = items;
        }
    }
}