using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NewScatterTool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemModel))]
    public class ItemModelEditor : Editor
    {
        ItemModel item
        {
            get { return (ItemModel)target; }
        }

        Color dColor = new Color32(200, 200, 200, 255);
        Color aColor = Color.white;
        GUIStyle font;
        private bool itemIsLoaded = false;

        private void OnEnable()
        {
            font = new GUIStyle();
            font.fontSize = 12;
            font.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShowGameObjectField();

            if (item.item != null)
            {
                LoadJSONValues();
                ShowBasicProperties();
                ShowItemProperties();
                ShowScatterParameters();
            }

            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(item);
        }

        private void ShowGameObjectField()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Drag and drop the prefab you want to use to scatter", EditorStyles.miniLabel);
                item.item = (GameObject)EditorGUILayout.ObjectField(item.item, typeof(GameObject), false);
                GUI.color = dColor;
            }
            GUILayout.EndHorizontal();
        }

        private void LoadJSONValues()
        {
            if (itemIsLoaded) return;

            item.GetInfoFromJSON();
            itemIsLoaded = true;
        }

        private void ShowBasicProperties()
        {
            

            string path = string.Format("{0}{1}", item.classification, item.idName);
            Texture2D texture = Resources.Load<Texture2D>("Sprites/" + path);

            GUILayout.BeginHorizontal("BOX");
            {
                GUILayout.Space(4);
                GUILayout.Label(texture, GUILayout.MaxHeight(48), GUILayout.MaxWidth(70));
                GUI.color = Color.white;
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(4);
                    string id = item.genus + " " + item.specie;
                    GUILayout.Label(id, EditorStyles.boldLabel);
                    GUILayout.Label(item.sexStageVar, EditorStyles.boldLabel);
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("R") && Selection.objects.Length == 1)
                {
                    string oldName = AssetDatabase.GetAssetPath(Selection.objects[0]);
                    string newName = item.idName + ".asset";
                    AssetDatabase.RenameAsset(oldName, newName);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowItemProperties()
        {
            GUI.color = dColor;
            GUILayout.BeginVertical("BOX");
            {
                GUILayout.Label("Item Properties", EditorStyles.boldLabel);
                ShowSize();
                ShowRotation();
                ShowAlineation();
                ShowOffset();
            }
            GUILayout.EndVertical();
        }

        private void ShowSize()
        {
            GUILayout.Label("Size: ");
            EditorGUI.indentLevel++;
            GUI.color = Color.white;
            GUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 70;
                float x = EditorGUILayout.FloatField("Min ", item.props.size.x);
                float y = EditorGUILayout.FloatField("Max ", item.props.size.y);
                item.props.size = new Vector2(x, y);
                EditorGUI.indentLevel--;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void ShowRotation()
        {
            GUILayout.Label("Rotation: ");
            EditorGUI.indentLevel++;

            CanRotateIn(ref item.props.canRotateInX, ref item.props.rotationX, "Can rotate in X: ");
            CanRotateIn(ref item.props.canRotateInY, ref item.props.rotationY, "Can rotate in Y: ");
            CanRotateIn(ref item.props.canRotateInZ, ref item.props.rotationZ, "Can rotate in Z: ");
            EditorGUILayout.Space();
        }

        private void CanRotateIn(ref bool canRotate, ref Vector2 rotation, string label)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 120;
                canRotate = EditorGUILayout.Toggle(label, canRotate);
                EditorGUIUtility.labelWidth = 45;
                float x = EditorGUILayout.FloatField("Min ", rotation.x);
                float y = EditorGUILayout.FloatField("Max ", rotation.y);
                rotation = new Vector2(x, y);
            }
            GUILayout.EndHorizontal();
        }

        private void ShowAlineation()
        {
            GUILayout.Label("Alineation: ");
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 270;
            item.props.isAlign = EditorGUILayout.ToggleLeft("Is aligned to its normal ", item.props.isAlign);
            EditorGUILayout.Space();
        }

        private void ShowOffset()
        {
            GUILayout.Label("Offset: ", GUILayout.Width(70));
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 200;
            item.props.heightOffset = EditorGUILayout.FloatField("Distance to ground: ", item.props.heightOffset);
            EditorGUILayout.Space();
        }

        private void ShowScatterParameters()
        {

        }

        //private void Header(string title)
        //{
        //    GUI.color = dColor;
        //    EditorGUILayout.BeginVertical("Box");
        //    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        //    EditorGUI.indentLevel++;
        //    GUI.color = Color.white;
        //}

        //private void Propertie(string title)
        //{
        //    GUI.color = dColor;
        //    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        //    EditorGUI.indentLevel++;
        //    GUI.color = Color.white;
        //}
    }
}