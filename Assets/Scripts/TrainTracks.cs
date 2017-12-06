using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrainTracks : MonoBehaviour {

	private int numSections = 100;												// Number of track sections
	private int numTracks = 100;												// Number of tracks per sectoin
	private float habitatHeight = 0.00001f;										// Offset
	private float torusRadius;													// Radius of torus ring
	private Constants.Configuration config;										// Holds reference to config file
	private List<GameObject> trackBottomLeftObjects = new List<GameObject>();	// List of trams for bottom left pos
	private List<GameObject> trackTopLeftObjects = new List<GameObject>();		// List of trams for top left pos
	private List<GameObject> trackBottomRightObjects = new List<GameObject>();	// List of trams for bottom right pos
	private List<GameObject> trackTopRightObjects = new List<GameObject>();		// List of trams for top right pos
	private Vector3 prevUp;														// Holds up position for first tram

	private GameObject bottomLeft;												// Container for bottom left line and positions
	private GameObject bottomRight;												// Container for bottom right line and positions
	private GameObject topLeft;													// Container for top left line and positions
	private GameObject topRight;												// Container for top right line and positions

	// Use this for initialization
	void Start()
	{
		// Create containers for the four tracks
		bottomLeft = new GameObject("Bottom Left Keys");
		bottomLeft.transform.SetParent(transform);
		bottomLeft.transform.localPosition = Vector3.zero;
		bottomLeft.transform.localRotation = transform.localRotation;
		bottomLeft.transform.localScale = transform.localScale;

		bottomRight = new GameObject("Bottom Right Keys");
		bottomRight.transform.SetParent(transform);
		bottomRight.transform.localPosition = Vector3.zero;
		bottomRight.transform.localRotation = transform.localRotation;
		bottomRight.transform.localScale = transform.localScale;

		topLeft = new GameObject("Top Left Keys"); 
		topLeft.transform.SetParent(transform);
		topLeft.transform.localPosition = Vector3.zero;
		topLeft.transform.localRotation = transform.localRotation;
		topLeft.transform.localScale = transform.localScale;

		topRight = new GameObject("Top Right Keys"); 
		topRight.transform.SetParent(transform);
		topRight.transform.localPosition = Vector3.zero;
		topRight.transform.localRotation = transform.localRotation;
		topRight.transform.localScale = transform.localScale;

		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;
		CreateTrackSections();
		SetTracksActive();
		DeleteKeys();
	}

	/// <summary>
	/// Call coroutine to draw lines
	/// </summary>
	public void SetTracksActive()
	{
		//StartCoroutine("ActivateTracks");
		ActivateTracks();
	}

	/// <summary>
	/// Draw four tracks as line. Split work over multiple framess
	/// </summary>
	/// <returns>The tracks.</returns>
	private void ActivateTracks()
	{
		// Create line renderer and set properties
		// Add bottom left line
		bottomLeft.AddComponent<LineRenderer>();
		LineRenderer lineRenderer = bottomLeft.GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = false;
		lineRenderer.positionCount = trackBottomLeftObjects.Count;
		lineRenderer.widthMultiplier = 0.001f;
		lineRenderer.loop = true;
		lineRenderer.material = Resources.Load("Ring") as Material;

		// Get all positions for line
		Vector3[] linePos = new Vector3[trackBottomLeftObjects.Count];
		for (int i=0; i<trackBottomLeftObjects.Count; i++)
		{
			linePos[i] = trackBottomLeftObjects[i].transform.localPosition;
		}
			
		// Set positions
		lineRenderer.SetPositions(linePos);

		// Add bottom right line
		bottomRight.AddComponent<LineRenderer>();
		LineRenderer lineRenderer1 = bottomRight.GetComponent<LineRenderer>();
		lineRenderer1.useWorldSpace = false;
		lineRenderer1.positionCount = trackBottomRightObjects.Count;
		lineRenderer1.widthMultiplier = 0.001f;
		lineRenderer1.loop = true;
		lineRenderer1.material = Resources.Load("Ring") as Material;

		Vector3[] linePos1 = new Vector3[trackBottomRightObjects.Count];
		for (int i=0; i<trackBottomRightObjects.Count; i++)
		{
			linePos1[i] = trackBottomRightObjects[i].transform.localPosition;
		}
			
		lineRenderer1.SetPositions(linePos1);

		// Add top left line
		topLeft.AddComponent<LineRenderer>();
		LineRenderer lineRenderer2 = topLeft.GetComponent<LineRenderer>();
		lineRenderer2.useWorldSpace = false;
		lineRenderer2.positionCount = trackTopLeftObjects.Count;
		lineRenderer2.widthMultiplier = 0.001f;
		lineRenderer2.loop = true;
		lineRenderer2.material = Resources.Load("Ring") as Material;

		Vector3[] linePos2 = new Vector3[trackTopLeftObjects.Count];
		for (int i=0; i<trackTopLeftObjects.Count; i++)
		{
			linePos2[i] = trackTopLeftObjects[i].transform.localPosition;
		}
			
		lineRenderer2.SetPositions(linePos2);

		// Add top right line
		topRight.AddComponent<LineRenderer>();
		LineRenderer lineRenderer3 = topRight.GetComponent<LineRenderer>();
		lineRenderer3.useWorldSpace = false;
		lineRenderer3.positionCount = trackTopRightObjects.Count;
		lineRenderer3.widthMultiplier = 0.001f;
		lineRenderer3.loop = true;
		lineRenderer3.material = Resources.Load("Ring") as Material;

		Vector3[] linePos3 = new Vector3[trackTopRightObjects.Count];
		for (int i=0; i<trackTopRightObjects.Count; i++)
		{
			linePos3[i] = trackTopRightObjects[i].transform.localPosition;
		}
			
		lineRenderer3.SetPositions(linePos3);
	}

	/// <summary>
	/// Delete all keys
	/// </summary>
	private void DeleteKeys()
	{
		foreach (GameObject k in trackBottomLeftObjects)
		{
			Destroy(k);
		}
		trackBottomLeftObjects.Clear();

		foreach (GameObject k in trackBottomRightObjects)
		{
			Destroy(k);
		}
		trackBottomRightObjects.Clear();

		foreach (GameObject k in trackTopLeftObjects)
		{
			Destroy(k);
		}
		trackTopLeftObjects.Clear();

		foreach (GameObject k in trackTopRightObjects)
		{
			Destroy(k);
		}
		trackTopRightObjects.Clear();
	}

	/// <summary>
	/// Creates the track sections.
	/// </summary>
	private void CreateTrackSections()
	{
		int numRingHabitats = numTracks * numSections;
		float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

		for (int instance = 0; instance < numSections; instance++)	// Iterate through the sections
		{
			for (int ringHabitatIndex = 0; ringHabitatIndex < numTracks; ringHabitatIndex++)	// Iterate throug the tracks
			{
				CreateTracksInSection(
					instance,
					ringHabitatIndex,
					ringHabitatSpacing);
			}
		}

		// Adjust positions of the tracks
		for (int i=0; i<trackBottomLeftObjects.Count; i++)
		{
			trackBottomLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomLeftObjects[i].transform.up) * 3.5e-5f;
			trackBottomLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomLeftObjects[i].transform.right) * 1e-4f;

			trackTopLeftObjects[i].transform.localPosition += transform.InverseTransformPoint(trackTopLeftObjects[i].transform.up) * 6.5e-5f;
			trackTopLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackTopLeftObjects[i].transform.right) * 1e-4f;

			trackBottomRightObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomRightObjects[i].transform.up) * 9e-5f;
			trackBottomRightObjects[i].transform.localPosition += transform.InverseTransformPoint(trackBottomRightObjects[i].transform.right) * 1e-4f;

			trackTopRightObjects[i].transform.localPosition += transform.InverseTransformPoint(trackTopRightObjects[i].transform.up) *  2e-5f;
			trackTopRightObjects[i].transform.localPosition += transform.InverseTransformPoint(trackTopRightObjects[i].transform.right) * 8e-5f;
		}
	}

	/// <summary>
	/// Creates the tracks in section.
	/// </summary>
	/// <param name="instance">Instance.</param>
	/// <param name="ringHabitatIndex">Ring habitat index.</param>
	/// <param name="ringHabitatSpacing">Ring habitat spacing.</param>
	private void CreateTracksInSection(int instance, int ringHabitatIndex, float ringHabitatSpacing)
	{
		float theta;
		float phi0;
		float phi1;

		theta = (instance * numTracks + ringHabitatIndex) * ringHabitatSpacing;
		phi0 = torusRadius;
		phi1 = torusRadius - habitatHeight * Mathf.Cos(config.RingLatitude * Mathf.Deg2Rad);

		// Find the current and next segments
		Vector3 habtop, habbot;

		habtop.x = phi0 * Mathf.Cos(theta) - torusRadius;
		habtop.z = phi0 * Mathf.Sin(theta);
		habtop.y = -Mathf.Pow(torusRadius - phi0, 1.7f) * 10;

		habbot.x = phi1 * Mathf.Cos(theta) - torusRadius;
		habbot.z = phi1 * Mathf.Sin(theta);
		habbot.y = -habitatHeight * Mathf.Sin(config.RingLatitude * Mathf.Deg2Rad);
			
		GameObject track = new GameObject();
		track.transform.SetParent(bottomLeft.transform);
		track.name = "Track " + trackBottomLeftObjects.Count;
		track.transform.localPosition = habtop;
		track.transform.localScale = new Vector3(2.5e-8f, 2.5e-8f, 8.28e-7f);

		// If we have no reference track to look at, save the up vector for later
		if (trackBottomLeftObjects.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		// Orient all tracks other than the first
		else if (trackBottomLeftObjects.Count > 0)
		{
			track.transform.LookAt(trackBottomLeftObjects[trackBottomLeftObjects.Count-1].transform.position, transform.TransformVector(habtop - habbot));
		}
			
		GameObject track1 = Instantiate(track, topLeft.transform);
		GameObject track2 = Instantiate(track, bottomRight.transform);
		GameObject track3 = Instantiate(track, topRight.transform);

		// Add to list to fix position later
		trackBottomLeftObjects.Add(track);
		trackTopLeftObjects.Add(track1);
		trackBottomRightObjects.Add(track2);
		trackTopRightObjects.Add(track3);

		// Orient first track to last track
		if (trackBottomLeftObjects.Count == numSections * numTracks)
		{
			trackBottomLeftObjects[0].transform.LookAt(track.transform.position, transform.TransformVector(prevUp));
			trackTopLeftObjects[0].transform.LookAt(track.transform.position, transform.TransformVector(prevUp));

			trackBottomRightObjects[0].transform.LookAt(track.transform.position, transform.TransformVector(prevUp));
			trackTopRightObjects[0].transform.LookAt(track.transform.position, transform.TransformVector(prevUp));
		}
	}
}