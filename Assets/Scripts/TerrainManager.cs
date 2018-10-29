using System.Collections;
using UnityEngine;

namespace TestPerformance
{
    public class TerrainManager : MonoBehaviour
    {
        public GameObject mainCharacter;
        public float timeToUpdate = 1.0f;
        public int distanceToShow = 50;

        public void Init()
        {
            SetSectionsVisibility();
            StartCoroutine(UpdateSections());
        }

        IEnumerator UpdateSections()
        {
            yield return new WaitForSeconds(timeToUpdate);
            SetSectionsVisibility();
            StartCoroutine(UpdateSections());
        }

        private void SetSectionsVisibility()
        {
            foreach (var item in GameState.oldSections)
            {
                float dist = Vector3.Distance(mainCharacter.transform.position, item.IdSection.transform.position);
                ItemsManager im = item.IdSection.GetComponent<ItemsManager>();

                if (distanceToShow > dist && !item.IdSection.activeSelf)
                {
                    im.SetVisibilityOn();
                    //GameState.numberOfVisibleItems += item.Items.Count;
                }
                else if (distanceToShow < dist && item.IdSection.activeSelf)
                {
                    im.SetVisibilityOff();
                    //GameState.numberOfVisibleItems -= item.Items.Count;
                }
            }

            //ShowLogItems();
        }

        private void ShowLogItems()
        {
            Debug.Log(string.Format("Visible items: {0}/{1}", GameState.numberOfVisibleItems, GameState.totalNumberOfItems));
        }
    }
}