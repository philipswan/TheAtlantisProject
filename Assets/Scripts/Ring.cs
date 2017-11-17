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
	public GameObject Track;

	private List<GameObject> tramsTop = new List<GameObject>();		// Holds all top trams in the scene
	private List<GameObject> tramsBot = new List<GameObject>();		// Holds all bottom trams in the scene
	private List<GameObject> tracksTop = new List<GameObject>();	// Holds all top tracks in the scene
	private List<GameObject> tracksBot = new List<GameObject>();	// Holds all bottom tracks in the scene
	private List<GameObject> keysTop = new List<GameObject>();		// Holds all reference points for top tram motion
	private List<GameObject> keysBot = new List<GameObject>();		// Holds all reference points for bottom tram motion
	private float tetheredRingRadius;								// Calculated radius of ring
	private Constants.Configuration config;							// Holds reference to config file

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

//				if (j == 22)
//				{
//					upx = x;
//					upy = y;
//					upz = z;
//				}

//				if (j==22)
//				{
//					GameObject test = new GameObject("point " + j);
//					test.transform.SetParent(transform);
//					test.transform.localPosition = new Vector3(x,y,z);
//				}

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

				if (i == 0 && j == 9)
				{
					prevx = x;
					prevy = y;
					prevz = z;
				}
				else if (TramRing && j == 9 
					&& prevx != 0.0f
					&& i != 0)
				{
					upx = (tetheredRingRadius + TubeRadius * Mathf.Cos(20 * tubeSize)) * Mathf.Cos(angle) - tetheredRingRadius;
					upz = (tetheredRingRadius + TubeRadius * Mathf.Cos(20 * tubeSize)) * Mathf.Sin(angle);
					upy = TubeRadius * Mathf.Sin(20 * tubeSize);

					Vector3 position = new Vector3(x, y, z);
					Vector3 previousPosition = new Vector3(prevx, prevy, prevz);
					Vector3 upPosition = new Vector3(upx, upy, upz);

					SetTop(position, previousPosition, upPosition);
					SetBottom(position, previousPosition, upPosition);

					prevx = x;
					prevy = y;
					prevz = z;
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
    
		if (TramRing)
		{
			UpdateTramKeys();
		}
	}

	private void SetTop(Vector3 pos, Vector3 prevPos, Vector3 upPos)
	{
		// Add tram for top track
		GameObject tramTop = Instantiate(Tram, transform);
		tramTop.transform.localPosition = pos;
		tramTop.transform.position -= tramTop.transform.up * 12e-6f;
		tramTop.name = "Top Tram " + tramsTop.Count.ToString();
		tramTop.transform.localScale = new Vector3(4e-7f, 4e-7f, 4e-7f);
		tramTop.transform.LookAt(transform.TransformPoint(prevPos),
			-transform.TransformPoint(upPos));

		tramTop.SetActive(true);
		tramsTop.Add(tramTop);

		// Add top tracks
		GameObject trackTop = Instantiate(Track, transform);
		trackTop.transform.position = tramTop.transform.position;
		trackTop.transform.position -= tramTop.transform.up * 3e-6f;
		trackTop.name = "Top Track " + tramsTop.Count.ToString();
		trackTop.transform.localScale = new Vector3(1e-8f, 1e-8f, 4e-7f);
		trackTop.transform.localRotation = tramTop.transform.localRotation;

		trackTop.SetActive(true);
		tracksTop.Add(tramTop);

		// Add key for top track
		GameObject keyTop = new GameObject("key " + keysTop.Count);
		keyTop.transform.SetParent(transform);
		keyTop.transform.localPosition = tramTop.transform.localPosition;
		keyTop.transform.localRotation = tramTop.transform.localRotation;
		keysTop.Add(keyTop);
	}

	private void SetBottom(Vector3 pos, Vector3 prevPos, Vector3 upPos)
	{
		// Add tram for bottom track
		GameObject tramBot = Instantiate(Tram, transform);
		tramBot.transform.localPosition = pos;
		tramBot.transform.position -= tramBot.transform.up * 3e-6f;
		tramBot.name = "Bottom Tram " + tramsBot.Count.ToString();
		tramBot.transform.localScale = new Vector3(4e-7f, 4e-7f, 4e-7f);
		tramBot.transform.LookAt(transform.TransformPoint(prevPos),
			-transform.TransformPoint(upPos));

		tramBot.SetActive(true);
		tramsBot.Add(tramBot);

		// Add bottom track
		GameObject trackBot = Instantiate(Track, transform);
		trackBot.transform.position = tramBot.transform.position;
		trackBot.transform.position -= tramBot.transform.up * 3e-6f;
		trackBot.name = "Bottom Track" + tramsTop.Count.ToString();
		trackBot.transform.localScale = new Vector3(1e-8f, 1e-8f, 4e-7f);
		trackBot.transform.localRotation = tramBot.transform.localRotation;

		trackBot.SetActive(true);
		tracksBot.Add(trackBot);

		// Add key for bottom tram
		GameObject keyBot = new GameObject("key " + keysBot.Count);
		keyBot.transform.SetParent(transform);
		keyBot.transform.localPosition = tramBot.transform.localPosition;
		keyBot.transform.localRotation = tramBot.transform.localRotation;
		keysBot.Add(keyBot);
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
				bool lastItem = sortedPositions.IndexOf(k) == sortedPositions.Count-1 ? true : false;
				tramsTop[i].GetComponent<TramMotion>().AddPosition(k, lastItem);
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
				bool lastItem = sortedPositions.IndexOf(k) == sortedPositions.Count-1 ? true : false;
				tramsBot[i].GetComponent<TramMotion>().AddPosition(k, lastItem);
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
