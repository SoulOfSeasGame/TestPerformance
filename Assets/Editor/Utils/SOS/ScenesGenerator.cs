using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SOS.Tools
{
    public class ScenesGenerator : Editor
    {
        private const string PATH_SCENES = "Assets/Scenes/Sectors/";

        [MenuItem("SOS/Create Scenes from Object(s)", false, 12)]
        static void CreateScenesFromObjects()
        {
            GameObject[] parent = Selection.gameObjects;

            if (parent.Length == 0)
                Debug.Log("<color=red>Nothing selected!!!</color>");
            else
            {
                foreach (var item in parent)
                {
                    item.transform.parent = null;
                    Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    EditorSceneManager.MoveGameObjectToScene(item, newScene);

                    string path = string.Format("{0}{1}.unity", PATH_SCENES, item.name);
                    if (EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path))
                        Debug.Log(string.Format("Saving scene <color=blue>{0}</color>", item.name));
                }

                Debug.Log(string.Format("<color=green>{0}</color> Scenes exported succesfully", parent.Length.ToString()));
            } 
        }
    }
}
