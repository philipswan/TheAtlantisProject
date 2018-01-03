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
    private string ringTag;

    private bool printed = false;
    private bool positionSet = false;

    private Constants.Configuration config;

	// Use this for initialization
	void Start () {
        config = Constants.Configuration.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (config.RingDiameter == -1 || config.TransitRingDiamter == -1)
        { return; }

        SetPosition();

        enabled = false;

        //if (ring.name == "Ring - Transit" && !printed)
        //{
        //    Vector3[] verts = ring.GetComponent<MeshFilter>().mesh.vertices;

        //    for (int i = 0; i < verts.Length; i++)
        //    {
        //        print(Vector3.Distance(transform.localPosition, verts[i]));
        //    }
        //}

        //printed = true;
    }

    private void SetPosition()
    {
        ringTag = GetTag();
        ring = GameObject.FindGameObjectWithTag(ringTag);
        newScale = GetNewScale();

        radius = ring.GetComponent<Ring>().GetDiameter() / 2;
        //print(name + " " + radius);

        List<Vector3> children = new List<Vector3>();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).position);
        }

        transform.localPosition = Vector3.zero - ring.transform.InverseTransformDirection(ring.transform.right) * radius;
        //transform.position = GameObject.FindGameObjectWithTag("Positioner").transform.position;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = children[i];
        }

        transform.localScale = new Vector3(newScale, 1, 1);
    }

    private string GetTag()
    {
        string tag = "";

        switch (scale)
        {
            case Scale.Ring:
                tag = "Ring";
                break;
            case Scale.TransitRing:
                tag = "Ring - Transit";
                break;
            default:
                tag = "Ring";
                break;
        }

        return tag;
    }

    private float GetNewScale()
    {
        float val = 1;
        switch (scale)
        {
            case Scale.Ring:
                val = 0.985555f;
                break;
            case Scale.TransitRing:
                val = 0.98522f;
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
