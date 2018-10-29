using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SOS.Tools
{ 
    public class QuadrantsSectorsToJSON : Editor
    {
        static private List<GameObject> quadrants = new List<GameObject>();
        static public List<string> sectors = new List<string>();

        [MenuItem("SOS/Export to JSON/Create Scene from Quadrants", false, 10)]
        static void WriteStringToJSON()
        {
            GameObject[] root = Selection.gameObjects;

            if (root.Length == 0 || root.Length > 2)
            {
                Debug.Log("<color=red>Please! </color> Select ONE object to export the quadrants & sectors to JSON.");
                return;
            }

            GetChildren(root[0].gameObject);

            if (quadrants.Count > 0)
            {
                string scene = "{\"scene\":[";
                string quadrant = string.Empty;
                string idQuadrant = string.Empty;
                string sectors = string.Empty;

                for (int n = 0; n < quadrants.Count; n++)
                {
                    idQuadrant = string.Empty;
                    sectors = string.Empty;

                    idQuadrant = string.Format("{{\"idQuadrant\":\"{0}\",\"sectors\":[", quadrants[n].gameObject.name);

                    for (int s = 0; s < quadrants[n].transform.childCount; s++)
                        sectors += string.Format("\"{0}\",", quadrants[n].transform.GetChild(s).gameObject.name);

                    sectors = string.Format("{0}{1}]}},", idQuadrant, sectors.Substring(0, sectors.Length - 1));

                    quadrant += sectors;
                }

                scene = string.Format("{0}{1}]}}", scene, quadrant.Substring(0, quadrant.Length - 1));

                WriteToFile("Scenes", scene);
            }
        }

        static private void GetChildren(GameObject parent)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
                quadrants.Add(parent.transform.GetChild(i).gameObject);
        }
        
        static private void WriteToFile(string name, string str)
        {
            string path = string.Format("Assets/Resources/Json/{0}.json", name);
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(str);
            writer.Close();
        }
    }
}
