using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

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
	private float acceleration;                                                 // Rate of acceleartion for trams (m/s^2)
	private float topSpeed;                                                     // Velocity after acceleration (m/s)
	private float travelTime;                                                   // Time it takes to travel from one key to the next (s)
	private float accelerationTime;                                             // Time it takes to accelerate (s)
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

    private List<string> topLeftCurveNames = new List<string>();                       // List of all top left animation clip names
    private List<AnimationCurve> topLeftCurves = new List<AnimationCurve>();           // List of all top left animation clips
    private List<string> topRightCurveNames = new List<string>();                      // List of all top right animation clip names
    private List<AnimationCurve> topRightCurves = new List<AnimationCurve>();          // List of all top right animation clips
    private List<string> bottomLeftCurveNames = new List<string>();                    // List of all bottom left animation clip names
    private List<AnimationCurve> BottomLeftCurves = new List<AnimationCurve>();        // List of all bottom left animation clips
    private List<string> bottomRightCurveNames = new List<string>();                   // List of all bottom right animation clip names
    private List<AnimationCurve> bottomRightCurves = new List<AnimationCurve>();       // List of all bottom right animation clips

    private List<string> clipNames = new List<string>();

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
		UpdateTramPositions();	// Move trams to proper positions
        UpdateKeyPositions();   // Move keys to proper positions
        //CreateAllClips();
        CreateClipNames();
        SetKeysToTrams();		// Now that all trams and sections are created, set all the tram keys for their movement
		DeleteKeys();			// Delete keys as they are no longer needed

		//FloatingMenu.Instance.AddItems(train, "Tram", new Vector3(1,1,1));
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
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime, insertedKeys);
		}

		foreach(GameObject t in tramTopRightObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime);
        }

        foreach (GameObject t in tramBottomLeftObjects)
		{
			t.SetActive(true);
			t.GetComponent<TramMotion>().SetTravelTram();
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
            t.GetComponent<TramMotion>().SetSpeeds(acceleration, topSpeed, travelTime, accelerationTime);
        }

        foreach (GameObject t in tramTopLeftObjects)
		{
			t.SetActive(true);
            t.GetComponent<TramMotion>().AddClipNames(clipNames);
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
		phi1 = torusRadius - habitatHeight * Mathf.Cos(-34 * Mathf.Deg2Rad);

		// Find the current and next segments
		Vector3 habtop, habbot;

		habtop.x = phi0 * Mathf.Cos(theta) - torusRadius;
		habtop.z = phi0 * Mathf.Sin(theta);
		habtop.y = -Mathf.Pow(torusRadius - phi0, 1.7f) * 10;

		habbot.x = phi1 * Mathf.Cos(theta) - torusRadius;
		habbot.z = phi1 * Mathf.Sin(theta);
		habbot.y = -habitatHeight * Mathf.Sin(-34 * Mathf.Deg2Rad);

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
			//tramBottomRightObjects[0].transform.LookAt(keysBottomRight[keysBottomRight.Count - 1].transform.position, transform.TransformVector(prevUp));

			keysBottomLeft[0].transform.LookAt(keysBottomLeft[keysBottomLeft.Count - 1].transform.position, transform.TransformVector(prevUp));
			//tramBottomLeftObjects[0].transform.LookAt(keysBottomLeft[keysBottomLeft.Count - 1].transform.position, transform.TransformVector(prevUp));
		
			key1.transform.LookAt(keysTopLeft[0].transform.position, transform.TransformVector(prevUp));
			//tramTopLeftObjects[tramTopLeftObjects.Count-1].transform.LookAt(keysTopLeft[0].transform.position, transform.TransformVector(prevUp));

			key3.transform.LookAt(keysTopRight[0].transform.position, transform.TransformVector(prevUp));
			//tramTopRightObjects[tramTopRightObjects.Count-1].transform.LookAt(keysTopRight[0].transform.position, transform.TransformVector(prevUp));
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
	/// Update positions of trams
	/// </summary>
	private void UpdateTramPositions()
	{
		// Adjust bottom right trams
		foreach (GameObject t in tramBottomRightObjects)
		{
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.right) * 9.5e-5f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 6e-5f;
		}

		// Adjust top right trams
		foreach (GameObject t in tramTopRightObjects)
		{
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.right) * 1.2e-4f;
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.up) * 3e-5f;
		}

		// Adjust bottom left trams
		foreach (GameObject t in tramBottomLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1e-4f;
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.up) * 1e-5f;
		}

		// Adjust top left trams
		foreach (GameObject t in tramTopLeftObjects)
		{
			t.transform.localPosition -= transform.InverseTransformPoint(t.transform.right) * 1e-4f;
			t.transform.localPosition += transform.InverseTransformPoint(t.transform.up) * 9e-5f;
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
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 9.5e-5f;
            k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 6e-5f;
        }

        // Adjust top right keys
        foreach (GameObject k in keysTopRight)
        {
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.right) * 1.2e-4f;
            k.transform.localPosition += transform.InverseTransformPoint(k.transform.up) * 3e-5f;
        }

        // Adjust bottom left keys
        foreach (GameObject k in keysBottomLeft)
        {
            k.transform.localPosition -= transform.InverseTransformPoint(k.transform.right) * 1e-4f;
            k.transform.localPosition -= transform.InverseTransformPoint(k.transform.up) * 1e-5f;
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
	private void SetKeysToTrams()
	{
		// Set the keys for each tram in the bottom right track
		for (int i=0; i<tramBottomRightObjects.Count; i++)
		{
            //List<string> sortedNames = SortAnimationCurveNames(i, bottomRightCurveNames);
            //tramBottomRightObjects[i].GetComponent<TramMotion>().AddClipNames(sortedNames);

            //List<AnimationCurve> sortedCurves = SortAnimationCurves(i, bottomRightCurves);
            //tramBottomRightObjects[i].GetComponent<TramMotion>().AddCurves(sortedCurves);

            List<Quaternion> sortedRotations = SortKeysRotations(i, keysBottomRight);
            tramBottomRightObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

            List<Vector3> sortedPositions = SortKeysPositions(i, keysBottomRight);
            tramBottomRightObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
        }

        // Set the keys for each tram in the bottom left track
        for (int i=0; i<tramBottomLeftObjects.Count; i++)
		{
            //List<string> sortedNames = SortAnimationCurveNames(i, bottomLeftCurveNames);
            //tramBottomLeftObjects[i].GetComponent<TramMotion>().AddClipNames(sortedNames);

            //List<AnimationCurve> sortedCurves = SortAnimationCurves(i, BottomLeftCurves);
            //tramBottomLeftObjects[i].GetComponent<TramMotion>().AddCurves(sortedCurves);

            List<Quaternion> sortedRotations = SortKeysRotations(i, keysBottomLeft);
            tramBottomLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

            List<Vector3> sortedPositions = SortKeysPositions(i, keysBottomLeft);
            tramBottomLeftObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
        }

		// Set the keys for each tram in the top left track
		for (int i=0; i<tramTopLeftObjects.Count; i++)
		{
            //List<string> sortedNames = SortAnimationCurveNames(i, topLeftCurveNames);
            //tramTopLeftObjects[i].GetComponent<TramMotion>().AddClipNames(sortedNames);

            //List<AnimationCurve> sortedCurves = SortAnimationCurves(i, topLeftCurves);
            //tramTopLeftObjects[i].GetComponent<TramMotion>().AddCurves(sortedCurves);

            List<Quaternion> sortedRotations = ReverseSortKeysRotations(i, keysTopLeft);
            tramTopLeftObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

            List<Vector3> sortedPositions = ReverseSortKeysPositions(i, keysTopLeft);
            tramTopLeftObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
        }

		// Set the keys for each tram in the top right track
		for (int i=0; i<tramTopRightObjects.Count; i++)
		{
            //List<string> sortedNames = SortAnimationCurveNames(i, topRightCurveNames);
            //tramTopRightObjects[i].GetComponent<TramMotion>().AddClipNames(sortedNames);

            //List<AnimationCurve> sortedCurves = SortAnimationCurves(i, topRightCurves);
            //tramTopRightObjects[i].GetComponent<TramMotion>().AddCurves(sortedCurves);

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
    /// Sort the curve names
    /// </summary>
    /// <param name="index"></param>
    /// <param name="curveNames"></param>
    /// <returns></returns>
    private List<string> SortAnimationCurveNames(int index, List<string> curveNames)
    {
        List<string> sortedCurveNames = new List<string>();
        index *= (int)numKeysPerSection / numTramsPerSection;
        index += startIndex * numKeysPerSection;

        for (int i = index; i >= 0; i--)
        {
            sortedCurveNames.Add(curveNames[i]);
        }
        for (int i = curveNames.Count - 1; i > index; i--)
        {
            sortedCurveNames.Add(curveNames[i]);
        }

        return sortedCurveNames;
    }

    /// <summary>
    /// Sort the curve names in reverse
    /// </summary>
    /// <param name="index"></param>
    /// <param name="curveNames"></param>
    /// <returns></returns>
    private List<string> ReverseSortAnimationCurveNames(int index, List<string> curveNames)
    {
        List<string> sortedCurveNames = new List<string>();
        index *= (int)numKeysPerSection / numTramsPerSection;
        index += startIndex * numKeysPerSection;

        for (int i = index; i < curveNames.Count; i++)
        {
            sortedCurveNames.Add(curveNames[i]);
        }
        for (int i = 0; i < index; i++)
        {
            sortedCurveNames.Add(curveNames[i]);
        }

        return sortedCurveNames;
    }

    private List<AnimationCurve> SortAnimationCurves(int index, List<AnimationCurve> curves)
    {
        List<AnimationCurve> sortedCurves = new List<AnimationCurve>();
        index *= (int)numKeysPerSection / numTramsPerSection;
        index += startIndex * numKeysPerSection;
        index *= 7;

        for (int i = index; i >= 0; i--)
        {
            sortedCurves.Add(curves[i]);
        }
        for (int i = curves.Count - 1; i > index; i--)
        {
            sortedCurves.Add(curves[i]);
        }

        return sortedCurves;
    }

    private List<AnimationCurve> ReverseSortAnimationCurves(int index, List<AnimationCurve> curves)
    {
        List<AnimationCurve> sortedCurves = new List<AnimationCurve>();
        index *= (int)numKeysPerSection / numTramsPerSection;
        index += startIndex * numKeysPerSection;
        index *= 7;

        for (int i = index; i < curves.Count; i++)
        {
            sortedCurves.Add(curves[i]);
        }
        for (int i = 0; i < index; i++)
        {
            sortedCurves.Add(curves[i]);
        }

        return sortedCurves;
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
    /// Gets the position keyframes.
    /// </summary>
    /// <returns>The position keyframes.</returns>
    /// <param name="startPos">Start position.</param>
    /// <param name="endPos">End position.</param>
    /// <param name="state">State.</param>
    /// <param name="samples">Samples.</param>
    private List<Keyframe[]> GetPositionKeyframes(Vector3 startPos, Vector3 endPos, int samples = 1000)
    {
        Keyframe[] keyframesX = new Keyframe[samples];  // Initialize array to hold x coord keyframes

        for (int i = 0; i < keyframesX.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesX[i] = new Keyframe(((float)i / keyframesX.Length), Mathf.Lerp(startPos.x, endPos.x, (float)i / keyframesX.Length));
        }

        Keyframe[] keyframesY = new Keyframe[samples];  // Initialize array to hold y coord keyframes

        for (int i = 0; i < keyframesY.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesY[i] = new Keyframe(((float)i / keyframesY.Length), Mathf.Lerp(startPos.y, endPos.y, (float)i / keyframesY.Length));
        }

        Keyframe[] keyframesZ = new Keyframe[samples];  // Initialize array to hold z coord keyframes

        for (int i = 0; i < keyframesZ.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesZ[i] = new Keyframe(((float)i / keyframesZ.Length), Mathf.Lerp(startPos.z, endPos.z, (float)i / keyframesZ.Length));
        }

        List<Keyframe[]> keyframes = new List<Keyframe[]>() { keyframesX, keyframesY, keyframesZ };
        return keyframes;
    }

    /// <summary>
    /// Gets the rotation keyframes.
    /// </summary>
    /// <returns>The rotation keyframes.</returns>
    /// <param name="startRot">Start rot.</param>
    /// <param name="endRot">End rot.</param>
    /// <param name="state">State.</param>
    /// <param name="samples">Samples.</param>
    private List<Keyframe[]> GetRotationKeyframes(Quaternion startRot, Quaternion endRot, int samples)
    {
        Keyframe[] keyframesX = new Keyframe[samples];  // Initialize array to hold x coord keyframes

        for (int i = 0; i < keyframesX.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesX[i] = new Keyframe(((float)i / keyframesX.Length), Mathf.Lerp(startRot.x, endRot.x, (float)i / keyframesX.Length));
        }

        Keyframe[] keyframesY = new Keyframe[samples];  // Initialize array to hold y coord keyframes

        for (int i = 0; i < keyframesY.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesY[i] = new Keyframe(((float)i / keyframesY.Length), Mathf.Lerp(startRot.y, endRot.y, (float)i / keyframesY.Length));
        }

        Keyframe[] keyframesZ = new Keyframe[samples];  // Initialize array to hold z coord keyframes

        for (int i = 0; i < keyframesZ.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesZ[i] = new Keyframe(((float)i / keyframesZ.Length), Mathf.Lerp(startRot.z, endRot.z, (float)i / keyframesZ.Length));
        }

        Keyframe[] keyframesW = new Keyframe[samples];  // Initialize array to hold w coord keyframes

        for (int i = 0; i < keyframesW.Length; i++)
        {
            // Lerp through points by sample interval
            keyframesW[i] = new Keyframe(((float)i / keyframesW.Length), Mathf.Lerp(startRot.w, endRot.w, (float)i / keyframesW.Length));
        }

        List<Keyframe[]> keyframes = new List<Keyframe[]>() { keyframesX, keyframesY, keyframesZ, keyframesW };
        return keyframes;
    }

    /// <summary>
    /// Creates the curves for the x, y, and z positions or rotations
    /// </summary>
    /// <returns>The curve.</returns>
    /// <param name="keyframes">Keyframes.</param>
    private List<AnimationCurve> CreateCurve(List<Keyframe[]> keyframes)
    {
        AnimationCurve localxPos = new AnimationCurve(keyframes[0]);
        AnimationCurve localyPos = new AnimationCurve(keyframes[1]);
        AnimationCurve localzPos = new AnimationCurve(keyframes[2]);
        AnimationCurve localxRot = new AnimationCurve(keyframes[3]);
        AnimationCurve localyRot = new AnimationCurve(keyframes[4]);
        AnimationCurve localzRot = new AnimationCurve(keyframes[5]);
        AnimationCurve localwRot = new AnimationCurve(keyframes[6]);

        List<AnimationCurve> curves = new List<AnimationCurve>() { localxPos, localyPos, localzPos, localxRot, localyRot, localzRot, localwRot };
        return curves;
    }

    /// <summary>
    /// Creates an animation clip and adds it to the list
    /// </summary>
    /// <returns>The clip.</returns>
    /// <param name="curves">Curves.</param>
    private AnimationClip CreateClip(List<AnimationCurve> curves)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        clip.SetCurve("", typeof(Transform), "localPosition.x", curves[0]);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curves[1]);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curves[2]);
        clip.SetCurve("", typeof(Transform), "localRotation.x", curves[3]);
        clip.SetCurve("", typeof(Transform), "localRotation.y", curves[4]);
        clip.SetCurve("", typeof(Transform), "localRotation.z", curves[5]);
        clip.SetCurve("", typeof(Transform), "localRotation.w", curves[6]);

        return clip;
    }

    /// <summary>
    /// Creates all animation curves
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="clipNames"></param>
    /// <param name="animationCurves"></param>
    private void CreateCurves(List<GameObject> keys, List<string> clipNames, List<AnimationCurve> animationCurves)
    {
        string name = "";
        int clipInPeriod = 0;

        for (int i = 0; i < keys.Count - 1; i++)
        {
            List<Keyframe[]> keyframes = new List<Keyframe[]>();
            List<Keyframe[]> keyPositions = new List<Keyframe[]>();
            List<Keyframe[]> keyRotations = new List<Keyframe[]>();
            List<AnimationCurve> curves = new List<AnimationCurve>();

            // If we're at a habitat, create a clip with the same start/end position/rotation to stop the tram
            if (i % 5 == 0 && i != 0)
            {
                // Get the keyframes between the two positions
                keyPositions = GetPositionKeyframes(keys[i].transform.localPosition, keys[i].transform.localPosition, 1000);
                keyRotations = GetRotationKeyframes(keys[i].transform.localRotation, keys[i].transform.localRotation, 1000);

                // Combine the x, y, and z coordinates for position and rotation into one list
                for (int j = 0; j < keyPositions.Count; j++)
                {
                    keyframes.Add(keyPositions[j]);
                }
                for (int j = 0; j < keyRotations.Count; j++)
                {
                    keyframes.Add(keyRotations[j]);
                }

                // Create animation curves from the keyframes
                curves = CreateCurve(keyframes);

                for (int j=0; j<curves.Count; j++)
                {
                    animationCurves.Add(curves[j]);
                }

                keyframes.Clear();
                keyPositions.Clear();
                keyRotations.Clear();
                curves.Clear();

                name = "Wait " + i;

                clipNames.Add(name);

                clipInPeriod = 0;

            }

            // Get the keyframes between the two positions
            keyPositions = GetPositionKeyframes(keys[i].transform.localPosition, keys[i + 1].transform.localPosition, 1000);
            keyRotations = GetRotationKeyframes(keys[i].transform.localRotation, keys[i + 1].transform.localRotation, 1000);

            // Combine the x, y, and z coordinates for position and rotation into one list
            for (int j = 0; j < keyPositions.Count; j++)
            {
                keyframes.Add(keyPositions[j]);
            }
            for (int j = 0; j < keyRotations.Count; j++)
            {
                keyframes.Add(keyRotations[j]);
            }

            // Create animation curves from the keyframes
            curves = CreateCurve(keyframes);

            for (int j = 0; j < curves.Count; j++)
            {
                animationCurves.Add(curves[j]);
            }

            if (clipInPeriod == 0)
            {
                name = "Accelerate " + i;
            }
            else if (clipInPeriod == 4)
            {
                name = "Decelerate " + i;
            }
            else
            {
                name = "Cruise " + i;
            }

            clipNames.Add(name);

            clipInPeriod++;

        }
    }

    /// <summary>
    /// Creates all animation curves
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="clipNames"></param>
    /// <param name="animationCurves"></param>
    private void ReverseCreateCurves(List<GameObject> keys, List<string> clipNames, List<AnimationCurve> animationCurves)
    {
        string name = "";
        int clipInPeriod = 0;

        for (int i = keys.Count - 1; i > 0; i--)
        {
            List<Keyframe[]> keyframes = new List<Keyframe[]>();
            List<Keyframe[]> keyPositions = new List<Keyframe[]>();
            List<Keyframe[]> keyRotations = new List<Keyframe[]>();
            List<AnimationCurve> curves = new List<AnimationCurve>();

            // If we're at a habitat, create a clip with the same start/end position/rotation to stop the tram
            if (i % 5 == 0 && i != 0)
            {
                // Get the keyframes between the two positions
                keyPositions = GetPositionKeyframes(keys[i].transform.localPosition, keys[i].transform.localPosition, 1000);
                keyRotations = GetRotationKeyframes(keys[i].transform.localRotation, keys[i].transform.localRotation, 1000);

                // Combine the x, y, and z coordinates for position and rotation into one list
                for (int j = 0; j < keyPositions.Count; j++)
                {
                    keyframes.Add(keyPositions[j]);
                }
                for (int j = 0; j < keyRotations.Count; j++)
                {
                    keyframes.Add(keyRotations[j]);
                }

                // Create animation curves from the keyframes
                curves = CreateCurve(keyframes);

                for (int j = 0; j < curves.Count; j++)
                {
                    animationCurves.Add(curves[j]);
                }

                keyframes.Clear();
                keyPositions.Clear();
                keyRotations.Clear();
                curves.Clear();

                name = "Wait " + i;

                clipNames.Add(name);

                clipInPeriod = 0;

            }

            // Get the keyframes between the two positions
            keyPositions = GetPositionKeyframes(keys[i].transform.localPosition, keys[i - 1].transform.localPosition, 1000);
            keyRotations = GetRotationKeyframes(keys[i].transform.localRotation, keys[i - 1].transform.localRotation, 1000);

            // Combine the x, y, and z coordinates for position and rotation into one list
            for (int j = 0; j < keyPositions.Count; j++)
            {
                keyframes.Add(keyPositions[j]);
            }
            for (int j = 0; j < keyRotations.Count; j++)
            {
                keyframes.Add(keyRotations[j]);
            }

            // Create animation curves from the keyframes
            curves = CreateCurve(keyframes);

            for (int j = 0; j < curves.Count; j++)
            {
                animationCurves.Add(curves[j]);
            }

            if (clipInPeriod == 0)
            {
                name = "Accelerate " + i;
            }
            else if (clipInPeriod == 4)
            {
                name = "Decelerate " + i;
            }
            else
            {
                name = "Cruise " + i;
            }

            clipNames.Add(name);

            clipInPeriod++;

        }
    }

    /// <summary>
    /// Genearte all clips
    /// </summary>
    private void CreateAllClips()
    {
        CreateCurves(keysBottomLeft, bottomLeftCurveNames, BottomLeftCurves);      // Generate curves for bottom left track
        CreateCurves(keysBottomRight, bottomRightCurveNames, bottomRightCurves);   // Generate curves for bottom right track
        ReverseCreateCurves(keysTopLeft, topLeftCurveNames, topLeftCurves);        // Generate curves for top left track
        ReverseCreateCurves(keysTopRight, topRightCurveNames, topRightCurves);     // Generate curves for top right track
    }
}