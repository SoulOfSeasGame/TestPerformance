using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewScatterTool
{
    [CreateAssetMenu(fileName = "ScatterList", menuName = "Create Scatter List", order = 0)]
    public class ScatterList : ScriptableObject
    {
        [SerializeField] List<ItemModel> _item = new List<ItemModel>();

        public List<ItemModel> items
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}