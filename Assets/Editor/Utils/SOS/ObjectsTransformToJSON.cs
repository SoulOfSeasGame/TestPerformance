using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SOS.Tools
{
    public class ObjectsTransformToJSON : Editor
    {
        [MenuItem("SOS/Export to JSON/Export objects from sector", false, 20)]
        static void WriteStringToJSON()
        {
            GameObject[] parent = Selection.gameObjects;

            if (parent.Length == 0)
                Debug.Log("<color=red>Nothing selected!!!</color> Please, select at least one object to export to JSON.");
            else
            {
                for (int p = 0; p<parent.Length; p++)
                {
                    List<GameObject> children = new List<GameObject>();

                    for (int i = 0; i < parent[p].transform.childCount; i++)
                        children.Add(parent[p].transform.GetChild(i).gameObject);

                    if (children.Count > 0)
                    {
                        string str = "{\"items\":[";

                        for (int n = 0; n < children.Count; n++)
                        {
                            string item = string.Format
                            (
                                "{{\"idName\":\"{0}\",\"position\":{{\"x\":{1},\"y\":{2},\"z\":{3}}},\"rotation\":{{\"x\":{4},\"y\":{5},\"z\":{6},\"w\":{7}}},\"scale\":{{\"x\":{8},\"y\":{9},\"z\":{10}}}}}",
                                //GetNameOfTheObject(children[n]),
                                children[n].name,
                                children[n].transform.position.x,
                                children[n].transform.position.y,
                                children[n].transform.position.z,
                                children[n].transform.rotation.x,
                                children[n].transform.rotation.y,
                                children[n].transform.rotation.z,
                                children[n].transform.rotation.w,
                                Mathf.Abs(children[n].transform.localScale.x),
                                Mathf.Abs(children[n].transform.localScale.y),
                                Mathf.Abs(children[n].transform.localScale.z)
                            );

                            str = string.Format("{0}{1},", str, item);
                        }

                        str = string.Format("{0}]}}", str.Substring(0, str.Length - 1));
                        WriteToFile(parent[p].name, str);
                    }
                }
            } 
        }

        static private string GetNameOfTheObject(GameObject go)
        {
            const string CLONE = "(Clone)";
            string clone = go.name.Substring(go.name.Length - CLONE.Length);

            if (clone == CLONE)
                return go.name.Substring(0, go.name.Length - CLONE.Length);
            else
                return go.name;

            //if (go.GetComponent<LODGroup>() != null)
            //    return go.name.Substring(0, go.name.Length - CLONE.Length);
            //else
            //    return go.GetComponent<Renderer>().sharedMaterial.name;
        }

        static private void WriteToFile(string name, string str)
        {
            string path = string.Format("Assets/Resources/Json/Sectors/{0}.json", name);
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(str);
            writer.Close();
        }
    }
}
