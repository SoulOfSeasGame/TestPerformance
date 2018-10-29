using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScatterTool
{
    public class PerlinNoiseEditor : EditorWindow
    {
        private GameObject source;
        private GameObject prefab;
        private int iterations = 1;
        private int numberOfElements = 1000;
        private int numTotalOfElements = 0;
        private int sizeMap = 312;
        private float scaleX = 10;
        private float scaleY = 10;
        private float offsetX = 0;
        private float offsetY = 0;

        private AnimationCurve curve = AnimationCurve.Linear(0, 0, 10, 10);
        private Texture2D map;
        private Texture link;
        private Texture arrowUp;
        private Texture arrowDown;
        private bool isLinked = true;
        private GUIStyle ToggleButtonStyleNormal = null;
        private GUIStyle ToggleButtonStyleToggled = null;
        private Texture2D noiseMap;

        [MenuItem("Tools/PerlinNoise")]
        static void Init()
        {
            PerlinNoiseEditor window = EditorWindow.GetWindow<PerlinNoiseEditor>("Perlin noise");
            window.minSize = window.maxSize = new Vector2(320, 500);
            window.Show();
        }

        void OnEnable()
        {
            string path = "Assets/Editor/Icons/";
            link = (Texture)AssetDatabase.LoadAssetAtPath(path + "link.png", typeof(Texture));
            arrowUp = (Texture)AssetDatabase.LoadAssetAtPath(path + "arrowUp.png", typeof(Texture));
            arrowDown = (Texture)AssetDatabase.LoadAssetAtPath(path + "arrowDown.png", typeof(Texture));
        }

        void OnGUI()
        {
            SetButtonsBehaviour();

            GUILayout.BeginVertical("BOX");
            {
                prefab = (GameObject)EditorGUILayout.ObjectField((GameObject)prefab, typeof(Object), true);
                iterations = EditorGUILayout.IntSlider("Iterations", iterations, 1, 10);
                numberOfElements = EditorGUILayout.IntSlider("Number of elements", numberOfElements, 1, 10000);
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("BOX");
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUIUtility.labelWidth = 70;
                        offsetX = EditorGUILayout.FloatField("OffsetX:", offsetX, GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                        scaleX = EditorGUILayout.FloatField("ScaleX:", scaleX);
                        if (isLinked) scaleY = scaleX / scaleY;
                        if (GUILayout.Button(arrowDown, GUILayout.Height(18), GUILayout.Width(18))) ResetScaleValue(ref scaleX, ref scaleY);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        offsetY = EditorGUILayout.FloatField("OffsetX:", offsetY, GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                        EditorGUI.BeginDisabledGroup(isLinked);
                        {
                            scaleY = EditorGUILayout.FloatField("ScaleY:", scaleY);
                            if (GUILayout.Button(arrowUp, GUILayout.Height(18), GUILayout.Width(18))) ResetScaleValue(ref scaleY, ref scaleX);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                if (GUILayout.Button(link, isLinked ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Height(38), GUILayout.Width(20)))
                    isLinked = !isLinked;

            }
            GUILayout.EndHorizontal();

            curve = EditorGUILayout.CurveField(curve);
            map = GenerateTexture();
            GUILayout.Label(map);

            if (GUILayout.Button("Create instances", GUILayout.Height(48))) //CreateIterations();
                this.Close();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void CreateIterations()
        {
            numTotalOfElements = 0;

            GameObject[] selection = Selection.gameObjects;

            if (!CheckValidSource(selection)) return;

            source = Selection.gameObjects[0];
            GameObject container = new GameObject();
            container.name = "Container";

            Vector3 min = source.GetComponent<MeshCollider>().bounds.min;
            Vector3 max = source.GetComponent<MeshCollider>().bounds.max;
            float side = max.x - min.x;

            for (int i = 0; i < iterations; i++)
                CreateInstance(container, min, max, side);

            Debug.Log(string.Format("Num total of created elements: <color=blue>{0}</color>", numTotalOfElements));
        }

        private void CreateInstance(GameObject container, Vector3 min, Vector3 max, float side)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                Vector3 worldPosition = new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
                float whitePercent = noiseMap.GetPixel(GetCoordInMap(worldPosition.x, min.x, side), GetCoordInMap(worldPosition.z, min.z, side)).r;

                if (whitePercent > Random.Range(0.0f, 1.0f))
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    go.transform.position = worldPosition;
                    go.transform.parent = container.transform;

                    numTotalOfElements++;
                }
            }
        }

        private int GetCoordInMap(float pos, float min, float side)
        {
            float posPercent = (pos - min) * 100 / side;
            return Mathf.FloorToInt(posPercent * sizeMap / 100);
        }

        private void ResetScaleValue(ref float value1, ref float value2)
        {
            value2 = value1;
        }

        private Texture2D GenerateTexture()
        {
            noiseMap = new Texture2D(sizeMap, sizeMap);

            for (int x = 0; x < sizeMap; x++)
            {
                for (int y = 0; y < sizeMap; y++)
                {
                    Color color = CalculateColor(x, y);
                    noiseMap.SetPixel(x, y, color);
                }
            }

            noiseMap.Apply();
            return noiseMap;
        }

        private Color CalculateColor(int x, int y)
        {
            float xCoord = (float)x / sizeMap * scaleX + offsetX;
            float yCoord = (float)y / sizeMap * scaleY + offsetY;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            sample = curve.Evaluate(sample);
            return new Color(sample, sample, sample);
        }

        private void SetButtonsBehaviour()
        {
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }
        }

        private bool CheckValidSource(GameObject[] selection)
        {
            bool error = true;

            if (selection.Length == 0)
                ErrorMessage(string.Format("<color=red>Nothing selected!</color> Please, select at least one object."), ref error);

            else if (selection[0].GetComponent<Collider>() == null)
                ErrorMessage(string.Format("No Collider found in <color=blue>{0}</color>...", selection[0].name), ref error);

            return error;
        }

        private void ErrorMessage(string message, ref bool error)
        {
            error = false;
            Debug.Log(message);
        }
    }
}