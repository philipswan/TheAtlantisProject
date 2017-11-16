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
	[Tooltip("Set true if this script is attached to the tram ring object. This will place the ring at a lower position.")]
	public bool TramRing;
	public GameObject Tram;

	private List<GameObject> tramsTop = new List<GameObject>();	// Holds all top trams in the scene
	private List<GameObject> tramsBot = new List<GameObject>();	// Holds all bottom trams in the scene
	private List<GameObject> keysTop = new List<GameObject>();	// Holds all reference points for top tram motion
	private List<GameObject> keysBot = new List<GameObject>();	// Holds all reference points for bottom tram motion
	private float tetheredRingRadius;							// Calculated radius of ring
	private Constants.Configuration config;						// Holds reference to config file

    void Start() {
		config = Constants.Configuration.Instance;

		if (!TramRing)
		{
			tetheredRingRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
			RefreshRing();
		}
		else
		{
			tetheredRingRadius = Mathf.Cos(config.RingLatitude * 1.025f * Mathf.PI / 180) / 2;
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
		float upx = 0, upy = 0, upz = 0;

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

				if (TramRing && j == 0 && prevx != 0.0f)
				{
					// Add tram for top track
					GameObject tramTop = Instantiate(Tram, transform);
					tramTop.transform.localPosition = new Vector3(x,y,z) + transform.forward * 3e-6f + transform.up * 3e-6f;
					tramTop.name = "Top Tram " + tramsTop.Count.ToString();
					tramTop.transform.localScale = new Vector3(4e-7f, 4e-7f, 4e-7f);
					tramTop.transform.LookAt(transform.TransformPoint(new Vector3(prevx, prevy, prevz)));

					Vector3 rot = Vector3.zero;
					rot.x = -2.716f;
					rot.y = 178.277f;
					rot.z = -233.825f;
					tramTop.transform.GetChild(0).localRotation = Quaternion.Euler(rot);

					tramTop.SetActive(true);
					tramsTop.Add(tramTop);

					// Add key for top track
					GameObject keyTop = new GameObject("key " + keysTop.Count);
					keyTop.transform.SetParent(transform);
					keyTop.transform.localPosition = tramTop.transform.localPosition;
					keyTop.transform.localRotation = tramTop.transform.localRotation;
					keysTop.Add(keyTop);

					// Add tram for bottom track
					GameObject tramBot = Instantiate(Tram, transform);
					tramBot.transform.localPosition = new Vector3(x,y,z) - Vector3.right * 3e-6f;
					tramBot.name = "Bottom Tram " + tramsBot.Count.ToString();
					tramBot.transform.localScale = new Vector3(4e-7f, 4e-7f, 4e-7f);
					tramBot.transform.LookAt(transform.TransformPoint(new Vector3(prevx, prevy, prevz)));

					tramBot.transform.GetChild(0).localRotation = Quaternion.Euler(rot);

					tramBot.SetActive(true);
					tramsBot.Add(tramBot);

					// Add key for bottom tram
					GameObject keyBot = new GameObject("key " + keysBot.Count);
					keyBot.transform.SetParent(transform);
					keyBot.transform.localPosition = tramBot.transform.localPosition;
					keyBot.transform.localRotation = tramBot.transform.localRotation;
					keysBot.Add(keyBot);
				}
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

		GameObject ring = new GameObject(TramRing ? "Tram Ring" : "Ring");
		ring.transform.SetParent(transform);

		MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;

        Renderer renderer = GetComponent<MeshRenderer>();
        Material mat = Resources.Load("earthMat") as Material;
        renderer.material = mat;
    
		UpdateTramKeys();
	}


	/// <summary>
	/// Updates the tram keys.
	/// </summary>
	private void UpdateTramKeys()
	{
		// Set the keys for each tram in the top track
		for (int i=0; i<tramsTop.Count; i++)
		{
			List<Quaternion> sortedKeys = SortKeysRotations(i, keysTop);
			foreach(Quaternion k in sortedKeys)
			{
				tramsTop[i].GetComponent<TramMotion>().AddRotation(k);
			}

			List<Vector3> sortedPositions = SortKeysPositions(i, keysTop);
			foreach(Vector3 k in sortedPositions)
			{
				tramsTop[i].GetComponent<TramMotion>().AddPosition(k);
			}
		}

		// Set the keys for each tram in the bottom track
		for (int i=0; i<tramsBot.Count; i++)
		{
			List<Quaternion> sortedKeys = SortKeysRotations(i, keysBot);
			foreach(Quaternion k in sortedKeys)
			{
				tramsBot[i].GetComponent<TramMotion>().AddRotation(k);
			}

			List<Vector3> sortedPositions = SortKeysPositions(i, keysBot);
			foreach(Vector3 k in sortedPositions)
			{
				tramsBot[i].GetComponent<TramMotion>().AddPosition(k);
			}
		}
	}

	/// <summary>
	/// Sorts the Gameobject keys.
	/// </summary>
	/// <returns>Gameobjects in order.</returns>
	/// <param name="index">Index.</param>
	/// <param name="keys">Keys.</param>
	private List<Quaternion> SortKeysRotations(int index, List<GameObject> keys)
	{
		List<Quaternion> sortedKeys = new List<Quaternion>();

		for (int i=index; i<keys.Count; i++)
		{
			sortedKeys.Add(keys[i].transform.localRotation);
		}
		for (int i=0; i<index; i++)
		{
			sortedKeys.Add(keys[i].transform.localRotation);
		}

		return sortedKeys;
	}

	/// <summary>
	/// Sorts the keys.
	/// </summary>
	/// <returns>Vector3 local positions in order.</returns>
	/// <param name="index">Index.</param>
	/// <param name="keys">Keys.</param>
	private List<Vector3> SortKeysPositions(int index, List<GameObject> keys)
	{
		List<Vector3> sortedKeys = new List<Vector3>();

		for (int i=index; i<keys.Count; i++)
		{
			sortedKeys.Add(keys[i].transform.localPosition);
		}
		for (int i=0; i<index; i++)
		{
			sortedKeys.Add(keys[i].transform.localPosition);
		}
			
		return sortedKeys;
	}
}
