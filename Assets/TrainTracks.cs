using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrainTracks : MonoBehaviour {

	public GameObject trainTrack;												// Train track prefab

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

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * 1.025f * Mathf.PI / 180) / 2;
	}

	/// <summary>
	/// Creates the track sections.
	/// </summary>
	public void CreateTrackSections()
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
			trackBottomLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomLeftObjects[i].transform.up) * 3.5e-6f;
			trackBottomLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomLeftObjects[i].transform.right) * 1e-5f;

			trackTopLeftObjects[i].transform.localPosition += transform.InverseTransformPoint(trackTopLeftObjects[i].transform.up) * 3.5e-6f;
			trackTopLeftObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackTopLeftObjects[i].transform.right) * 1e-5f;

			trackBottomRightObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackBottomRightObjects[i].transform.up) * 9e-6f;
			trackBottomRightObjects[i].transform.localPosition += transform.InverseTransformPoint(trackBottomRightObjects[i].transform.right) * 1e-5f;

			trackTopRightObjects[i].transform.localPosition -= transform.InverseTransformPoint(trackTopRightObjects[i].transform.up) *  2e-6f;
			trackTopRightObjects[i].transform.localPosition += transform.InverseTransformPoint(trackTopRightObjects[i].transform.right) * 1e-5f;
		}
	}

	/// <summary>
	/// Creates the tracks in section.
	/// </summary>
	/// <param name="instance">Instance.</param>
	/// <param name="ringHabitatIndex">Ring habitat index.</param>
	/// <param name="ringHabitatSpacing">Ring habitat spacing.</param>
	public void CreateTracksInSection(int instance, int ringHabitatIndex, float ringHabitatSpacing)
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
			
		GameObject track = Instantiate(trainTrack, transform);
		track.SetActive(true);
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
			
		GameObject track1 = Instantiate(track, transform);
		GameObject track2 = Instantiate(track, transform);
		GameObject track3 = Instantiate(track, transform);

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