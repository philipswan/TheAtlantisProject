/**
 * Based on a script by Steffen (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in $primitives_966_104.zip, originally named "Primitives.cs")
 *
 * Editted by Michael Zoller on December 6, 2015.
 * It was shortened by about 30 lines (and possibly sped up by a factor of 2) by consolidating math & loops and removing intermediate Collections.
 */
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Ring : MonoBehaviour {

	[Header("Ring Properties")]
    public float TubeRadius = 0.001f;
    public int NumSegments = 100;
    public int NumTubes = 12;
	[Header("Tram Options")]
	[Tooltip("Set true if this is the tram ring")]
	public bool TramRing;

	private Material[] DefaultMaterials;								// Regular materials
	private List<Material> HighlightMaterials = new List<Material>();	// Regarul materials + highlight material
	private float tetheredRingRadius;									// Calculated radius of ring
	private Constants.Configuration config;								// Holds reference to config file
	private float furthestPoint;										// Furthest point from position on the mesh. Used to center in a container for the menu
	private bool highlited;												// Current materials used

    void Start() {
		config = Constants.Configuration.Instance;
		highlited = false;

		tetheredRingRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
		RefreshRing();

		GameObject p = Instantiate(gameObject, transform.parent);
		p.GetComponent<Ring>().enabled = false;

		if (TramRing)
		{
            FloatingMenu.Instance.AddItems(p, "Transit Ring", new Vector3(14, 2860, 15), furthestPoint);
			tag = "Ring - Transit";
        }
		else
		{
            FloatingMenu.Instance.AddItems(p, "Tethered Ring", new Vector3(14, 2860, 15), furthestPoint);
			tag = "Ring";
        }
    }

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
	public void SetMaterials()
	{
		if (highlited)
		{
			GetComponent<MeshRenderer>().materials = DefaultMaterials;
		}
		else
		{
			GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
			GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 1000);
		}

		highlited = ! highlited;
	}

	/// <summary>
	/// Retuns the diameter of the ring
	/// </summary>
	/// <param name="_go">Go.</param>
	private float FindPointOnMesh(GameObject _go)
	{
		Vector3[] verticies = GetComponent<MeshFilter>().mesh.vertices;
		float distance = 0;

		for (int i=0; i<verticies.Length; i++)
		{
			if (distance < Vector3.Distance(transform.localPosition, verticies[i]))
			{
				distance = Vector3.Distance(transform.localPosition, verticies[i]);
			}
		}

		//print(TramRing + " " + 5000 * (distance/2));
		return distance;
	}

    private void RefreshRing() {
        // Total vertices
        int totalVertices = NumSegments * NumTubes;

        // Total primitives
        int totalPrimitives = totalVertices * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the mesh
        Mesh mesh = new Mesh();

        // Init the vertex and triangle arrays
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangleIndices = new int[totalIndices];

        // Calculate size of a segment and a tube
        float segmentSize = 2 * Mathf.PI / (float)NumSegments;
        float tubeSize = 2 * Mathf.PI / (float)NumTubes;

        // Create floats for our xyz coordinates
		float x = 0, y = 0, z = 0;

        // Begin loop that fills in both arrays
        for (int i = 0; i < NumSegments; i++)
        {
            // Find next (or first) segment offset
            int n = (i + 1) % NumSegments; // changed segmentList.Count to numSegments

			// Find the current and next segments
            int currentTubeOffset = i * NumTubes;
            int nextTubeOffset = n * NumTubes;
            float normi = (float)i / NumSegments * Mathf.PI;
            float funci = 0.5f - (Mathf.Cos(normi) / 2.0f);
            float angle = funci * 2 * Mathf.PI;

			for (int j = 0; j < NumTubes; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % NumTubes; // changed currentTube.Count to numTubes

                // Find the 4 vertices that make up a quad
                int iv1 = currentTubeOffset + j;
                int iv2 = currentTubeOffset + m;
                int iv3 = nextTubeOffset + m;
                int iv4 = nextTubeOffset + j;

                // Calculate X, Y, Z coordinates.
				x = (tetheredRingRadius + TubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Cos(angle) - tetheredRingRadius;
                z = (tetheredRingRadius + TubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Sin(angle);
                y = TubeRadius * Mathf.Sin(j * tubeSize);

                // Add the vertex to the vertex array
                vertices[iv1] = new Vector3(x, y, z);

                // "Draw" the first triangle involving this vertex
                triangleIndices[iv1 * 6] = iv1;
                triangleIndices[iv1 * 6 + 1] = iv2;
                triangleIndices[iv1 * 6 + 2] = iv3;

                // Finish the quad
                triangleIndices[iv1 * 6 + 3] = iv3;
                triangleIndices[iv1 * 6 + 4] = iv4;
                triangleIndices[iv1 * 6 + 5] = iv1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

		MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;

		MeshRenderer mr = GetComponent<MeshRenderer>();
		DefaultMaterials = mr.materials;

		SetHighlightMaterials();

		furthestPoint = FindPointOnMesh(gameObject);
	}

	private void SetHighlightMaterials()
	{
		for (int i=0; i<DefaultMaterials.Length; i++)
		{
			HighlightMaterials.Add(DefaultMaterials[i]);
		}

		HighlightMaterials.Add(Resources.Load("Silhouetted Diffuse") as Material);
	}
}
