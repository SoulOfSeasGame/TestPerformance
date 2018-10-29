using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ScatterTool
{
    public class ScatterToolView : EditorWindow
    {
        private GameObject scatterToolGO;
        private ScatterTool scatterTool;
        private STImplementation scatterImp;
        private STParameters scatterParams;

        private SerializedObject serializedObject;
        private SerializedProperty targetSurfaces;
        private SerializedProperty material;
        private bool itemsVisibilityToogle = true;

        private GUIStyle ToggleButtonStyleNormal = null;
        private GUIStyle ToggleButtonStyleToggled = null;
        private Color active = new Color(0.93f, 0.93f, 0.93f);
        private Color deactive = new Color(0.75f, 0.75f, 0.75f);

        private GUIStyle font = new GUIStyle();

        private Vector2 scrollSurfaces;
        private Vector2 scrollItems;

        [MenuItem("Tools/Scatter")]
        static void Init()
        {
            ScatterToolView window = EditorWindow.GetWindow<ScatterToolView>("Scatter tool");
            window.minSize = window.maxSize = new Vector2(800, 700);
        }

        void OnEnable()
        {
            CreateScatterObject();

            serializedObject = new SerializedObject(scatterTool);
            material = serializedObject.FindProperty("material");

            font.fontSize = 11;
            font.fontStyle = FontStyle.Bold;
        }

        void OnGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginVertical("BOX");
                    {
                        scatterTool.iterations = EditorGUILayout.IntSlider("Iterations", scatterTool.iterations, 1, 10);
                        scatterTool.numberOfElements = EditorGUILayout.IntSlider("Number of elements", scatterTool.numberOfElements, 1, 1000);
                        scatterTool.globalScale = EditorGUILayout.FloatField("Global scale", scatterTool.globalScale);
                        //source = EditorGUILayout.ObjectField(source, typeof(Material), true);
                        /////
                        //source = Resources.Load("Rock") as Material;
                        /////
                        //Material mat = source as Object as Material;
                        EditorGUILayout.PropertyField(material);

                        if (GUILayout.Button("Create instances", GUILayout.Height(48)))
                            scatterImp.Init(Selection.gameObjects);
                    }
                    GUILayout.EndVertical();
                    //if (GUILayout.Button("Load surfaces", GUILayout.Height(30))) AddSurfaces();

                    //GUILayout.BeginHorizontal();
                    //{
                    //    if (GUILayout.Button("Remove Selected", GUILayout.Height(30))) ArrangeSurfaceList(true);
                    //    if (GUILayout.Button("Remove Unselected", GUILayout.Height(30))) ArrangeSurfaceList(false);
                    //    if (GUILayout.Button("Clear list", GUILayout.Height(30))) ClearSurfaces();
                    //}
                    //GUILayout.EndHorizontal();

                    //CreateSurfacesList();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    if (!scatterTool.itemsAreLoaded)
                        scatterTool.LoadItems();
                    else
                    {
                        if (GUILayout.Button("Reload Items and Properties", GUILayout.Height(30)) && EditorUtility.DisplayDialog("Reload items", "Reloading items will delete all your changes. Are you sure you want to reload them anyway?", "Yes", "No"))
                        {
                            scatterTool.ResetTool();
                            scatterTool.LoadItems();
                        }

                        CreateItems();

                        if (GUILayout.Button("Reset Tool", GUILayout.Height(50))) scatterTool.ResetTool();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            DestroyImmediate(scatterToolGO);
        }

        private void CreateScatterObject()
        {
            try
            {
                scatterToolGO = FindObjectOfType<ScatterTool>().gameObject;
            }
            catch
            {
                scatterToolGO = new GameObject();
                scatterToolGO.AddComponent<ScatterTool>();
                scatterToolGO.name = "ScatterObject";
                scatterToolGO.hideFlags = HideFlags.HideInHierarchy;
                scatterToolGO.hideFlags = HideFlags.HideInInspector;
            }

            scatterTool = scatterToolGO.GetComponent<ScatterTool>();
            scatterImp = scatterToolGO.GetComponent<STImplementation>();
            scatterParams = scatterToolGO.GetComponent<STParameters>();
        }

        #region LOAD SURFACES
        private void AddSurfaces()
        {
            scatterTool.AddElements(Selection.transforms);
        }

        private void CreateSurfacesList()
        {
            SetButtonsBehaviour();

            scrollSurfaces = EditorGUILayout.BeginScrollView(scrollSurfaces);
            {
                for (int i = 0; i < scatterTool.targetSurfaces.Count; i++)
                {
                    Surface surface = scatterTool.targetSurfaces[i];
                    GUI.color = surface.isUsed ? active : deactive;

                    if (GUILayout.Button(surface.surface.name, surface.isUsed ? ToggleButtonStyleNormal : ToggleButtonStyleToggled))
                        surface.isUsed = !surface.isUsed;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void ArrangeSurfaceList(bool state)
        {
            scatterTool.ArrangeSurfaceList(state);
        }

        private void ClearSurfaces()
        {
            scatterTool.ClearSurfaceList();
        }
        #endregion

        #region LOAD ITEMS & PROPERTIES
        private void CreateItems()
        {
            SetButtonsBehaviour();
            
            scrollItems = EditorGUILayout.BeginScrollView(scrollItems);
            {
                for (int i = 0; i < scatterTool.items.Count; i++)
                {
                    Item item = scatterTool.items[i];
                    GUI.color = item.isUsed ? active : deactive;

                    GUILayout.BeginHorizontal("BOX");
                    {
                        ImageItemAndButton(item);
                        BasicProperties(item);
                        GUILayout.FlexibleSpace();
                        AdvancedProperties(item);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private Texture2D GetTexture(Item item)
        {
            string path = string.Format("{0}{1}", item.path, item.name);
            return Resources.Load<Texture2D>("Sprites/" + path);
        }

        private void ImageItemAndButton(Item item)
        {
            if (GUILayout.Button(GetTexture(item), item.isUsed ? ToggleButtonStyleNormal : ToggleButtonStyleToggled, GUILayout.Height(64), GUILayout.MaxWidth(64)))
                item.isUsed = !item.isUsed;
        }

        private void BasicProperties(Item item)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label(item.genus, font, GUILayout.Height(font.fontSize));
                GUILayout.Label(item.specie, font, GUILayout.Height(font.fontSize));
                GUILayout.Label(item.stageSexVar, font, GUILayout.Height(font.fontSize));
                item.useCustomParams = GUILayout.Toggle(item.useCustomParams, "Use custom parameters");

                if (item.useCustomParams)
                {
                    if (GUILayout.Button("Scatter Prop.", GUILayout.Height(28), GUILayout.MaxWidth(150)))
                        EditScatterProperties(item);
                }  
                //GUILayout.BeginHorizontal();
                //{
                //    GUILayout.Label("% ");
                //    item.percentage = EditorGUILayout.Slider(item.percentage, 0, 100);
                //}
                //GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void AdvancedProperties(Item item)
        {
            GUILayout.BeginVertical();
            {
                LocalScale(item);
                GUILayout.Space(2);
                AlignToNormalAndOffsetHeight(item);
                GUILayout.Space(2);
                LocalRotation(item);
            }
            GUILayout.EndVertical();
        }

        private void LocalScale(Item item)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Local Scale: ");
                Vector2 localScale = EditorGUILayout.Vector2Field(GUIContent.none, new Vector2(item.commonSize, item.maxSize), GUILayout.MaxWidth(100));
                item.commonSize = localScale.x;
                item.maxSize = localScale.y;
            }
            GUILayout.EndHorizontal();
        }

        private void AlignToNormalAndOffsetHeight(Item item)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Align to normal:");
                item.isAlign = EditorGUILayout.Toggle(item.isAlign, GUILayout.MaxWidth(30));
                GUILayout.FlexibleSpace();
                GUILayout.Label("Offset height:");
                item.heightOffset = EditorGUILayout.FloatField(item.heightOffset, GUILayout.MaxWidth(36));
            }
            GUILayout.EndHorizontal();
        }

        private void LocalRotation(Item item)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Local Rotation:");
                item.canRotateInX = EditorGUILayout.ToggleLeft("x", item.canRotateInX, GUILayout.MaxWidth(30));
                GUILayout.Space(10);
                item.canRotateInY = EditorGUILayout.ToggleLeft("y", item.canRotateInY, GUILayout.MaxWidth(30));
                GUILayout.Space(10);
                item.canRotateInZ = EditorGUILayout.ToggleLeft("z", item.canRotateInZ, GUILayout.MaxWidth(30));
            }
            GUILayout.EndHorizontal();
        }

        private void EditScatterProperties(Item item)
        {
            Rect sw = EditorWindow.GetWindow(typeof(ScatterToolView)).position;
            STParametersView window = ScriptableObject.CreateInstance(typeof(STParametersView)) as STParametersView;

            if (item.scatterParams.isOpen)
                Debug.Log(string.Format("There is another instance of <color=blue>{0}</color> already open", item.name));
            else
            {
                Vector2 windowSize = new Vector2(320, 460);

                window.Init(item, scatterParams);
                window.ShowUtility();
                window.titleContent = new GUIContent(string.Format("{0} {1}",item.genus, item.specie), GetTexture(item));
                window.position = new Rect(sw.x + (sw.width / 2) - windowSize.x / 2, sw.y + (sw.height / 2) - windowSize.y / 2, windowSize.x, windowSize.y);
                window.maxSize = window.minSize = windowSize;
            }
        }
        #endregion

        //private void CreateList()
        //{
        //    items.Clear();

        //    for (int i = 0; i < scatterTool.itemsEditor.Count; i++)
        //    {
        //        Item itemInspc = scatterTool.itemsEditor[i];
        //        ItemParams item = new ItemParams();
        //        item.idItem = Resources.Load<GameObject>(string.Format("{0}{1}", itemInspc.path, itemInspc.name));
        //        item.isUsed = itemInspc.isUsed;
        //        item.percentage = Mathf.CeilToInt(itemInspc.percentage);
        //        item.localScale = new Vector2(itemInspc.commonSize, itemInspc.maxSize);
        //        item.localRotation = new Vector3(GetRandomRotation(itemInspc.canRotateInX), GetRandomRotation(itemInspc.canRotateInY), GetRandomRotation(itemInspc.canRotateInZ));
        //        item.isAlign = itemInspc.isAlign;
        //        item.heightOffset = itemInspc.heightOffset;
        //        items.Add(item);
        //    }

        //    scatterTool.SetItemProperties(items);
        //}

        //private float GetRandomRotation(bool isActive)
        //{
        //    const float MIN_ROTATION = 0;
        //    const float MAX_ROTATION = 360;
        //    return isActive ? Random.Range(MIN_ROTATION, MAX_ROTATION) : MIN_ROTATION;
        //}

        private void SetButtonsBehaviour()
        {
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }
        }
    }
}