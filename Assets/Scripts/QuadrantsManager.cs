using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

namespace TestPerformance
{
    public class QuadrantsManager : MonoBehaviour
    {
        private const string QUADRANT = "Quadrant";

        public GameObject mainCharacter;
        public float timeToUpdate = 1.0f;
        public int distanceToShow = 50;

        private List<Quadrant> usedQuadrants = new List<Quadrant>();
        private List<Quadrant> quadrants = new List<Quadrant>();
        private QuadrantsJSON scene;
        private GameObject quadrant;

        private int numQuadrantsPerSide;
        private int numSectorsPerSide;
        private int widthQuadrant;
        private float widthSection;
        
        void Awake()
        {
            LoadQuadrantsFromJSON();
            GetValuesFromScene();
            CreateQuadrants();
            LoadSectors();

            CheckSectorsVisibility();
            StartCoroutine(UpdateSections());
            StartCoroutine(StartMainCharacter());
        }

        private void LoadQuadrantsFromJSON()
        {
            TextAsset sectionJSON = Resources.Load("Json/Scenes") as TextAsset;
            scene = JsonUtility.FromJson<QuadrantsJSON>(sectionJSON.text);
        }

        private void GetValuesFromScene()
        {
            numQuadrantsPerSide = (int)Mathf.Sqrt(scene.scene.Count);
            widthQuadrant = GameState.TERRAIN_WIDTH / numQuadrantsPerSide;
            numSectorsPerSide = (int)Mathf.Sqrt(scene.scene[0].sectors.Count);
            widthSection = widthQuadrant / numSectorsPerSide;
        }

        private void CreateQuadrants()
        {
            int halfOffset = widthQuadrant / 2;

            for (int y = 0; y < numQuadrantsPerSide; y++)
            {
                for (int x = 0; x < numQuadrantsPerSide; x++)
                {
                    CreateQuadrant(widthQuadrant, halfOffset, y, x);
                    CreateCollider(widthQuadrant);
                }
            }
        }

        private void CreateQuadrant(int offset, int halfOffset, int y, int x)
        {
            quadrant = new GameObject();
            quadrant.transform.position = new Vector3(offset * x + halfOffset, 0, offset * y + halfOffset);
            quadrant.transform.parent = transform;
            quadrant.name = string.Format("{0}_{1}_{2}", QUADRANT, x + 1, y + 1);
            quadrant.AddComponent<CheckInfluence>().Init(this, widthQuadrant);
        }

        private void CreateCollider(int offset)
        {
            BoxCollider collider = quadrant.AddComponent<BoxCollider>();
            float colliderSize = offset + (widthSection * 2);
            collider.size = new Vector3(colliderSize, distanceToShow, colliderSize);
            collider.isTrigger = true;
        }

        private void LoadSectors()
        {
            foreach (var item in scene.scene)
            {
                Quadrant quadrant = new Quadrant();
                quadrant.idQuadrant = transform.Find(item.idQuadrant);
                List<Sector> sectors = new List<Sector>();

                foreach (var sect in item.sectors)
                {
                    Sector sector = new Sector();

                    string[] pos = sect.Split('_');
                    Vector3 position = new Vector3(GetPositionFromCoordinate(pos[1]), 0, GetPositionFromCoordinate(pos[2]));
                    sector.idSector = sect;
                    sector.position = position;
                    sectors.Add(sector);
                }

                quadrant.sectors = sectors;
                quadrants.Add(quadrant);
            }
        }

        private float GetPositionFromCoordinate(string coord)
        {
            return (Int32.Parse(coord) * widthSection) - (widthSection / 2);
        }

        public void AddQuadrant(Transform quadrant)
        {
            usedQuadrants.Add(quadrants.Find(n => n.idQuadrant == quadrant));
        }

        public void RemoveQuadrant(Transform quadrant)
        {
            Quadrant quadrantHidden = usedQuadrants.Find(q => q.idQuadrant == quadrant);
            HideAllSectorsInQuadrant(quadrantHidden);
            usedQuadrants.Remove(quadrantHidden);
        }

        IEnumerator UpdateSections()
        {
            yield return new WaitForSeconds(timeToUpdate);
            CheckSectorsVisibility();
            StartCoroutine(UpdateSections());
        }

        private void CheckSectorsVisibility()
        {
            Vector3 posMainCharacter = new Vector3(mainCharacter.transform.position.x, 0, mainCharacter.transform.position.z);

            foreach (var quadrant in usedQuadrants)
            {
                foreach (var sector in quadrant.sectors)
                {
                    float ourDistance = Vector3.Distance(posMainCharacter, sector.position);
                    string sectorName = sector.idSector;

                    if (ourDistance < distanceToShow && !SceneManager.GetSceneByName(sectorName).isLoaded)
                        LoadScene(sectorName, posMainCharacter, sector.position);
                    else if (SceneManager.GetSceneByName(sector.idSector).isLoaded)
                    {
                        GameObject rootSector = SceneManager.GetSceneByName(sectorName).GetRootGameObjects()[0];

                        if (ourDistance < distanceToShow && !rootSector.activeSelf)
                            ShowScene(rootSector, posMainCharacter, sector.position);
                        else if (ourDistance > distanceToShow && rootSector.activeSelf)
                            HideScene(rootSector, posMainCharacter, sector.position);
                    }
                }
            }
        }

        private void HideAllSectorsInQuadrant(Quadrant quadrant)
        {
            foreach (var item in quadrant.sectors)
            {
                if (SceneManager.GetSceneByName(item.idSector).isLoaded)
                {
                    GameObject rootSector = SceneManager.GetSceneByName(item.idSector).GetRootGameObjects()[0];
                    if (rootSector.activeSelf) HideScene(rootSector, Vector3.zero, Vector3.zero);
                }
            }
        }

        private void LoadScene(string name, Vector3 a, Vector3 b)
        {
            SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            GameState.NumberOfScenes++;
            Debug.DrawLine(a, b, Color.green, 1.5f);
            Debug.Log(string.Format("<color=blue>[LOADING]</color> scene {0}", name));
        }

        private void ShowScene(GameObject sector, Vector3 a, Vector3 b)
        {
            sector.SetActive(true);
            sector.GetComponent<ItemsVisibilityManager>().SetObjectsVisibility(mainCharacter.transform.position, true);

            Debug.Log(string.Format("<color=green>[SHOWING]</color> scene {0}", sector.name));
            Debug.DrawLine(a, b, Color.green, 1.5f);
        }

        private void HideScene(GameObject section, Vector3 a, Vector3 b)
        {
            section.GetComponent<ItemsVisibilityManager>().SetObjectsVisibility(mainCharacter.transform.position, false);

            Debug.Log(string.Format("<color=red>[HIDDING]</color> scene {0}", section.name));
            Debug.DrawLine(a, b, Color.red, 1.5f);
        }

        IEnumerator StartMainCharacter ()
        {
            yield return new WaitForSeconds(2);
            mainCharacter.AddComponent<DrawMainCharacterGizmo>().Init(GameState.MAX_DISTANCE_SIGHT);
            mainCharacter.GetComponent<FirstPersonController>().enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Debug.Break();
        }
    }
}