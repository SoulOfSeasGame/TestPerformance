using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace TestPerformance
{
    public class ScenesManager : MonoBehaviour
    {
        private List<string> scenes = new List<string> { "Sector_13_13", "Sector_13_14", "Sector_13_15", "Sector_13_16", "Sector_13_17", "Sector_13_18", "Sector_13_19", "Sector_13_20", "Sector_14_13", "Sector_14_14", "Sector_14_15", "Sector_14_16", "Sector_14_17", "Sector_14_18", "Sector_14_19", "Sector_14_20", "Sector_15_13", "Sector_15_14", "Sector_15_15", "Sector_15_16", "Sector_15_17", "Sector_15_18", "Sector_15_19", "Sector_15_20", "Sector_16_13", "Sector_16_14", "Sector_16_15", "Sector_16_16", "Sector_16_17", "Sector_16_18", "Sector_16_19", "Sector_16_20", "Sector_17_13", "Sector_17_14", "Sector_17_15", "Sector_17_16", "Sector_17_17", "Sector_17_18", "Sector_17_19", "Sector_17_20", "Sector_18_13", "Sector_18_14", "Sector_18_15", "Sector_18_16", "Sector_18_17", "Sector_18_18", "Sector_18_19", "Sector_18_20", "Sector_19_13", "Sector_19_14", "Sector_19_15", "Sector_19_16", "Sector_19_17", "Sector_19_18", "Sector_19_19", "Sector_19_20", "Sector_20_13", "Sector_20_14", "Sector_20_15", "Sector_20_16", "Sector_20_17", "Sector_20_18", "Sector_20_19", "Sector_20_20" };

        public GameObject mainCharacter;
        public float timeToUpdate = 1.0f;
        public int distanceToShow = 50;
        public float widthSection = 25;
        private AsyncOperation async = null;

        void Awake()
        {
            foreach (var item in scenes)
            {
                string[] pos = item.Split('_');
                Vector3 position = new Vector3(GetPositionFromCoordinate(pos[1]), 0, GetPositionFromCoordinate(pos[2]));
                GameState.scenes.Add(item, position);
            }

            SetSectionsVisibility();
            StartCoroutine(UpdateSections());
        }

        private float GetPositionFromCoordinate(string coord)
        {
            return (Int32.Parse(coord) * widthSection) - (widthSection/2);
        }

        IEnumerator UpdateSections()
        {
            yield return new WaitForSeconds(timeToUpdate);
            SetSectionsVisibility();
            StartCoroutine(UpdateSections());
        }

        private void SetSectionsVisibility()
        {
            Vector3 posMainCharacter = new Vector3(mainCharacter.transform.position.x, 0, mainCharacter.transform.position.z);

            foreach (var item in GameState.scenes)
            {
                float ourDistance = Vector3.Distance(posMainCharacter, item.Value);

                if (ourDistance < distanceToShow && !SceneManager.GetSceneByName(item.Key).isLoaded)
                    LoadScene(item.Key);
                else if (SceneManager.GetSceneByName(item.Key).isLoaded)
                {
                    GameObject section = SceneManager.GetSceneByName(item.Key).GetRootGameObjects()[0];

                    if (ourDistance < distanceToShow && !section.activeSelf)
                        ShowScene(section);
                    else if (ourDistance > distanceToShow && section.activeSelf)
                        HideScene(section);
                }
            }
        }

        private void LoadScene(string name)
        {
            //async = SceneManager.LoadSceneAsync(item.Key, LoadSceneMode.Additive);
            //async.allowSceneActivation = true;
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }

        private void ShowScene(GameObject section)
        {
            section.SetActive(true);
            section.GetComponent<ItemsVisibilityManager>().SetObjectsVisibility(Vector3.zero, true);
        }

        private void HideScene(GameObject section)
        {
            section.GetComponent<ItemsVisibilityManager>().SetObjectsVisibility(Vector3.zero, false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Debug.Break();
        }
    }
}
