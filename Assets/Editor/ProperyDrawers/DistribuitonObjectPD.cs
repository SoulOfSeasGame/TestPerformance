using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DistribuitonObject))]
public class DistribuitonObjectPropertyDrawer : PropertyDrawer
{
    List<float> widths = new List<float> { 8, 25, 8, 20, 3, 5, 15, 5 };

    float space;
    Rect contentPosition;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.indentLevel = 0;

        label = EditorGUI.BeginProperty(position, label, property);
        contentPosition = EditorGUI.PrefixLabel(position, GUIContent.none);
        contentPosition.width += EditorGUIUtility.currentViewWidth - contentPosition.width;
        space = GetSpace();

        EditorGUI.LabelField(GetContent(0), "Prefab");
        EditorGUI.PropertyField(GetContent(1), property.FindPropertyRelative("prefab"), GUIContent.none);
        EditorGUI.LabelField(GetContent(2), "Scale");
        EditorGUI.PropertyField(GetContent(3), property.FindPropertyRelative("scaleOffset"), GUIContent.none);
        EditorGUI.LabelField(GetContent(4), "%");
        EditorGUI.PropertyField(GetContent(5), property.FindPropertyRelative("percent"), GUIContent.none);
        EditorGUI.LabelField(GetContent(6), "Align Normal");
        EditorGUI.PropertyField(GetContent(7), property.FindPropertyRelative("align"), GUIContent.none);
        
        EditorGUI.EndProperty();
    }

    private Rect GetContent(int n)
    {
        Rect rectLabelPrefab = contentPosition;
        rectLabelPrefab.width *= (widths[n] / 100);
        rectLabelPrefab.x = GetXPosition(n);
        return rectLabelPrefab;
    }

    private float GetSpace()
    {
        float x = 0;

        for (int i = 0; i < widths.Count; i++)
            x += (widths[i] / 100);

        float width = contentPosition.width - contentPosition.width * x;
        return width / (widths.Count - 1);
    }

    private float GetXPosition(int n)
    {
        float x = 0;

        for (int i = 0; i < n; i++)
            x += (widths[i] / 100);

        float width = contentPosition.width;
        return (width *= x) + space * (n+1);
    }
}
