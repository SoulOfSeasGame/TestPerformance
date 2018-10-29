using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace ScatterTool
{
    public class STImplementation : MonoBehaviour
    {
        private ScatterTool scatterTool;
        private List<GameObject> selection = new List<GameObject>();
        private int numTotalOfElements = 0;
        private GameObject combinedMesh;
        private MeshCollider collider;
        private RaycastHit hit;

        public void Init(GameObject[] selection)
        {
            this.selection = selection.ToList();

            scatterTool = GetComponent<ScatterTool>();

            //Debug.Log(scatterTool.iterations);
            //Debug.Log(scatterTool.numberOfElements);
            CombineObjects();
            CreateIterations();
        }

        public void CombineObjects()
        {
            combinedMesh = new GameObject();
            MeshFilter mFilter = combinedMesh.AddComponent<MeshFilter>();
            MeshRenderer mRenderer = combinedMesh.AddComponent<MeshRenderer>();
            combinedMesh.name = "CombinedMesh";

            List<MeshFilter> meshFilters = new List<MeshFilter>();

            foreach (var item in selection)
            {
                if (item.GetComponent<MeshFilter>())
                    meshFilters.Add(item.GetComponent<MeshFilter>());
            }

            List<CombineInstance> combine = new List<CombineInstance>();

            CombineMeshes(meshFilters, combine);

            mFilter.sharedMesh = new Mesh();
            mFilter.sharedMesh.CombineMeshes(combine.ToArray(), true);
            mRenderer.material = scatterTool.material;
            collider = combinedMesh.AddComponent<MeshCollider>();
            combinedMesh.SetActive(true);
        }

        private void CombineMeshes(List<MeshFilter> meshFilters, List<CombineInstance> combine)
        {
            for (int n = 0; n < meshFilters.Count; n++)
            {
                CombineInstance c = new CombineInstance();
                int index = 0;

                if (MeshExist(meshFilters[n], ref index))
                {
                    c.mesh = meshFilters[n].sharedMesh;
                    c.transform = meshFilters[n].transform.localToWorldMatrix;
                    c.subMeshIndex = index;

                    combine.Add(c);
                }
            }
        }

        private bool MeshExist(MeshFilter mesh, ref int index)
        {
            for (int m = 0; m < mesh.sharedMesh.subMeshCount; m++)
            {
                if (mesh.gameObject.GetComponent<MeshRenderer>().sharedMaterials[m].name == scatterTool.material.name)
                {
                    index = m;
                    return true;
                }
            }
            return false;
        }

        private void CreateIterations()
        {
            Vector3 min = combinedMesh.GetComponent<MeshCollider>().bounds.min;
            Vector3 max = combinedMesh.GetComponent<MeshCollider>().bounds.max;
            float side = max.x - min.x;

            for (int i = 0; i < scatterTool.iterations; i++)
                CreateInstance(min, max, side);

            Debug.Log(string.Format("Num total of created elements: <color=blue>{0}</color>", numTotalOfElements));
        }

        private void CreateInstance(Vector3 min, Vector3 max, float side)
        {
            for (int i = 0; i < scatterTool.numberOfElements; i++)
            {
                int index = Random.Range(0, scatterTool.items.Count);
                index = 0;

                Texture2D noiseMap = scatterTool.items[index].scatterParams.noiseMap;
                Vector3 worldPosition = new Vector3(Random.Range(min.x, max.x), 10, Random.Range(min.z, max.z));
                float whitePercent = noiseMap.GetPixel(GetCoordInMap(worldPosition.x, min.x, side), GetCoordInMap(worldPosition.z, min.z, side)).r;

                if (whitePercent > Random.Range(0.0f, 1.0f) && GetNameOfHitMaterial(worldPosition) == scatterTool.material)
                {
                    CreateItem(scatterTool.items[index], worldPosition);
                    numTotalOfElements++;
                }
            }
        }

        private int GetCoordInMap(float pos, float min, float side)
        {
            float posPercent = (pos - min) * 100 / side;
            return Mathf.FloorToInt(posPercent * ScatterTool.SIZEMAP / 100);
        }

        private Material GetNameOfHitMaterial(Vector3 pos)
        {
            if (Physics.Raycast(pos, Vector3.down, out hit))
            {
                if (hit.collider.gameObject != combinedMesh) return null;

                Mesh mesh = collider.sharedMesh;
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
            else return null;
        }

        private void CreateItem(Item item, Vector3 pos)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(item.item) as GameObject;

            BasicProperties(item, go);
            SetPosition(item, pos, go);
            AlignItem(item, go);
            RotateItem(item, go);
            SetScale(item, go);
            SetFinalProperties(go);
        }

        private void BasicProperties(Item item, GameObject go)
        {
            go.SetActive(false);
            go.name = item.name;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        private void SetPosition(Item item, Vector3 pos, GameObject go)
        {
            pos.y = hit.point.y;
            go.transform.position = pos;
            Vector3 localPosition = go.transform.localPosition;
            localPosition.y += item.heightOffset;
            go.transform.localPosition = localPosition;
        }

        private void AlignItem(Item item, GameObject go)
        {
            if (item.isAlign)
            {
                go.transform.rotation = Quaternion.LookRotation(hit.normal);
                go.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
        }

        private void RotateItem(Item item, GameObject go)
        {
            if (item.canRotateInX) go.transform.rotation *= Quaternion.Euler(Random.Range(0, 360), 0, 0);
            if (item.canRotateInY) go.transform.rotation *= Quaternion.Euler(0, Random.Range(0, 360), 0);
            if (item.canRotateInZ) go.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        private void SetScale(Item item, GameObject go)
        {
            go.transform.localScale *= Random.Range(item.commonSize, item.maxSize);
            go.transform.localScale *= scatterTool.globalScale;
            //go.transform.localScale *= Random.Range(distributionObject[index].scaleOffset.x, distributionObject[index].scaleOffset.y);
        }

        private void SetFinalProperties(GameObject go)
        {
            go.transform.parent = combinedMesh.transform;
            go.SetActive(true);
        }
    }
}