using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class InitScene : MonoBehaviour
    {
        public GameObject mainCharacter;
        public TerrainManager terrainManager;

        private void Awake()
        {
            LoadSections();
            terrainManager.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Debug.Break();
        }

        private void LoadSections()
        {
            Transform terrain = terrainManager.gameObject.transform;

            for (int i = 0; i < terrain.childCount; i++)
            {
                TextAsset sectionJSON = Resources.Load("Json/Sections/" + terrain.GetChild(i).name) as TextAsset;

                string jsonString = sectionJSON == null ? null : sectionJSON.text;

                OldSection section = new OldSection();
                GameState.oldSections.Add(section);

                section.IdSection = terrain.GetChild(i).gameObject;
                section.Items = new ItemsSector();
                section.Items = JsonUtility.FromJson<ItemsSector>(jsonString);

                section.IdSection.AddComponent<VisibilityManager>().Init(50);
                section.IdSection.AddComponent<ItemsManager>().Init(GameState.oldSections[i], mainCharacter);

            }
        }
    }
}