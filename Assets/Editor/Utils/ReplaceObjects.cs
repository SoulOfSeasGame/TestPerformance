using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ReplaceObjects : ScriptableWizard
{
    static GameObject replacement = null;
    static bool keep = false;

    public GameObject ReplacementObject = null;
    public bool KeepOriginals = false;

    private Transform[] transforms;
    private int counter = 0;
    private float numberOfObjects;

    [MenuItem("Tools/Objects utilities/Replace Object(s)...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ReplaceObjects>("Replace Object(s)", "Apply & Close", "Apply");
    }

    public ReplaceObjects()
    {
        ReplacementObject = replacement;
        KeepOriginals = keep;
    }

    void OnWizardUpdate()
    {
        replacement = ReplacementObject;
        keep = KeepOriginals;
    }

    void OnWizardCreate()
    {
        Create();
    }

    void OnWizardOtherButton()
    {
        Create();
    }

    private void Create()
    {
        transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
        numberOfObjects = transforms.Length;

        if (ErrorsHandle()) return;

        ReplacingObjects();
        DestroyingObjects();
    }

    private void ReplacingObjects()
    {
        foreach (Transform item in transforms)
        {
            GameObject go;
            PrefabType pref = PrefabUtility.GetPrefabType(replacement);

            if (pref == PrefabType.Prefab || pref == PrefabType.ModelPrefab)
                go = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
            else
                go = (GameObject)Editor.Instantiate(replacement);

            go.name = replacement.name;
            go.transform.parent = item.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.parent = item.parent;

            if (++counter < numberOfObjects)
                EditorUtility.DisplayProgressBar("Replacing object...", string.Format("{0}/{1}", ++counter, numberOfObjects), (++counter / numberOfObjects));
            else
                EditorUtility.ClearProgressBar();
        }

        EditorSceneManager.MarkAllScenesDirty();
    }

    private static void DestroyingObjects()
    {
        if (!keep)
        {
            foreach (GameObject g in Selection.gameObjects)
                GameObject.DestroyImmediate(g);
        }
    }

    private bool ErrorsHandle()
    {
        bool error = false;

        if (numberOfObjects == 0)
        {
            Debug.Log("<color=red>Nothing selected!!!</color> Please, select at least one object.");
            error = true;
        }

        if (replacement == null)
        {
            Debug.Log("<color=red>Please,</color> assign the new object to replace.");
            error = true;
        }

        return error;
    }
}