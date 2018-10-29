///====================================///
/// We are no longer using this script ///
///====================================///

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SOS.Tools
{
    public class ScatterObjects : ScriptableWizard
    {
        private const string CONSOLE_DETAILS = "Some errors found! See the console for more details.";
        static List<Material> Surface;
        static int NumObjects = 500;
        static List<DistribuitonObject> DistributionObject = null;
        static Vector2 RandomRotate = new Vector2(0, 360);
        static Vector2 RandomScale = new Vector2(0.8f, 1.2f);

        public List<Material> surface;
        [Range(0, 3000)]
        public int numObjects;
        [Header("Offsets")]
        public Vector2 randomRotate;
        public Vector2 randomScale;
        [Space]
        public List<DistribuitonObject> distributionObject = null;

        private Vector3 minPos;
        private Vector3 maxPos;
        private Vector3 pos;
        private MeshRenderer rend;
        private RaycastHit hit;
        private int counter = 0;
        private float numberOfObjects;

        [MenuItem("Tools/Objects utilities/Scatter Objects...")]
        static void CreateWizard()
        {
            EditorWindow window = ScriptableWizard.DisplayWizard<ScatterObjects>("Scatter Objects", "Apply & Close", "Apply");
            window.minSize = new Vector2(550, 330);
            window.maxSize = new Vector2(550, 900);
        }

        public ScatterObjects()
        {
            surface = Surface;
            numObjects = NumObjects;
            randomRotate = RandomRotate;
            randomScale = RandomScale;
            distributionObject = DistributionObject;
        }

        void OnWizardUpdate()
        {
            Surface = surface;
            NumObjects = numObjects;
            RandomRotate = randomRotate;
            RandomScale = randomScale;
            DistributionObject = distributionObject;
        }

        void OnWizardCreate()
        {
            CreateObjects();
        }

        void OnWizardOtherButton()
        {
            CreateObjects();
        }

        private void CreateObjects()
        {
            if (distributionObject == null) return;
            if (surface.Count == 0)
            {
                errorString = "No surface assigned";
                return;
            }

            Transform[] source = Selection.GetTransforms(SelectionMode.TopLevel);

            numberOfObjects = source.Length * numObjects;
            counter = 0;

            if (source.Length == 0)
                errorString = "Nothing selected!!! Please, select at least one object.";
            else
            {
                for (int t = 0; t < source.Length; t++)
                {
                    if (CheckObjectProperties(source[t])) continue;
                    GetLimits(source[t]);
                    SearchingPlace(source[t]);
                }
            }
        }

        private bool CheckObjectProperties(Transform source)
        {
            bool error = false;

            if (!source.GetComponent<MeshRenderer>())
                ErrorMessage(CONSOLE_DETAILS, ref error, string.Format("Component <MeshRenderer> not found in <color=blue>{0}</color>... Skipping this object!!", source.name));
            else if(!source.GetComponent<MeshCollider>())
                ErrorMessage(CONSOLE_DETAILS, ref error, string.Format("No component <MeshCollider> in <color=blue>{0}</color>... Skipping this object!!", source.name));

            return error;
        }

        private void GetLimits(Transform source)
        {
            rend = source.GetComponent<MeshRenderer>();
            minPos = rend.bounds.min;
            maxPos = rend.bounds.max;
        }

        private void SearchingPlace(Transform source)
        {
            for (int n = 0; n < numObjects; n++)
            {
                pos = new Vector3(Random.Range(minPos.x, maxPos.x), 50, Random.Range(minPos.z, maxPos.z));

                if (++counter < numberOfObjects)
                    EditorUtility.DisplayProgressBar("Creating instance...", string.Format("{0}/{1}", ++counter, numberOfObjects), (++counter / numberOfObjects));
                else
                    EditorUtility.ClearProgressBar();

                if (!surface.Contains(GetNameOfHitMaterial())) continue;

                CreateObject(source);
            }
        }

        private void CreateObject(Transform source)
        {
            int index = Random.Range(0, distributionObject.Count);
            GameObject go = PrefabUtility.InstantiatePrefab(distributionObject[index].prefab) as GameObject;
            pos.y = hit.point.y;
            go.name = distributionObject[index].prefab.name;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.identity;

            if (distributionObject[index].align)
            {
                go.transform.rotation = Quaternion.LookRotation(hit.normal);
                go.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
            else
                go.transform.rotation = Quaternion.Euler(0, Random.Range(randomRotate.x, randomRotate.y), 0);

            go.transform.localScale *= Random.Range(randomScale.x, randomScale.y);
            go.transform.localScale *= Random.Range(distributionObject[index].scaleOffset.x, distributionObject[index].scaleOffset.y);
            go.transform.parent = source;
        }

        private Material GetNameOfHitMaterial()
        {
            if (Physics.Raycast(pos, Vector3.down, out hit))
            {
                MeshCollider collider = hit.collider as MeshCollider;
                Mesh mesh;

                if (collider != null)
                {
                    mesh = collider.sharedMesh;

                    int limit = hit.triangleIndex * 3;
                    int submesh;

                    for (submesh = 0; submesh < mesh.subMeshCount; submesh++)
                    {
                        int numIndices = mesh.GetTriangles(submesh).Length;
                        if (numIndices > limit) break;

                        limit -= numIndices;
                    }

                    return collider.GetComponent<MeshRenderer>().sharedMaterials[submesh];
                }
                else
                {
                    errorString = " Mesh Collider is requiered";
                    return null;
                }
            }
            else return null;
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private bool ErrorsHandle(Transform[] source)
        {
            bool error = false;

            if (source.Length == 0)
                ErrorMessage("Nothing selected! Please, select at least one object.", ref error);

            return error;
        }

        private void ErrorMessage(string message, ref bool error, string altMessage = "")
        {
            errorString = message;
            error = true;
            Debug.Log(altMessage);
        }
    }
}