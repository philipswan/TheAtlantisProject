using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingContainer : MonoBehaviour {
    public enum Scale
    {
        Default,
        Ring,
        TransitRing,
        Test
    };
    public Scale scale;

    private float newScale;
    private float radius;
    private Vector3 furthestPoint;
    private GameObject ring;

    private bool printed = false;

	// Use this for initialization
	void Start () {
        ring = transform.GetChild(0).gameObject;

        newScale = GetNewScale();
        radius = ring.GetComponent<Ring>().GetDiameter() / 2;
        print(radius);

		List<Vector3> children = new List<Vector3>();

		for (int i=0; i<transform.childCount; i++)
		{
			children.Add(transform.GetChild(i).position);
		}

        transform.localPosition = Vector3.zero - ring.transform.InverseTransformDirection(ring.transform.right) * radius;
		//transform.position = GameObject.FindGameObjectWithTag("Positioner").transform.position;

		for (int i=0; i<transform.childCount; i++)
		{
			transform.GetChild(i).position = children[i];
		}
			
		transform.localScale = new Vector3(newScale, newScale, newScale);
	}
	
	// Update is called once per frame
	void Update () {
        if (ring.name == "Ring - Transit" && !printed)
        {
            Vector3[] verts = ring.GetComponent<MeshFilter>().mesh.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
                print(Vector3.Distance(transform.localPosition, verts[i]));
            }
        }

        printed = true;
    }

    //private float GetDiameter()
    //{
    //    float diameter = -1;
    //    Vector3 objectPos = transform.GetChild(0).localPosition;

    //    Vector3[] verts = transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices;

    //    for (int i=0; i<verts.Length; i++)
    //    {
    //        float distance = Vector3.Distance(objectPos, verts[i]);
    //        if (distance > diameter)
    //        {
    //            diameter = distance;
    //            furthestPoint = verts[i];
    //        }
    //    }

    //    print(diameter);
    //    return diameter / 2;
    //}

    //public void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(furthestPoint, transform.GetChild(0).localPosition);
    //}

    private float GetNewScale()
    {
        float val = 1;
        switch (scale)
        {
            case Scale.Ring:
                val = 0.989981f;
                break;
            case Scale.TransitRing:
                val = 0.988481f;
                break;
            case Scale.Test:
                val = 0.9985f;
                break;
            default:
                val = 1;
                break;
        }

        return val;
    }
}
