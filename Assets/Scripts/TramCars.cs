﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TramCars : MonoBehaviour {

	[Tooltip("Totol keys is numKeys * numTrams")]
	public int numKeys = 10;												// Number of sections
	public int numTrams = 10;												// Number of trams per section
	public float habitatHeight = 0.00001f;									// Offset
	public GameObject train;												// Train car prefab

	private float torusRadius;												// Radius of torus ring
	private Constants.Configuration config;									// Holds reference to config file
	private List<GameObject> tramBottomObjects = new List<GameObject>();	// List of all bottom trams
	private List<GameObject> tramTopObjects = new List<GameObject>();		// List of all top trams
	private List<GameObject> keysBottom = new List<GameObject>();			// List of all top keys. Points for the top trams to travel to
	private List<GameObject> keysTop = new List<GameObject>();				// List of all bottom keys. Points for the bottom trams to travel to
	private Vector3 prevUp;													// Holds up position for first tram

	// Use this for initialization
	void Start()
	{
		config = Constants.Configuration.Instance;

		torusRadius = Mathf.Cos(config.RingLatitude * 1.025f * Mathf.PI / 180) / 2;
		CreateTramSections();
		SetTramsActive();
		DeleteKeys();
	}

	/// <summary>
	/// Call coroutine to set tram object active
	/// </summary>
	public void SetTramsActive()
	{
		ActivateTrams();
		//StartCoroutine("ActivateTrams");
	}
		
	/// <summary>
	/// Set trams active over multiple frames
	/// </summary>
	/// <returns>The trams.</returns>
	private void ActivateTrams()
	{
		foreach(GameObject t in tramBottomObjects)
		{
			t.SetActive(true);
		}
			
		foreach(GameObject t in tramTopObjects)
		{
			t.SetActive(true);
		}
	}

	/// <summary>
	/// Delete all keys
	/// </summary>
	private void DeleteKeys()
	{
		foreach (GameObject k in keysTop)
		{
			Destroy(k);
		}
		keysTop.Clear();

		foreach (GameObject k in keysBottom)
		{
			Destroy(k);
		}
		keysBottom.Clear();
	}

	/// <summary>
	/// Create a new tram section
	/// </summary>
	private void CreateTramSections()
	{
		bool createTram;
		int tramSpacing = (int)numKeys / numTrams;
		int numRingHabitats = numTrams * numKeys;
		float ringHabitatSpacing = 2.0f * Mathf.PI / (float)numRingHabitats;

		for (int instance = 0; instance < numKeys; instance++)	// Iterate through the sections
		{
			createTram = instance % tramSpacing == 0 ? true : false;	// Should we create a tram in this section?

			for (int ringHabitatIndex = 0; ringHabitatIndex < numTrams; ringHabitatIndex++)	// Iterate through the keys and tram if there is one
			{
				CreateTramsInSection(
					instance,
					ringHabitatIndex,
					ringHabitatSpacing,
					createTram);

				if (createTram)
				{
					createTram = false;	// Only 1 tram per section
				}

			}

		}

		UpdateTramKeys();	// Now that all trams and sections are created, set all the tram keys for their movement
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

		theta = (instance * numTrams + ringHabitatIndex) * ringHabitatSpacing;
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

		GameObject key = new GameObject("Key " + keysBottom.Count);
		key.transform.SetParent(transform);
		key.transform.localPosition = habtop;

		GameObject key1 = Instantiate(key, transform);

		// If we have no other reference object to look at, save the up vector for later
		if (keysBottom.Count == 0)
		{
			prevUp = habtop - habbot;
		}
		// Set orientation of all keys other than the first
		else if (keysBottom.Count > 0)
		{
			key.transform.LookAt(keysBottom[keysBottom.Count-1].transform.position, transform.TransformVector(habtop - habbot));
			key.transform.localPosition -= transform.InverseTransformPoint(key.transform.right) * 1e-5f;

			key1.transform.LookAt(keysTop[keysTop.Count-1].transform.position, transform.TransformVector(habtop - habbot));
			key1.transform.localPosition -= transform.InverseTransformPoint(key1.transform.right) * 1e-5f;
			key1.transform.localPosition += transform.InverseTransformPoint(key1.transform.up) * 7e-6f;
		}
			
		// Add keys to list to assign them to trams later
		keysBottom.Add(key);
		keysTop.Add(key1);

		// Set the last key and tram position and orientation
		if (keysBottom.Count == numKeys * numTrams)
		{
			keysBottom[0].transform.LookAt(key.transform.position, transform.TransformVector(prevUp));
			keysBottom[0].transform.localPosition -= transform.InverseTransformPoint(keysBottom[0].transform.right) * 1e-5f;

			tramBottomObjects[0].transform.LookAt(key.transform.position, transform.TransformVector(prevUp));
			tramBottomObjects[0].transform.localPosition -= transform.InverseTransformPoint(tramBottomObjects[0].transform.right) * 1e-5f;

			keysTop[0].transform.LookAt(key.transform.position, transform.TransformVector(prevUp));
			keysTop[0].transform.localPosition -= transform.InverseTransformPoint(keysTop[0].transform.right) * 1e-5f;
			keysTop[0].transform.localPosition += transform.InverseTransformPoint(keysTop[0].transform.up) * 7e-6f;

			tramTopObjects[0].transform.LookAt(key1.transform.position, transform.TransformVector(prevUp));
			tramTopObjects[0].transform.localPosition -= transform.InverseTransformPoint(tramTopObjects[0].transform.right) * 1e-5f;
			tramTopObjects[0].transform.localPosition += transform.InverseTransformPoint(tramTopObjects[0].transform.up) * 7e-6f;
		}

		// Create a tram at an interval set by the user
		if (createTram)
		{
			GameObject tram = Instantiate(train, transform);
			//tram.SetActive(true);
			tram.name = "Tram " + tramBottomObjects.Count;
			tram.transform.localPosition = habtop;
			tram.transform.localScale = new Vector3(6e-7f, 6e-7f, 6e-7f);

			GameObject tram1 = Instantiate(tram, transform);

			// Set orientation of trams other than the first
			if (tramBottomObjects.Count > 0)
			{
				tram.transform.LookAt(keysBottom[keysBottom.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram.transform.localPosition -= transform.InverseTransformPoint(tram.transform.right) * 1e-5f;

				tram1.transform.LookAt(keysTop[keysBottom.Count-2].transform.position, transform.TransformVector(habtop - habbot));
				tram1.transform.localPosition -= transform.InverseTransformPoint(tram1.transform.right) * 1e-5f;
				tram1.transform.localPosition += transform.InverseTransformPoint(tram1.transform.up) * 7e-6f;
			}
				
			// Add trams to list to assign their keys later
			tramBottomObjects.Add(tram);
			tramTopObjects.Add(tram1);
		}
	}

	/// <summary>
	/// Updates the tram keys.
	/// </summary>
	private void UpdateTramKeys()
	{
		// Set the keys for each tram in the bottom track
		for (int i=0; i<tramBottomObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = SortKeysRotations(i, keysBottom);
			tramBottomObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysBottom);
			tramBottomObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
		}
		// Set the keys for each tram in the bottom track
		for (int i=0; i<tramTopObjects.Count; i++)
		{
			List<Quaternion> sortedRotations = SortKeysRotations(i, keysTop);
			tramTopObjects[i].GetComponent<TramMotion>().AddRotation(sortedRotations);

			List<Vector3> sortedPositions = SortKeysPositions(i, keysTop);
			tramTopObjects[i].GetComponent<TramMotion>().AddPosition(sortedPositions);
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
		index *= numKeys;

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
		index *= numKeys;

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