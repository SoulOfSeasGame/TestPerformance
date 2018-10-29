using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class CheckInfluence : MonoBehaviour
    {
        private QuadrantsManager qv;
        private Vector3 size;
        
        private Color32 colorIn = new Color32(128, 255, 128, 50);
        private Color32 colorOut = new Color32(128, 128, 255, 170);
        private Color32 colorSolid;
        private Color32 colorWireIn = new Color32(128, 255, 128, 255);
        private Color32 colorWireOut = new Color32(128, 128, 255, 255);
        private Color32 colorWire;

        public void Init(QuadrantsManager quadrantsVisibility, int widthQuadrant)
        {
            qv = quadrantsVisibility;
            colorSolid = colorOut;
            colorWire = colorWireOut;
            size = new Vector3(widthQuadrant, 10, widthQuadrant);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("MainCharacter"))
            {
                qv.AddQuadrant(gameObject.transform);
                colorSolid = colorIn;
                colorWire = colorWireIn;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("MainCharacter"))
            {
                qv.RemoveQuadrant(gameObject.transform);
                colorSolid = colorOut;
                colorWire = colorWireOut;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = colorSolid;
            Gizmos.DrawCube(gameObject.transform.position, size);
            Gizmos.color = colorWire;
            Gizmos.DrawWireCube(gameObject.transform.position, size);
        }
    }
}
