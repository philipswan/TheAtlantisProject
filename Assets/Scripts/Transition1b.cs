﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1b : MonoBehaviour {

	public static Transition1b Instance;
	[Tooltip("The transforms that the system will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();		// Holds all transforms
	public List<Material> Materials = new List<Material>();		// Runtime materials

	private Constants.Configuration config;						// Holds reference to config script
	private float startTime;									// Time script was enabled

    // Use this for initialization
    void Awake () {
		Instance = this;
		enabled = false;
	}

	void Start()
	{
		config = Constants.Configuration.Instance;
	}

    void Update() {
		int Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.SystemTravelTime);
		float Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.SystemTravelTime - Scene, 1.0f);

		if (Scene < Keys.Count * 2 - 1)
		{
			UpdateSystem(Scene, Blend);
		}
		else if (Scene == Keys.Count * 2 - 1)
		{
			Transition1.Instance.enabled = true;
			enabled = false;
		}
    }

	void OnEnable()
	{
		transform.GetChild(0).GetComponent<MeshRenderer>().materials = Materials.ToArray();
		startTime = Time.unscaledTime;
	}

	/// <summary>
	/// Max the specified a and b.
	/// </summary>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
    private int max(int a, int b)
    {
        return (a > b) ? a : b;
    }

	/// <summary>
	/// Minimum the specified a and b.
	/// </summary>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
    private int min(int a, int b)
    {
        return (a < b) ? a : b;
    }

    /// <summary>
    /// Move the system to the proper transforms in the Keys list
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="blend">Blend.</param>
    private void UpdateSystem(int scene, float blend)
    {
        int index0 = max(0, min(Keys.Count - 1, (int)Mathf.Floor(scene / 2)));
        int index1 = max(0, min(Keys.Count - 1, (int)Mathf.Floor((scene+1) / 2)));

		//transform.localPosition = Vector3.Lerp(Keys[index0].localPosition, Keys[index1].position, blend);
		transform.localRotation = Quaternion.Lerp(Keys[index0].localRotation, Keys[index1].localRotation, Mathf.Pow(blend, 1.05f));
		transform.localScale = Vector3.Lerp(Keys[index0].localScale, Keys[index1].localScale, Mathf.Pow(blend, 1.75f));
    }

	/// <summary>
	/// Set the transform for the user's elevator
	/// </summary>
	/// <param name="_elevator">Elevator.</param>
	public void SetElevator(Transform _elevator)
	{
		Keys.Add(_elevator);
	}
}
