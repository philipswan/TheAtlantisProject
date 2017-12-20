using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TramCars : MonoBehaviour {

	public static TramCars Instance;
	[Tooltip("Totol keys is numSections * numKeysPerSectoin")]
	public int numSections = 10;												// Number of sections
	public int numKeysPerSection = 10;											// Total number of trams
	[Tooltip("Each tram per section includes 4 trams (1 for each track)." +
		" Total trams does not equal Num Sections * Num Trams Per Section because we only need to render what the user will see.")]
	public int numTramsPerSection = 1;											// Number of trams per section
	public float habitatHeight = 0.00001f;										// Offset
	public GameObject train;													// Train car prefab

	private float torusRadius;													// Radius of torus ring
	private float acceleration;
	private float topSpeed;
	private float travelTime;
	private float accelerationTime;
	private Constants.Configuration config;										// Holds reference to config file
	private Vector3 prevUp;														// Holds up position for first tram
	private int startIndex;														// What index the trams should start being created
	private int endIndex;														// What index the trams should stop being created
	private int habitatIndex;													// How many keys between habitats
	private int insertedKeys;

	private List<GameObject> tramBottomRightObjects = new List<GameObject>();	// List of all bottom right trams
	private List<GameObject> tramBottomLeftObjects = new List<GameObject>();	// List of all bottom left trams
	private List<GameObject> tramTopLeftObjects = new List<GameObject>();		// List of all top left trams
	private List<GameObject> tramTopRightObjects = new List<GameObject>();		// List of all top right trams
	private List<GameObject> keysBottomRight = new List<GameObject>();			// List of all bottom right keys. Points for the bottom right trams to travel to
	private List<GameObject> keysBottomLeft = new List<GameObject>();			// List of all bottom left keys. Points for the bottom left trams to travel to
	private List<GameObject> keysTopLeft = new List<GameObject>();				// List of all top left keys. Points for the top left trams to travel to
	private List<GameObject> keysTopRight = new List<GameObject>();				// List of all top right keys. Points for the top right trams to travel to

	private List<Material> onGazeEnterMaterials = new List<Material>();
	private Material onGazeExitMaterial;
	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		habitatIndex = (int)numKeysPerSection / RingHabitats.Instance.numRingHabitatsPerInstance;

		insertedKeys = 3;
		startIndex = 1;	// Set start/end section to only draw cars that can be seen by the user
		endIndex = 8;
		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;

		CreateTramSections();	// Create all trams and keys
		UpdatePositions();		// Move trams to proper positions
		UpdateTramKeys();		// Now that all trams and sections are created, set all the tram keys for their movement
		DeleteKeys();			// Delete keys as they are no longer needed

		FloatingMenu.Instance.AddItems(train, "Tram", new Vector3(1,1,1));
	}
		
	/// <summary>
	/// Set trams active over multiple frames
	/// </summary>
	/// <returns>The trams.</returns>
	public void ActivateTrams()
	{
		foreach(GameObject t in tramBottomRightObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime, insertedKeys);
		}

		foreach(GameObject t in tramTopRightObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
			t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime);
		}
			
		foreach(GameObject t in tramBottomLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
			t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime);
		}

		foreach(GameObject t in tramTopLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime, insertedKeys);
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

		foreach (GameObject k in keysTopRight)
		{
			Destroy(k);
		}
		keysTopRight.Clear();
	}

	/// <summary>
	/// Create a new tram section
	/// </summary>
	private void CreateTramSections()
	{
		bool createTram;
		int tramSpacing = (int)numKeysPerSection / numTramsPerSection;

		//  R of actual ring is 5,182,853 m
		//  R of tram ring is 0.41453875 (Sphere size (1, 1, 1))
		//  R of tram ring at scale 5000 is 2072 m
		//  Arc length = RC where C is the center angle
		// Tram top speed is 223.5 m/s
		// Tram acceleration is +/-11.7 m/s^2

		float radiusRing 	= 2072f;	// Radius of the tram ring
		float rpk 			= (2 * Mathf.PI) / (numSections * numKeysPerSection);	// Radians between keys
		float scaleRatio 	= 2072.0f / 5182853.0f;	// ratio of tram ring to actual ring (when the system is at scale 5000)
		float alKeys 		= radiusRing * rpk;	// arc length between keys in meters
		topSpeed 			= 223.5f * scaleRatio;	// top speed (500 mph) scaled to system
		acceleration 		= 11.7f * scaleRatio;	// acceleartion (1.2g) scaled to system
		travelTime 			= alKeys / topSpeed;
		accelerationTime 	= topSpeed / acceleration;

		// Print calculations
		print("Scale Ratio: " + scaleRatio);
		print("Ttotal keys: " + numSections * numKeysPerSection);
		print("rad per key: " + rpk);
		print("arc length between keys: " + alKeys + "m");
		print("Top speed: " + topSpeed + "m/s" + " acceleration: " + acceleration + "m/s^2");
		print("Top speed travel time: " + travelTime + "s");
		print("Acceleartion time: " + accelerationTime + "s");

		int numKeys = numKeysPerSection * numSections;
		float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numKeys;

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
		GameObject key3 = Instantiate(key, transform);

		// If we have no other reference object to look at, save the up vector for later
		if (keysBottomRight.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		// Set orientation of all keys other than the first
		else if (keysBottomRight.Count > 0)
		{
			key.transform.LookAt(keysBottomRight[keysBottomRight.Count-1].transform.position, transform.TransformVector(habtop - habbot));
			key2.transform.LookAt(keysBottomLeft[keysBottomLeft.Count-1].transform.position, transform.TransformVector(habtop - habbot));

			keysTopLeft[keysTopLeft.Count-1].transform.LookAt(key1.transform.position, transform.TransformVector(habtop - habbot));
			keysTopRight[keysTopRight.Count-1].transform.LookAt(key3.transform.position, transform.TransformVector(habtop - habbot));
		}
			
		// Add keys to list to assign them to trams later
		keysBottomRight.Add(key);
		keysTopLeft.Add(key1);
		keysBottomLeft.Add(key2);
		keysTopRight.Add(key3);

		// Set the first key and tram position and orientation
		if (keysBottomRight.Count == numSections * numKeysPerSection)
		{
			keysBottomRight[0].transform.LookAt(keysBottomRight[keysBottomRight.Count - 1].transform.position, transform.TransformVector(prevUp));
			tramBottomRightObjects[0].transform.LookAt(keysBottomRight[keysBottomRight.Count - 1].transform.position, transform.TransformVector(prevUp));

			keysBottomLeft[0].transform.LookAt(keysBottomLeft[keysBottomLeft.Count - 1].transform.position, transform.TransformVector(prevUp));
			tramBottomLeftObjects[0].transform.LookAt(keysBottomLeft[keysBottomLeft.Count - 1].transform.position, transform.TransformVector(prevUp));
		
			key1.transform.LookAt(keysTopLeft[0].transform.position, transform.TransformVector(prevUp));
			tramTopLeftObjects[tramTopLeftObjects.Count-1].transform.LookAt(keysTopLeft[0].transform.position, transform.TransformVector(prevUp));

			key3.transform.LookAt(keysTopRight[0].transform.position, transform.TransformVector(prevUp));
			tramTopRightObjects[tramTopRightObjects.Count-1].transform.LookAt(keysTopRight[0].transform.position, transform.TransformVector(prevUp));
		}

		// Create a tram at an interval set by the user
		if (createTram)
		{
			GameObject tram = Instantiate(train, transform);
			tram.name = "Bottom Right Tram " + tramBottomRightObjects.Count;
			tram.transform.localPosition = habtop;
			tram.transform.localScale = new Vector3(6e-6f, 6e-6f, 6e-6f);

			GameObject tram1 = Instantiate(tram, transform);
			tram1.name = "Top Left Tram " + tramTopLeftObjects.Count;

			GameObject tram2 = Instantiate(tram, transform);
			tram2.name = "Bottom Left Tram " + tramBottomLeftObjects.Count;

			GameObject tram3 = Instantiate(tram, transform);
			tram3.name = "Top Right Tram " + tramTopRightObjects.Count;

			// Set orientation of trams other than the first
			if (tramBottomRightObjects.Count > 0)
			{
				tram.transform.LookAt(keysBottomRight[keysBottomRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram2.transform.LookAt(keysBottomLeft[keysBottomLeft.Count-2].transform.position, transform.TransformVector(habtop - habbot));

				tramTopLeftObjects[tramTopLeftObjects.Count-1].transform.LookAt(keysTopLeft[keysTopLeft.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tramTopRightObjects[tramTopRightObjects.Count-1].transform.LookAt(keysTopRight[keysTopRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
			}
				
			// Add trams to list to assign their keys later
			tramBottomRightObjects.Add(tram);
			tramTopLeftObjects.Add(tram1);
			tramBottomLeftObjects.Add(tram2);
			tramTopRightObjects.Add(tram3);
		}
	}

	private void SetTramMaterials(GameObject tram)
	{
		MeshRenderer mr = tram.GetComponent<MeshRenderer>();
		List<Material> materials = new List<Material>();
		materials.Add(mr.material);
		materials.Add(Resources.Load("Outline Diffuse") as Material);
		mr.materials = materials.ToArray();
		//mr.materials[1].SetFloat("_Outline", 0);
	}

	/// <summary>
	/// Update positions of keys and trams
	/// </summary>
	private void UpdatePositions()
	{
		// Adjust bottom right trams
		foreach (GameObject t in tramBottomRightObjects)
		{
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.right) * 9.5e-5f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 6e-5f;
		}

		// Adjust bottom right keys
		foreach (GameObject k in keysBottomRight)
		{
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 9.5e-5f;
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 6e-5f;
		}

		// Adjust top right trams
		foreach (GameObject t in tramTopRightObjects)
		{
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.right) * 1.2e-4f;
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.up) * 3e-5f;
		}

		// Adjust top right keys
		foreach (GameObject k in keysTopRight)
		{
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 1.2e-4f;
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 3e-5f;
		}

		// Adjust bottom left trams
		foreach (GameObject t in tramBottomLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1e-4f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 1e-5f;
		}

		// Adjust bottom left keys
		foreach (GameObject k in keysBottomLeft)
		{
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 1e-4f;
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 1e-5f;
		}

		// Adjust top left trams
		foreach (GameObject t in tramTopLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1e-4f;
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.up) * 9e-5f;
		}

		// Adjust top left keys
		foreach (GameObject k in keysTopLeft)
		{
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 1e-4f;
			k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 9e-5f;
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
			//sortedRotations = InsertRotations(sortedRotations, insertedKeys);
			tramBottomRightObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysBottomRight);
			//sortedPositions = InsertPositions(sortedPositions, insertedKeys);
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
			List<Quaternion> sortedRotations = ReverseSortKeysRotations(i, keysTopLeft);
			//sortedRotations = InsertRotations(sortedRotations, insertedKeys);
			tramTopLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = ReverseSortKeysPositions(i, keysTopLeft);
			//sortedPositions = InsertPositions(sortedPositions, insertedKeys);
			tramTopLeftObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
		}

		// Set the keys for each tram in the top right track
		for (int i=0; i<tramTopRightObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = ReverseSortKeysRotations(i, keysTopRight);
			tramTopRightObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = ReverseSortKeysPositions(i, keysTopRight);
			tramTopRightObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
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

	/// <summary>
	/// Sorts the Gameobject keys in reverse order
	/// </summary>
	/// <returns>Gameobjects in order.</returns>
	/// <param name="index">Index.</param>
	/// <param name="keys">Keys.</param>
	private List<Quaternion> ReverseSortKeysRotations(int index, List<GameObject> keys)
	{
		List<Quaternion> sortedKeys = new List<Quaternion>();
		index *= (int)numKeysPerSection / numTramsPerSection;
		index += startIndex * numKeysPerSection;

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
	/// Sorts the keys in reverse order
	/// </summary>
	/// <returns>Vector3 local positions in order.</returns>
	/// <param name="index">Index.</param>
	/// <param name="keys">Keys.</param>
	private List<Vector3> ReverseSortKeysPositions(int index, List<GameObject> keys)
	{
		List<Vector3> sortedKeys = new List<Vector3>();
		index *= (int)numKeysPerSection / numTramsPerSection;
		index += startIndex * numKeysPerSection;

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

	/// <summary>
	/// Insert extra positions at habitat indexes
	/// </summary>
	/// <returns>The positions.</returns>
	/// <param name="positions">Positions.</param>
	/// <param name="insertedKeys">Inserted keys.</param>
	private List<Vector3> InsertPositions(List<Vector3> positions, int insertedKeys)
	{
		List<Vector3> newPos = new List<Vector3>();

		for (int i=0; i<positions.Count; i++)
		{
			newPos.Add(positions[i]);
			if (i % habitatIndex == 0)
			{
				for (int j=0; j<insertedKeys; j++)
				{
					newPos.Add(positions[i]);
				}
			}
		}

		return newPos;
	}

	/// <summary>
	/// Insert extra rotations at habitat indexes
	/// </summary>
	/// <returns>The rotations.</returns>
	/// <param name="rotations">Rotations.</param>
	/// <param name="insertedKeys">Inserted keys.</param>
	private List<Quaternion> InsertRotations(List<Quaternion> rotations, int insertedKeys)
	{
		List<Quaternion> newRot = new List<Quaternion>();

		for (int i=0; i<rotations.Count; i++)
		{
			newRot.Add(rotations[i]);
			if (i % habitatIndex == 0)
			{
				for (int j=0; j<insertedKeys; j++)
				{
					newRot.Add(rotations[i]);
				}
			}
		}

		return newRot;
	}
}