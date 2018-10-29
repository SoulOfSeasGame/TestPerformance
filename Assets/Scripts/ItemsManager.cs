using System.Collections.Generic;
using UnityEngine;
using flag = System.Boolean;
using QGM.Managers;

namespace TestPerformance
{
    public class ItemsManager : MonoBehaviour
    {
        private flag objectsAlreadyInstantiated = false;

        private OldSection section;
        private ItemsSector items;
        private List<GameObject> children = new List<GameObject>();
        private GameObject mainCharacter;
        private VisibilityManager vm;

        public void Init(OldSection section, GameObject mainCharacter)
        {
            this.section = section;
            this.items = section.Items;
            this.mainCharacter = mainCharacter;
            vm = GetComponent<VisibilityManager>();
        }

        public void SetVisibilityOn()
        {
            gameObject.SetActive(true);
            InstantiateObjectsInQueue();
        }

        public void SetVisibilityOff()
        {
            vm.AddGameObjects(children, false, gameObject);
        }

        private void InstantiateObjectsInQueue()
        {
            if (section.Items == null) return;

            if (!objectsAlreadyInstantiated)
            {
                objectsAlreadyInstantiated = true;
                InstantiatorManager.Instance.AddGameObjects(section.Items, gameObject);
            }
            else
                vm.AddGameObjects(children, true, gameObject);
        }

        public void AddChildren(GameObject go)
        {
            children.Add(go);
        }
    }
}