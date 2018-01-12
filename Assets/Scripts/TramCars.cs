using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TramCars : MonoBehaviour {

    #region Public Properties
    public static TramCars Instance;
	[Tooltip("Totol keys is numSections * numKeysPerSectoin")]
	public int numSections = 100;												// Number of sections
	public int numKeysPerSection = 50;											// Total number of trams
	[Tooltip("Each tram per section includes 4 trams (1 for each track)." +
		" Total trams does not equal Num Sections * Num Trams Per Section because we only need to render what the user will see.")]
	public int numTramsPerSection = 5;											// Number of trams per section
	public float habitatHeight = 0.00001f;										// Offset
	public GameObject train;                                                    // Train car prefab
    #endregion

    #region Private Properties
    private float torusRadius;													// Radius of torus ring
	private float travelTime;                                                   // Time it takes to travel from one key to the next (s)
	private float accelerationTime;                                             // Time it takes to accelerate (s)
	private Constants.Configuration config;										// Holds reference to config file
	private Vector3 prevUp;														// Holds up position for first tram
	private int startIndex;														// What index the trams should start being created
	private int endIndex;														// What index the trams should stop being created
	private int habitatIndex;													// How many keys between habitats

	private List<GameObject> tramBottomRightObjects = new List<GameObject>();	// List of all bottom right trams
	private List<GameObject> tramBottomLeftObjects = new List<GameObject>();	// List of all bottom left trams
	private List<GameObject> tramTopLeftObjects = new List<GameObject>();		// List of all top left trams
	private List<GameObject> tramTopRightObjects = new List<GameObject>();		// List of all top right trams
	private List<GameObject> keysBottomRight = new List<GameObject>();			// List of all bottom right keys. Points for the bottom right trams to travel to
	private List<GameObject> keysBottomLeft = new List<GameObject>();			// List of all bottom left keys. Points for the bottom left trams to travel to
	private List<GameObject> keysTopLeft = new List<GameObject>();				// List of all top left keys. Points for the top left trams to travel to
	private List<GameObject> keysTopRight = new List<GameObject>();				// List of all top right keys. Points for the top right trams to travel to

    private List<string> clipNames = new List<string>();
    #endregion

    #region Mono Methods
    void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		habitatIndex = (int)numKeysPerSection / RingHabitats.Instance.numRingHabitatsPerInstance;

		startIndex = 1;	// Set start/end section to only draw cars that can be seen by the user
		endIndex = 8;
		torusRadius = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2;

		CreateTramSections();	// Create all trams and keys
        UpdateKeyPositions();   // Move keys to proper positions
        CreateClipNames();      // Create list of clip names
        SetKeysToTrams();		// Now that all trams and sections are created, set all the tram keys for their movement
		DeleteKeys();           // Delete keys as they are no longer needed

        FloatingMenu.Instance.AddItems(train, "Tram", new Vector3(1, 1, 1));
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Set all trams active, copy the clip names list to each tram, and set
    /// the acceleration and top speed times to travel between each key
    /// </summary>
    /// <returns>The trams.</returns>
    public void ActivateTrams()
	{
		foreach(GameObject t in tramBottomRightObjects)
		{
			t.SetActive(true);
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(travelTime, accelerationTime);
		}

		foreach(GameObject t in tramTopRightObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(travelTime, accelerationTime);
        }

        foreach (GameObject t in tramBottomLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(travelTime, accelerationTime);
        }

        foreach (GameObject t in tramTopLeftObjects)
		{
			t.SetActive(true);
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(travelTime, accelerationTime);
        }
    }
    #endregion

    #region Private Methods
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
        //  Acceleration and velocity are set in the constants script

		float radiusRing 	= 2072f;	                                            // Radius of the tram ring
		float rpk 			= (2 * Mathf.PI) / (numSections * numKeysPerSection);	// Radians between keys
		float scaleRatio 	= 2072.0f / 5182853.0f;	                                // ratio of tram ring to actual ring (when the system is at scale 5000)
		float alKeys 		= radiusRing * rpk;	                                    // arc length between keys in meters
		float topSpeed 		= config.TramVelocity * scaleRatio;	                    // top speed scaled to system
		float acceleration 	= config.TramAcceleration * scaleRatio;	                // acceleartion scaled to system
		travelTime 			= alKeys / topSpeed;                                    // Time it takes to travel from one key to the next
		accelerationTime 	= topSpeed / acceleration;                              // Time it takes to accelerate to full speed

		// Print calculations for debuggin
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
		phi1 = torusRadius - habitatHeight * Mathf.Cos(-34 * Mathf.Deg2Rad);

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
			tram.name = "Bottom Right " + tramBottomRightObjects.Count.ToString();
			tram.transform.localPosition = habtop;
			tram.transform.localScale = new Vector3(1.5e-6f, 1.5e-6f, 1.5e-6f);

			GameObject tram1 = Instantiate(tram, transform);
			tram1.name =  "Top Left " + tramTopLeftObjects.Count.ToString();

			GameObject tram2 = Instantiate(tram, transform);
			tram2.name = "Bottom Left " + tramBottomLeftObjects.Count.ToString();

			GameObject tram3 = Instantiate(tram, transform);
			tram3.name = "Top Right " + tramTopRightObjects.Count.ToString();

			// Set orientation of trams other than the first
			if (tramBottomRightObjects.Count > 0)
			{
				tram.transform.LookAt(keysBottomRight[keysBottomRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram2.transform.LookAt(keysBottomLeft[keysBottomLeft.Count-2].transform.position, transform.TransformVector(habtop - habbot));

				tramTopLeftObjects[tramTopLeftObjects.Count-1].transform.LookAt(keysTopLeft[keysTopLeft.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tramTopRightObjects[tramTopRightObjects.Count-1].transform.LookAt(keysTopRight[keysTopRight.Count-2].transform.position, transform.TransformVector(habtop - habbot));
			}

			// Set the yield time for each tram
			tram.GetComponent<TramMotion>().SetWaitTime(tramBottomRightObjects.Count);
			tram1.GetComponent<TramMotion>().SetWaitTime(tramTopLeftObjects.Count);
			tram2.GetComponent<TramMotion>().SetWaitTime(tramBottomLeftObjects.Count);
			tram3.GetComponent<TramMotion>().SetWaitTime(tramTopRightObjects.Count);

			// Add trams to list to assign their keys later
			tramBottomRightObjects.Add(tram);
			tramTopLeftObjects.Add(tram1);
			tramBottomLeftObjects.Add(tram2);
			tramTopRightObjects.Add(tram3);
		}
	}

    /// <summary>
    /// Update positions of keys
    /// </summary>
    private void UpdateKeyPositions()
    {
        // Adjust bottom right keys
        foreach (GameObject k in keysBottomRight)
        {
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 3.5e-5f;
            k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 4e-5f;
        }

        // Adjust top right keys
        foreach (GameObject k in keysTopRight)
        {
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 4e-5f;
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 3.5e-6f;
        }

        // Adjust bottom left keys
        foreach (GameObject k in keysBottomLeft)
        {
			k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 4e-5f;
            k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 2.5e-5f;
        }

        // Adjust top left keys
        foreach (GameObject k in keysTopLeft)
        {
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 4e-5f;
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 1e-7f;
        }
    }

	/// <summary>
	/// Set the keys to each tram
	/// </summary>
	private void SetKeysToTrams()
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
            List<Quaternion> sortedRotations = ReverseSortKeysRotations(i, keysTopLeft);
            tramTopLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

            List<Vector3> sortedPositions = ReverseSortKeysPositions(i, keysTopLeft);
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

    private void CreateClipNames()
    {
        int clipInPeriod = 0;
        string name = "";

        for (int i=0; i<keysBottomLeft.Count; i++)
        {
            if (clipInPeriod == 0)
            {
                name = "Accelerate " + i;
            }
            else if (clipInPeriod == 4)
            {
                name = "Decelerate " + i;
            }
            else if (clipInPeriod == 5)
            {
                name = "Wait " + i;
                clipInPeriod = -1;
            }
            else
            {
                name = "Cruise " + i;
            }

            clipInPeriod++;
            clipNames.Add(name);
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
    #endregion
}