using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScatterTool
{
    public class STParameters : MonoBehaviour
    {
        private const string BASIC_PARAMS = "STBasicParams";
        private const string CURVE_PARAMS = "STCurveParams";

        private ScatterParams sp;
        private ScatterParams copyParams = new ScatterParams();
        private Item item;
        
        private string idName;
        private bool copyIsActive = false;

        private string itemParams = string.Empty;
        private string itemCurve = string.Empty;

        public ScatterParams InitItemParameters(Item item)
        {
            this.idName = item.name;

            sp = new ScatterParams();
            sp.isOpen = false;

            if (!PlayerPrefs.HasKey(idName + BASIC_PARAMS))
            {
                //Debug.Log(idName + " <color=red>NO EXIST!</color> Creating new parameters...");
                sp = ResetParams(sp);
                SaveParams(sp, item);
            }
            else
                LoadParams(sp);

            return sp;
        }

        public ScatterParams ResetParams(ScatterParams sp)
        {
            sp.scaleX = 10;
            sp.scaleY = 10;
            sp.offsetX = 0;
            sp.offsetY = 0;
            sp.curve = AnimationCurve.Linear(0, 1, 1, 0);
            sp.noiseMap = GenerateTexture(sp);

            return sp;
        }

        public void SaveParams(ScatterParams sp, Item item)
        {
            string itemParams = string.Format("{0}~{1}~{2}~{3}", sp.scaleX.ToString(), sp.scaleY.ToString(), sp.offsetX.ToString(), sp.offsetY.ToString());
            PlayerPrefs.SetString(item.name + BASIC_PARAMS, itemParams);

            string itemCurve = string.Empty;
            Keyframe[] key = sp.curve.keys;

            for (int i = 0; i < key.Length; i++)
                itemCurve += string.Format("{0}~{1}~{2}~{3}¬", key[i].time, key[i].value, key[i].inTangent, key[i].outTangent);

            itemCurve = itemCurve.Remove(itemCurve.Length - 1);
            PlayerPrefs.SetString(item.name + CURVE_PARAMS, itemCurve);

            item.scatterParams.noiseMap = GenerateTexture(sp);
            item.scatterParams = sp;
            
            Debug.Log(itemParams);
            Debug.Log(itemCurve);
            Debug.Log("<color=yellow>SAVING</color> parameters for " + item.name);
        }

        public void LoadParams(ScatterParams sp)
        {
            //Debug.Log("<color=green>LOADING</color> params for " + idName);

            string[] p = PlayerPrefs.GetString(idName + BASIC_PARAMS).Split('~');
            sp.scaleX = float.Parse(p[0]);
            sp.scaleY = float.Parse(p[1]);
            sp.offsetX = float.Parse(p[2]);
            sp.offsetY = float.Parse(p[3]);

            string[] c = PlayerPrefs.GetString(idName + CURVE_PARAMS).Split('¬');
            Keyframe[] ks = new Keyframe[c.Length];

            for (int i = 0; i < c.Length; i++)
            {
                string[] cp = c[i].Split('~');
                ks[i] = new Keyframe(float.Parse(cp[0]), float.Parse(cp[1]), float.Parse(cp[2]), float.Parse(cp[3]));
            }

            sp.curve = new AnimationCurve();
            sp.curve.keys = ks;
            sp.noiseMap = GenerateTexture(sp);

            this.sp = sp;
        }

        public void CopyParams(ScatterParams sp, string name)
        {
            copyIsActive = true;

            copyParams.scaleX = sp.scaleX;
            copyParams.scaleY = sp.scaleY;
            copyParams.offsetX = sp.offsetX;
            copyParams.offsetY = sp.offsetY;
            copyParams.curve = sp.curve;

            Debug.Log(string.Format("<color=blue>COPYING</color> properties from {0}", name));
        }

        public ScatterParams PasteParams(ScatterParams sp)
        {
            if (!copyIsActive) return sp;

            sp.scaleX = copyParams.scaleX;
            sp.scaleY = copyParams.scaleY;
            sp.offsetX = copyParams.offsetX;
            sp.offsetY = copyParams.offsetY;
            sp.curve = copyParams.curve;
            sp.noiseMap = GenerateTexture(sp);

            return sp;
        }

        public Texture2D GenerateTexture(ScatterParams sp)
        {
            Texture2D noiseMap = new Texture2D(ScatterTool.SIZEMAP, ScatterTool.SIZEMAP);

            for (int x = 0; x < ScatterTool.SIZEMAP; x++)
            {
                for (int y = 0; y < ScatterTool.SIZEMAP; y++)
                {
                    Color color = CalculateColor(sp, x, y);
                    noiseMap.SetPixel(x, y, color);
                }
            }

            noiseMap.Apply();
            return noiseMap;
        }

        private Color CalculateColor(ScatterParams sp, int x, int y)
        {
            float xCoord = (float)x / ScatterTool.SIZEMAP * sp.scaleX + sp.offsetX;
            float yCoord = (float)y / ScatterTool.SIZEMAP * sp.scaleY + sp.offsetY;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            sample = sp.curve.Evaluate(sample);
            return new Color(sample, sample, sample);
        }
    }
}