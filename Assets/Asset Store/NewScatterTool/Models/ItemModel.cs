using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace NewScatterTool
{
    [Serializable]
    public class ItemProperties
    {
        public Vector2 size;
        public bool canRotateInX;
        public Vector2 rotationX;
        public bool canRotateInY;
        public Vector2 rotationY;
        public bool canRotateInZ;
        public Vector2 rotationZ;
        public bool isAlign;
        public float heightOffset;
    }

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

    [CreateAssetMenu(fileName = "NewScatterItem", menuName = "Scatter Item", order = 0)]
    [Serializable]
    public class ItemModel : ScriptableObject
    {
        public GameObject item;
        public Texture2D picture;
        public string classification;
        public bool isUsed;
        public string idName;
        public string genus;
        public string specie;
        public string sexStageVar;
        public List<Material> materials;

        public ItemProperties props;
        public ScatterParams sParams;

        public void GetInfoFromJSON()
        {
            TextAsset sectionJSON = Resources.Load("Json/ScatterItems/Items") as TextAsset;
            string str = string.Format("{{\"items\":{0}}}", sectionJSON.text);
            ItemsJSON itemsJSON = JsonUtility.FromJson<ItemsJSON>(str);

            ItemJSON itemJSON = itemsJSON.items.Find(i => i.IdItem == item.name);

            if (itemJSON != null)
            {
                idName = itemJSON.IdItem;
                genus = itemJSON.Genus;
                specie = itemJSON.Specie;
                sexStageVar = itemJSON.StageSexVar;
                classification = GetPathFromTypeOfObject(itemJSON.TypeOf);

                props.canRotateInX = StringToBool(itemJSON.CanRotateInX);
                props.rotationX = new Vector2(0, 360);
                props.canRotateInY = StringToBool(itemJSON.CanRotateInY);
                props.rotationY = new Vector2(0, 360);
                props.canRotateInZ = StringToBool(itemJSON.CanRotateInZ);
                props.rotationZ = new Vector2(0, 360);
                props.size = new Vector2(CentimetersToMeters(itemJSON.CommonSize), CentimetersToMeters(itemJSON.MaxSize));
                props.isAlign = StringToBool(itemJSON.IsAlign);
                props.heightOffset = itemJSON.HeightOffset;
            }
        }

        public string GetPathFromTypeOfObject(string typeOf)
        {
            switch (typeOf)
            {
                case "Coral": return "Items/Cnidarians/Corals/";
            }

            return string.Empty;
        }

        public float CentimetersToMeters(float value)
        {
            const float CENTIMETERS_TO_METERS = 0.01f;
            return value * CENTIMETERS_TO_METERS;
        }

        public bool StringToBool(string value)
        {
            return value == "TRUE" ? true : false;
        }
    }
}