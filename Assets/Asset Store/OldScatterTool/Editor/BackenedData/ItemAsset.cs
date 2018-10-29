using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldScatterTool
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scatter Item List", order = 0)]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField] List<ItemModel> _item = new List<ItemModel>();

        internal List<ItemModel> items
        {
            get { return _item; }
            set { _item = value; }
        }

        void Awake()
        {
            if (_item.Count == 0) GenerateDefaultItem();
        }

        private int IDCounter;
        private int minNumChildren = 5;
        private int maxNumChildren = 10;
        private float probabilityOfBeingLeaf = 0.5f;

        private void GenerateDefaultItem()
        {
            var root = new ItemModel("Root", -1, 0);
            _item.Add(root);

            var child = new ItemModel("Element 1", 0, 1);
            _item.Add(child);

        }
    }
}