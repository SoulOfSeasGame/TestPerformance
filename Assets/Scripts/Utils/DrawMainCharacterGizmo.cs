using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMainCharacterGizmo : MonoBehaviour
{
    private float radius;

    public void Init(float radius)
    {
        this.radius = radius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
