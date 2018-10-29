using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class VisibilityManager : MonoBehaviour
    {
        public int clusterSize = 10;
        private Queue<GameObject> itemsToQueue = new Queue<GameObject>();
        private Coroutine visibility;
        private GameObject parent;

        public void Init(int clusterSize)
        {
            this.clusterSize = clusterSize;
        }

        public void AddGameObjects(List<GameObject> go, bool state, GameObject parent)
        {
            StopCoroutine();
            this.parent = parent;

            foreach (var item in go)
                itemsToQueue.Enqueue(item);

            visibility = StartCoroutine(ShowObjects(state));
        }

        private IEnumerator ShowObjects(bool state)
        {
            yield return new WaitForEndOfFrame();

            int numberOfItemsToShow = itemsToQueue.Count > clusterSize ? clusterSize : itemsToQueue.Count;
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
                if (!state) parent.SetActive(false);
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
                itemsToQueue.Clear();
            }
        }
    }
}
