using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrainTracks : MonoBehaviour {

	[Tooltip("Totol keys is numKeys * numTrams")]
	public int numKeys = 10;
	public int numTrams = 10;
	public int numTubeSides = 8;
	public float tubeRadius = .0001f;
	public float habitatHeight = .00001f;
	public GameObject trainTrack;

	private float torusRadius;
	private Constants.Configuration config;									// Holds reference to config file
	private List<GameObject> trackObjects = new List<GameObject>();			// List of all trams
	private Vector3 prevUp;													// Holds up position for first tram

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * 1.025f * Mathf.PI / 180) / 2;
		RefreshRingHabitats();
	}

	public void RefreshRingHabitats()
	{
		int numRingHabitats = numTrams * numKeys;
		float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

		for (int instance = 0; instance < numKeys; instance++)
		{
			for (int ringHabitatIndex = 0; ringHabitatIndex < numTrams; ringHabitatIndex++)
			{
				NewRingHabitat(
					instance,
					ringHabitatIndex,
					ringHabitatSpacing);
			}
		}

		foreach (GameObject t in trackObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 3.5e-6f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1e-5f;
		}
	}

	public void NewRingHabitat(int instance, int ringHabitatIndex, float ringHabitatSpacing)
	{
		float theta;
		float phi0;
		float phi1;

		theta = (instance * numTrams + ringHabitatIndex) * ringHabitatSpacing;
		phi0 = torusRadius;
		phi1 = torusRadius - habitatHeight * Mathf.Cos(config.RingLatitude * Mathf.Deg2Rad);

		// Find the current and next segments
		Vector3 habtop, habbot, hableft;

		habtop.x = phi0 * Mathf.Cos(theta) - torusRadius;
		habtop.z = phi0 * Mathf.Sin(theta);
		habtop.y = -Mathf.Pow(torusRadius - phi0, 1.7f) * 10;

		habbot.x = phi1 * Mathf.Cos(theta) - torusRadius;
		habbot.z = phi1 * Mathf.Sin(theta);
		habbot.y = -habitatHeight * Mathf.Sin(config.RingLatitude * Mathf.Deg2Rad);

		hableft.x = phi0 * Mathf.Cos(theta) - torusRadius;
		hableft.z = phi0 * Mathf.Sin(theta);
		hableft.y = -0.0075f * Mathf.Sin(config.RingLatitude * Mathf.Deg2Rad);
			
		GameObject track = Instantiate(trainTrack, transform);
		track.SetActive(true);
		track.name = "Track " + trackObjects.Count;
		track.transform.localPosition = habtop;
		track.transform.localScale = new Vector3(2.5e-8f, 2.5e-8f, 8.28e-7f);

		if (trackObjects.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		else if (trackObjects.Count > 0)
		{
			track.transform.LookAt(trackObjects[trackObjects.Count-1].transform.position, transform.TransformVector(habtop - habbot));
		}

		trackObjects.Add(track);

		if (trackObjects.Count == numKeys * numTrams)
		{
			trackObjects[0].transform.LookAt(track.transform.position, transform.TransformVector(prevUp));
		}
	}
}