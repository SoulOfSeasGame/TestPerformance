using System.Collections;
using System.Collections.Generic;
using TestPerformance;
using UnityEngine;
using System.Linq;

namespace QGM.Managers
{
    public class InstantiatorManager : MonoBehaviour
    {
        public List<GameObject> prefab;
        public int clusterSize = 10;
        private Queue<ItemSector> itemsToQueue = new Queue<ItemSector>();
        private Queue<GameObject> parentsToQueue = new Queue<GameObject>();
        private Coroutine instantiator;

        public static InstantiatorManager Instance;

        private void Awake()
        {
            Instance = this;
            prefab = Resources.LoadAll<GameObject>("Prefabs/Items/Corals/").ToList();
        }

        public void AddGameObject(ItemSector go, GameObject parent)
        {
            itemsToQueue.Enqueue(go);
            parentsToQueue.Enqueue(parent);
            //Debug.Log(string.Format("<color=green>Adding... </color> {0}", go.Id));
            StartingCoroutine();
        }

        public void AddGameObjects(ItemsSector go, GameObject parent)
        {
            foreach (var item in go.items)
            {
                itemsToQueue.Enqueue(item);
                parentsToQueue.Enqueue(parent);
            }

            //Debug.Log(string.Format("<color=green>Adding block of</color> {0} <color=green>objects...</color>", go.Count));
            StartingCoroutine();
        }

        private void StartingCoroutine()
        {
            if (instantiator == null)
            {
                instantiator = StartCoroutine(InstantiateObjects());
                //Debug.Log("<color=cyan>Starting coroutine... </color>");
            }
        }

        private IEnumerator InstantiateObjects()
        {
            yield return new WaitForEndOfFrame();

            int numberOfItemsToInstantiate = itemsToQueue.Count > clusterSize ? clusterSize : itemsToQueue.Count;
            InstantiateObject(numberOfItemsToInstantiate);
            //Debug.Log(string.Format("Instantiating {0} objects...", numberOfItemsToInstantiate));
            HandleCoroutine();
        }

        private void HandleCoroutine()
        {
            if (itemsToQueue.Count > 0)
                instantiator = StartCoroutine(InstantiateObjects());
            else
            {
                StopCoroutine(instantiator);
                instantiator = null;
                //Debug.Log("<color=orange>Stopping coroutine... </color>");
            }
        }

        private void InstantiateObject(int items)
        {
            for (int n = 0; n < items; n++)
            {
                ItemSector item = itemsToQueue.Dequeue();
                GameObject parent = parentsToQueue.Dequeue();
                GameObject go = Instantiate(prefab.Find(i => i.name == item.idName));
                go.transform.position = item.position;
                go.transform.rotation = item.rotation;
                go.transform.localScale = item.scale;
                go.transform.parent = parent.transform;
                parent.GetComponent<ItemsManager>().AddChildren(go);
            }
        }
    }
}
