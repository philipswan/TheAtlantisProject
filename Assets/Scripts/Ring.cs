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

	public GameObject Tram;
    public float TubeRadius = 0.001f;
    public int NumSegments = 100;
    public int NumTubes = 12;
	[Tooltip("Set true if this script is attached to the tram ring object. This will place the ring lower.")]
	public bool TramRing;

	private List<GameObject> trams = new List<GameObject>();	// Holds all trams in the scene
    private float tetheredRingRadius;							// Calculated radius of ring
	private Constants.Configuration config;						// Holds reference to config file

    void Start() {
		config = Constants.Configuration.Instance;

		if (!TramRing)
		{
			tetheredRingRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
			print("ring1 radius: " + tetheredRingRadius);
			RefreshRing();
		}
		else
		{
			tetheredRingRadius = Mathf.Cos(config.RingLatitude * 1.025f * Mathf.PI / 180) / 2;
			print("ring2 radius: " + tetheredRingRadius);
			RefreshRing();
		}
    }

    public void RefreshRing() {
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
		float prevx = 0, prevy = 0, prevz = 0;

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
				prevx = x;
				prevy = y;
				prevz = z;

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

				if (TramRing && i >= trams.Count)
				{
					GameObject tram = Instantiate(Tram, transform);
					tram.transform.localPosition = new Vector3(x,y,z);
					tram.name = "Tram: " + trams.Count.ToString();
					tram.transform.localScale = new Vector3(6e-7f, 6e-7f, 6e-7f);
					tram.transform.LookAt(transform.TransformPoint(new Vector3(prevx, prevy, prevz)));

					Vector3 rot = Vector3.zero;
					rot.z = -125;
					tram.transform.GetChild(0).localRotation = Quaternion.Euler(rot);

					tram.SetActive(true);
					trams.Add(tram);
				}

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
		GameObject ring = new GameObject("ringx");
		MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
        Renderer renderer = GetComponent<MeshRenderer>();
        Material mat = Resources.Load("earthMat") as Material;
        renderer.material = mat;
    }

}
