using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TramCars : MonoBehaviour {

	[Tooltip("Totol keys is numSections * numKeysPerSectoin")]
	public int numSections = 10;												// Number of sections
	public int numKeysPerSection = 10;											// Total number of trams
	public int numTramsPerSection = 1;											// Number of trams per section
	public float habitatHeight = 0.00001f;										// Offset
	public GameObject train;													// Train car prefab

	private float torusRadius;													// Radius of torus ring
	private Constants.Configuration config;										// Holds reference to config file
	private List<GameObject> tramBottomRightObjects = new List<GameObject>();	// List of all bottom right trams
	private List<GameObject> tramBottomLeftObjects = new List<GameObject>();	// List of all bottom left trams
	private List<GameObject> tramTopLeftObjects = new List<GameObject>();		// List of all top left trams
	private List<GameObject> keysBottomRight = new List<GameObject>();			// List of all bottom right keys. Points for the bottom right trams to travel to
	private List<GameObject> keysBottomLeft = new List<GameObject>();			// List of all bottom left keys. Points for the bottom left trams to travel to
	private List<GameObject> keysTopLeft = new List<GameObject>();				// List of all top left keys. Points for the top left trams to travel to
	private Vector3 prevUp;														// Holds up position for first tram
	private int startIndex;														// What index the trams should start being created
	private int endIndex;														// What index the trams should stop being created

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		startIndex = 3;	// Set start/end index to only draw cars that can be seen by the user
		endIndex = 8;
		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;

		CreateTramSections();	// Create all trams and keys
		UpdatePositions();		// Move trams to proper positions
		UpdateTramKeys();		// Now that all trams and sections are created, set all the tram keys for their movement
		SetTramsActive();		// Activate all trams
		DeleteKeys();			// Delete keys as they are no longer needed
	}

	/// <summary>
	/// Call coroutine to set tram object active
	/// </summary>
	public void SetTramsActive()
	{
		ActivateTrams();
	}
		
	/// <summary>
	/// Set trams active over multiple frames
	/// </summary>
	/// <returns>The trams.</returns>
	private void ActivateTrams()
	{
		foreach(GameObject t in tramBottomRightObjects)
		{
			t.SetActive(true);
		}
			
		foreach(GameObject t in tramBottomLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
		}

		foreach(GameObject t in tramTopLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
		}
	}

	/// <summary>
	/// Delete all keys
	/// </summary>
	private void DeleteKeys()
	{
		foreach (GameObject k in keysTopLeft)
		{
			Destroy(k);
		}
		keysTopLeft.Clear();

		foreach (GameObject k in keysBottomRight)
		{
			Destroy(k);
		}
		keysBottomRight.Clear();

		foreach (GameObject k in keysBottomLeft)
		{
			Destroy(k);
		}
		keysBottomLeft.Clear();
	}

	/// <summary>
	/// Create a new tram section
	/// </summary>
	private void CreateTramSections()
	{
		bool createTram;
		int tramSpacing = (int)numKeysPerSection / numTramsPerSection;
		int numRingHabitats = numKeysPerSection * numSections;
		float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

		for (int instance = 0; instance < numSections; instance++)	// Iterate through the sections
		{
			for (int ringHabitatIndex = 0; ringHabitatIndex < numKeysPerSection; ringHabitatIndex++)	// Iterate through the keys and tram if there is one
			{
				// Decide if we should create a tram
				if (ringHabitatIndex % tramSpacing == 0  && instance >= startIndex && instance <= endIndex)
				{
					createTram = true;
				}
				else
				{
					createTram = false;
				}

				CreateTramsInSection(
					instance,
					ringHabitatIndex,
					ringHabitatSpacing,
					createTram);
			}
		}
	}

	/// <summary>
	/// Creates the trams in section.
	/// </summary>
	/// <param name="instance">Instance.</param>
	/// <param name="ringHabitatIndex">Ring habitat index.</param>
	/// <param name="ringHabitatSpacing">Ring habitat spacing.</param>
	/// <param name="createTram">If set to <c>true</c> create tram.</param>
	private void CreateTramsInSection(int instance, int ringHabitatIndex, float ringHabitatSpacing, bool createTram)
	{
		float theta;
		float phi0;
		float phi1;

		theta = (instance * numKeysPerSection + ringHabitatIndex) * ringHabitatSpacing;
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

		GameObject key = new GameObject("Key " + keysBottomRight.Count);
		key.transform.SetParent(transform);
		key.transform.localPosition = habtop;

		GameObject key1 = Instantiate(key, transform);
		GameObject key2 = Instantiate(key, transform);

		// If we have no other reference object to look at, save the up vector for later
		if (keysBottomRight.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		// Set orientation of all keys other than the first
		else if (keysBottomRight.Count > 0)
		{
			key.transform.LookAt(keysBottomRight[keysBottomRight.Count-1].transform.position, transform.TransformVector(habtop - habbot));
			key1.transform.LookAt(keysTopLeft[keysTopLeft.Count-1].transform.position, transform.TransformVector(habtop - habbot));
			key2.transform.LookAt(keysBottomLeft[keysBottomLeft.Count-1].transform.position, transform.TransformVector(habtop - habbot));
		}
			
		// Add keys to list to assign them to trams later
		keysBottomRight.Add(key);
		keysTopLeft.Add(key1);
		keysBottomLeft.Add(key2);

		// Set the first key and tram position and orientation
		if (keysBottomRight.Count == numSections * numKeysPerSection)
		{
			keysBottomRight[0].transform.LookAt(keysBottomRight[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));
			tramBottomRightObjects[0].transform.LookAt(keysBottomRight[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));

			keysTopLeft[0].transform.LookAt(keysTopLeft[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));
			tramTopLeftObjects[0].transform.LookAt(keysTopLeft[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));

			keysBottomLeft[0].transform.LookAt(keysBottomLeft[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));
			tramBottomLeftObjects[0].transform.LookAt(keysBottomLeft[startIndex * numKeysPerSection - 1].transform.position, transform.TransformVector(prevUp));
		}

		// Create a tram at an interval set by the user
		if (createTram)
		{
			GameObject tram = Instantiate(train, transform);
			tram.name = "Tram " + tramBottomRightObjects.Count;
			tram.transform.localPosition = habtop;
			tram.transform.localScale = new Vector3(6e-6f, 6e-6f, 6e-6f);

			GameObject tram1 = Instantiate(tram, transform);
			GameObject tram2 = Instantiate(tram, transform);

			// Set orientation of trams other than the first
			if (tramBottomRightObjects.Count > 0)
			{
				tram.transform.LookAt(keysBottomRight[keysBottomRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram1.transform.LookAt(keysTopLeft[keysBottomRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram2.transform.LookAt(keysBottomLeft[keysBottomRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));

			}
				
			// Add trams to list to assign their keys later
			tramBottomRightObjects.Add(tram);
			tramTopLeftObjects.Add(tram1);
			tramBottomLeftObjects.Add(tram2);

		}
	}

	/// <summary>
	/// Update positions of keys and trams
	/// </summary>
	private void UpdatePositions()
	{
		foreach (GameObject t in tramBottomRightObjects)
		{
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.right) * 9.5e-5f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 6e-5f;
		}

		foreach (GameObject k in keysBottomRight)
		{
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 9.5e-5f;
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 6e-5f;
		}

		foreach (GameObject t in tramTopLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1.2e-4f;
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.up) * 8e-5f;
		}

		foreach (GameObject k in keysTopLeft)
		{
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 1.2e-4f;
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 8e-5f;
		}

		foreach (GameObject t in tramBottomLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1.2e-4f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 2e-5f;
		}

		foreach (GameObject k in keysBottomLeft)
		{
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 1.2e-4f;
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 2e-5f;
		}
	}

	/// <summary>
	/// Set the keys to each tram
	/// </summary>
	private void UpdateTramKeys()
	{
		// Set the keys for each tram in the bottom right track
		for (int i=0; i<tramBottomRightObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = SortKeysRotations(i, keysBottomRight);
			tramBottomRightObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysBottomRight);
			tramBottomRightObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
		}

		// Set the keys for each tram in the bottom left track
		for (int i=0; i<tramBottomLeftObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = SortKeysRotations(i, keysBottomLeft);
			tramBottomLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysBottomLeft);
			tramBottomLeftObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
		}

		// Set the keys for each tram in the top left track
		for (int i=0; i<tramTopLeftObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = SortKeysRotations(i, keysTopLeft);
			tramTopLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysTopLeft);
			tramTopLeftObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
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
		index *= (int)numKeysPerSection / numTramsPerSection;
		index += startIndex * numKeysPerSection;

		for (int i=index; i>=0; i--)
		{
			sortedKeys.Add(keys[i].transform.localRotation);
		}
		for (int i=keys.Count-1; i>index; i--)
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
		index *= (int)numKeysPerSection / numTramsPerSection;
		index += startIndex * numKeysPerSection;

		for (int i=index; i>=0; i--)
		{
			sortedKeys.Add(keys[i].transform.localPosition);
		}
		for (int i=keys.Count-1; i>index; i--)
		{
			sortedKeys.Add(keys[i].transform.localPosition);
		}

		return sortedKeys;
	}
}