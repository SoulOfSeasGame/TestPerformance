using System.Collections.Generic;
using System.Linq;
using TestPerformance;
using UnityEngine;

namespace ScatterTool
{
    [RequireComponent(typeof(STImplementation))]
    [RequireComponent(typeof(STParameters))]
    public class ScatterTool : MonoBehaviour
    {
        private const string CONSOLE_DETAILS = "Some errors found! See the console for more details.";
        public static int SIZEMAP = 312;

        public int iterations;
        public int numberOfElements;
        public float globalScale;
        public Material material;

        public bool itemsAreLoaded = false;
        public List<Item> items = new List<Item>();
        public List<ScatterParams> itemsParams = new List<ScatterParams>();
        public List<Surface> targetSurfaces = new List<Surface>();
        private ItemsJSON itemsJSON = new ItemsJSON();

        public void LoadItems()
        {
            ///
            material = Resources.Load("Rock") as Material;
            ///

            LoadItemsAndProperties();
            LoadScatterParameters();
        }

        public void LoadItemsAndProperties()
        {
            itemsAreLoaded = true;

            TextAsset sectionJSON = Resources.Load("Json/Items/Cnidarians") as TextAsset;
            string str = string.Format("{{\"items\":{0}}}", sectionJSON.text);
            itemsJSON = JsonUtility.FromJson<ItemsJSON>(str);

            foreach (var item in itemsJSON.items)
            {
                Item it = new Item();
                it.isUsed = true;
                it.name = item.IdItem;
                it.genus = item.Genus;
                it.specie = item.Specie;
                it.stageSexVar = item.StageSexVar;
                it.path = GetPathFromTypeOfObject(item.TypeOf);
                it.commonSize = CentimetersToMeters(item.CommonSize);
                it.maxSize = CentimetersToMeters(item.MaxSize);
                it.canRotateInX = StringToBool(item.CanRotateInX);
                it.canRotateInY = StringToBool(item.CanRotateInY);
                it.canRotateInZ = StringToBool(item.CanRotateInZ);
                it.isAlign = StringToBool(item.IsAlign);
                it.heightOffset = CentimetersToMeters(item.HeightOffset);
                it.useCustomParams = false;
                it.item = Resources.Load<GameObject>(string.Format("{0}{1}", it.path, item.IdItem));
                ScatterParams sp = new ScatterParams();
                it.scatterParams = sp;
                it.scatterParams.isOpen = false;
                items.Add(it);
            }
        }

        public void LoadScatterParameters()
        {
            //PlayerPrefs.DeleteAll();
            for (int i = 0; i < items.Count; i++)
                items[i].scatterParams = GetComponent<STParameters>().InitItemParameters(items[i]);
        }

        public void AddElements(Transform[] list)
        {
            foreach (var item in list)
            {
                Surface surface = new Surface();
                surface.isUsed = true;
                surface.surface = item.gameObject;

                if (CheckSurface(surface.surface, item.gameObject))
                    targetSurfaces.Add(surface);
            }
        }

        private bool CheckSurface(GameObject surface, GameObject item)
        {
            bool error = true;

            if (item.GetComponent<Collider>() == null)
                ErrorMessage(string.Format("No Collider found in <color=blue>{0}</color>... Skipping this object!!", item.name), ref error);

            if (targetSurfaces.Exists(i => i.surface == item))
                ErrorMessage(string.Format("Object <color=blue>{0}</color> already in the list.", item.name), ref error);

            return error;
        }

        public void ArrangeSurfaceList(bool state)
        {
            targetSurfaces.RemoveAll(i => state ? i.isUsed : !i.isUsed);
        }

        public void ClearSurfaceList()
        {
            targetSurfaces.Clear();
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

        public string GetPathFromTypeOfObject(string typeOf)
        {
            switch (typeOf)
            {
                case "Coral": return "Items/Cnidarians/Corals/";
            }

            return string.Empty;
        }

        public void ResetTool()
        {
            itemsAreLoaded = false;
            itemsJSON.items.Clear();
            items.Clear();
        }

        private void ErrorMessage(string message, ref bool error)
        {
            error = false;
            Debug.Log(message);
        }
    }
}