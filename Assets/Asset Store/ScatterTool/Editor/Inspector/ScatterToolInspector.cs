///====================================///
/// We are no longer using this script ///
///====================================///

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ScatterTool
{
    [CustomEditor(typeof(ScatterTool))]
    public class ScatterToolInspector : Editor
    {
        ScatterTool scatterTool;

        SerializedProperty itemsAreLoaded;
        private List<Item> items = new List<Item>();
        private bool itemsVisibilityToogle = true;
        private GUIStyle ToggleButtonStyleNormal = null;
        private GUIStyle ToggleButtonStyleToggled = null;
        private Color active = new Color(0.93f, 0.93f, 0.93f);
        private Color deactive = new Color(0.75f, 0.75f, 0.75f);

        void OnEnable()
        {
            scatterTool = (ScatterTool)target;

            itemsAreLoaded = serializedObject.FindProperty("itemsAreLoaded");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            

            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();

            if (!itemsAreLoaded.boolValue)
            {
                EditorGUILayout.HelpBox("No items loaded", MessageType.Warning, true);
                if (GUILayout.Button("Load Items and Properties", GUILayout.Height(30)))
                {
                    LoadItemsAndProperties();
                }
            }
            else
            {

                //if (GUILayout.Button("GO!", GUILayout.Height(50)))
                //    CreateList();

                itemsVisibilityToogle = EditorGUILayout.Foldout(itemsVisibilityToogle, "Show Items & Properties");

                if (itemsVisibilityToogle)
                    CreateItems();

                if (GUILayout.Button("Reload Items and Properties", GUILayout.Height(30)))
                {
                    scatterTool.ResetTool();
                    LoadItemsAndProperties();
                }
                if (GUILayout.Button("Reset Tool", GUILayout.Height(50)))
                {
                    scatterTool.ResetTool();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void LoadItemsAndProperties()
        {
            Debug.Log("Loading items...");
            scatterTool.LoadItemsAndProperties();
        }

        //private void AddPrefabs()
        //{
        //    Transform[] source = Selection.GetTransforms(SelectionMode.TopLevel);
        //    scatterTool.AddElements(source);
        //}

        private void CreateItems()
        {
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            for (int i = 0; i < scatterTool.items.Count; i++)
            {
                Item itemInspc = scatterTool.items[i];
                GUI.color = itemInspc.isUsed ? active : deactive;

                GUILayout.BeginHorizontal("BOX");
                {
                    ImageItemAndButton(itemInspc);
                    BasicProperties(itemInspc);
                    AdvancedProperties(itemInspc);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void ImageItemAndButton(Item itemInspc)
        {
            string path = string.Format("{0}{1}", itemInspc.path, itemInspc.name);
            Texture2D texture = Resources.Load<Texture2D>("Sprites/" + path);

            if (GUILayout.Button(texture, itemInspc.isUsed ? ToggleButtonStyleNormal : ToggleButtonStyleToggled, GUILayout.Height(64), GUILayout.MaxWidth(64)))
            {
                itemInspc.isUsed = !itemInspc.isUsed;
                GUI.color = Color.blue;
            }
        }

        private static void BasicProperties(Item itemInspc)
        {
            GUIStyle font = new GUIStyle();
            font.fontSize = 11;
            font.fontStyle = FontStyle.Bold;

            GUILayout.BeginVertical();
            {
                GUILayout.Label(itemInspc.genus, font, GUILayout.Height(font.fontSize));
                GUILayout.Label(itemInspc.specie, font, GUILayout.Height(font.fontSize));
                GUILayout.Label(itemInspc.stageSexVar, font, GUILayout.Height(font.fontSize));

                //GUILayout.BeginHorizontal();
                //{
                //    GUILayout.Label("% ");
                //    itemInspc.percentage = EditorGUILayout.Slider(itemInspc.percentage, 0, 100);
                //}
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private static void AdvancedProperties(Item itemInspc)
        {
            GUILayout.BeginVertical();
            {
                LocalScale(itemInspc);
                GUILayout.Space(2);
                AlignToNormalAndOffsetHeight(itemInspc);
                GUILayout.Space(2);
                LocalRotation(itemInspc);
            }
            GUILayout.EndVertical();
        }

        private static void LocalScale(Item itemInspc)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Local Scale: ");
                Vector2 localScale = EditorGUILayout.Vector2Field(GUIContent.none, new Vector2(itemInspc.commonSize, itemInspc.maxSize), GUILayout.MaxWidth(100));
                itemInspc.commonSize = localScale.x;
                itemInspc.maxSize = localScale.y;
            }
            GUILayout.EndHorizontal();
        }

        private static void AlignToNormalAndOffsetHeight(Item itemInspc)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Align to normal:");
                itemInspc.isAlign = EditorGUILayout.Toggle(itemInspc.isAlign, GUILayout.MaxWidth(30));
                GUILayout.FlexibleSpace();
                GUILayout.Label("Offset height:");
                itemInspc.heightOffset = EditorGUILayout.FloatField(itemInspc.heightOffset, GUILayout.MaxWidth(36));
            }
            GUILayout.EndHorizontal();
        }

        private static void LocalRotation(Item itemInspc)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Local Rotation:");
                itemInspc.canRotateInX = EditorGUILayout.ToggleLeft("x", itemInspc.canRotateInX, GUILayout.MaxWidth(30));
                GUILayout.Space(10);
                itemInspc.canRotateInY = EditorGUILayout.ToggleLeft("y", itemInspc.canRotateInY, GUILayout.MaxWidth(30));
                GUILayout.Space(10);
                itemInspc.canRotateInZ = EditorGUILayout.ToggleLeft("z", itemInspc.canRotateInZ, GUILayout.MaxWidth(30));
            }
            GUILayout.EndHorizontal();
        }
    }
}