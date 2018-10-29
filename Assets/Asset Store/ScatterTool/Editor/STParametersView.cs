using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScatterTool
{
    public class STParametersView : EditorWindow
    {
        private STParameters STParams;
        private ScatterParams sp = new ScatterParams();
        private Item item;
        
        private Texture link;
        private Texture arrowUp;
        private Texture arrowDown;
        private Texture warning;
        private GUIStyle font = new GUIStyle();

        private bool isLinked = true;
        private GUIStyle ToggleButtonStyleNormal = null;
        private GUIStyle ToggleButtonStyleToggled = null;
        private bool editorHasChanged = false;

        public void Init(Item item, STParameters STParams)
        {
            this.item = item;
            this.STParams = STParams;
            this.sp = item.scatterParams;

            sp.isOpen = true;
        }

        void OnEnable()
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string path = AssetDatabase.GetAssetOrScenePath(ms);
            path = Path.GetDirectoryName(path) + "/Icons/";

            link = (Texture)AssetDatabase.LoadAssetAtPath(path + "link.png", typeof(Texture));
            arrowUp = (Texture)AssetDatabase.LoadAssetAtPath(path + "arrowUp.png", typeof(Texture));
            arrowDown = (Texture)AssetDatabase.LoadAssetAtPath(path + "arrowDown.png", typeof(Texture));
            warning = (Texture)AssetDatabase.LoadAssetAtPath(path + "warning.png", typeof(Texture));

            font.fontSize = 10;
            font.fontStyle = FontStyle.Normal;
        }

        private void OnDisable()
        {
            sp.isOpen = false;
        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            SetButtonsBehaviour();
            OffsetAndScale();
            CurveAndImage();
            CheckEditorChanges();
            EditButtons();
            ShowNotifications();
        }

        private void OffsetAndScale()
        {
            GUILayout.BeginHorizontal("BOX");
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUIUtility.labelWidth = 70;
                        sp.offsetX = EditorGUILayout.FloatField("OffsetX:", sp.offsetX, GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                        sp.scaleX = EditorGUILayout.FloatField("ScaleX:", sp.scaleX);
                        if (isLinked) sp.scaleY = sp.scaleX / sp.scaleY;
                        if (GUILayout.Button(arrowDown, GUILayout.Height(18), GUILayout.Width(18))) ResetScaleValue(ref sp.scaleX, ref sp.scaleY);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        sp.offsetY = EditorGUILayout.FloatField("OffsetY:", sp.offsetY, GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                        EditorGUI.BeginDisabledGroup(isLinked);
                        {
                            sp.scaleY = EditorGUILayout.FloatField("ScaleY:", sp.scaleY);
                            if (GUILayout.Button(arrowUp, GUILayout.Height(18), GUILayout.Width(18))) ResetScaleValue(ref sp.scaleY, ref sp.scaleX);
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
        }

        private void CurveAndImage()
        {
            sp.curve = EditorGUILayout.CurveField(sp.curve);
            GUILayout.Label(STParams.GenerateTexture(sp));
        }

        private void CheckEditorChanges()
        {
            if (EditorGUI.EndChangeCheck())
                editorHasChanged = true;
        }

        private void EditButtons()
        {
            GUILayout.BeginHorizontal("BOX");
            {
                if (GUILayout.Button("Save", GUILayout.Height(24))) Save();
                if (GUILayout.Button("Copy", GUILayout.Height(24))) STParams.CopyParams(sp, item.name);
                if (GUILayout.Button("Paste", GUILayout.Height(24))) Paste();
                if (GUILayout.Button("Reset", GUILayout.Height(24))) ResetP();
            }
            GUILayout.EndHorizontal();
        }

        private void ResetP()
        {
            sp = STParams.ResetParams(sp);
            editorHasChanged = true;
        }

        private void Save()
        {
            STParams.SaveParams(sp, item);
            editorHasChanged = false;
        }

        private void Paste()
        {
            sp = STParams.PasteParams(sp);
            editorHasChanged = true;
        }

        private void ShowNotifications()
        {
            if (!editorHasChanged) return;

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(warning, GUILayout.Width(32));
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Space(3);
                    GUILayout.Label("The parameters have been changed.", font, GUILayout.Height(font.fontSize));
                    GUILayout.Space(3);
                    GUILayout.Label("Press 'Save' to keep the changes", font, GUILayout.Height(font.fontSize));
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void ResetScaleValue(ref float value1, ref float value2)
        {
            value2 = value1;
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