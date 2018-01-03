using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateLength : MonoBehaviour {

    GameObject cursor;
    Vector3 pos;

    // Use this for initialization
    void Start () {
        Vector3[] verts = GetComponent<MeshFilter>().mesh.vertices;
        float distance = -1;
        cursor = transform.parent.GetChild(1).gameObject;

        for (int i = 0; i < verts.Length; i++)
        {
            if (distance < Vector3.Distance(cursor.transform.position, transform.TransformPoint(verts[i])))
            {
                distance = Vector3.Distance(cursor.transform.position, verts[i]);
                pos = verts[i];
            }
        }

        print(distance);
    }

    private void OnDrawGizmos()
    {
        if (cursor != null)
            Gizmos.DrawLine(cursor.transform.position, transform.TransformPoint(pos));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
