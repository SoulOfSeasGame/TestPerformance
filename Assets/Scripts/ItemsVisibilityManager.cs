using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestPerformance
{
    public class ItemsVisibilityManager : MonoBehaviour //Every sector have one of this class.
    {
        private const int CLUSTER_SIZE = 15;
        private const string MAIN_CHARACTER = "MainCharacter";

        private Vector3 mainCharacter;
        private List<GameObject> itemsInSector = new List<GameObject>();
        private Queue<GameObject> itemsToQueue;
        private Coroutine visibility;
        private bool state;

        private void Awake()
        {
            SetItemsList();
            SetObjectsVisibility(GameObject.FindGameObjectWithTag(MAIN_CHARACTER).transform.position, true);
        }

        public void SetObjectsVisibility(Vector3 position, bool state)
        {
            mainCharacter = position;
            this.state = state;

            itemsToQueue = new Queue<GameObject>(OrderList());
            visibility = StartCoroutine(ShowObjects(state));
        }

        private void SetItemsList()
        {
            for (int i = 0; i < transform.childCount; i++)
                itemsInSector.Add(transform.GetChild(i).gameObject);
        }

        private IEnumerator ShowObjects(bool state)
        {
            yield return new WaitForEndOfFrame();

            int numberOfItemsToShow = itemsToQueue.Count > CLUSTER_SIZE ? CLUSTER_SIZE : itemsToQueue.Count;
            SetVisibility(numberOfItemsToShow, state);
            HandleCoroutine(state);
        }

        private void HandleCoroutine(bool state)
        {
            if (itemsToQueue.Count > 0)
                visibility = StartCoroutine(ShowObjects(state));
            else
            {
                StopCoroutine();
                if (!state) gameObject.SetActive(false);
            }
        }

        private void SetVisibility(int items, bool state)
        {
            for (int n = 0; n < items; n++)
            {
                GameObject item = itemsToQueue.Dequeue();
                item.SetActive(state);
            }
        }

        private void StopCoroutine()
        {
            if (visibility != null)
            {
                StopCoroutine(visibility);
                visibility = null;
                itemsToQueue = null;
            }
        }

        private List<GameObject> OrderList()
        {
            Dictionary<GameObject, float> childrenByDistance = new Dictionary<GameObject, float>();

            foreach (var item in itemsInSector)
                childrenByDistance.Add(item, Vector3.Distance(item.transform.position, mainCharacter));

            var myList = childrenByDistance.ToList();

            if (state)
                myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            else
                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            List<GameObject> listOrderer = new List<GameObject>();

            foreach (var item in myList)
                listOrderer.Add(item.Key);

            return listOrderer;
        }
    }
}
