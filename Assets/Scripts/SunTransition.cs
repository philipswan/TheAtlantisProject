using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTransition : MonoBehaviour {

	public static SunTransition Instance;

	[Tooltip("The transforms that the sun will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();		// Holds all transforms

	private Constants.Configuration config;						// Holds reference to config script
	private float startTime;									// Time script was enabled

	// Use this for initialization
	void Start () {
		Instance = this;
		config = Constants.Configuration.Instance;
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		int Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.SystemTravelTime);
		float Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.SystemTravelTime - Scene, 1.0f);

		if (Scene < Keys.Count * 2 - 1)
		{
			UpdateSystem(Scene + 1, Blend);
		}
		else if (Scene == Keys.Count * 2 - 1)
		{
			enabled = false;
		}
	}

	void OnEnable()
	{
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

		Quaternion newRotation = Quaternion.Lerp(Keys[index0].localRotation, Keys[index1].localRotation, Mathf.Pow(blend, 1.05f));

		transform.localRotation = newRotation;
	}
}
