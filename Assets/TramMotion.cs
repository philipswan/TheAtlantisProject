using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	private List<Transform> keys = new List<Transform>();
	private Constants.Configuration config;						// Holds reference to config script

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		int Scene = (int)Mathf.Floor(Time.unscaledTime / config.TramTravelTime);
		float Blend = Mathf.Min (Time.unscaledTime / config.TramTravelTime - Scene, 1.0f);

		if (Scene < keys.Count * 2)
		{
			UpdateSystem(Scene, Blend);
		}
		else if (Scene == keys.Count * 2)
		{
			//Transition1.Instance.BeginTransition();
			enabled = false;
		}
	}

	/// <summary>
	/// Adds key to list
	/// </summary>
	/// <param name="_key">Key.</param>
	public void AddKey(Transform _key)
	{
		keys.Add(_key);
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
	/// Updates the tram transform.
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="blend">Blend.</param>
	private void UpdateSystem(int scene, float blend)
	{
		int index0 = max(0, min(keys.Count - 1, (int)Mathf.Floor(scene / 2)));
		int index1 = max(0, min(keys.Count - 1, (int)Mathf.Floor((scene+1) / 2)));

		transform.localPosition = Vector3.Lerp(keys[index0].localPosition, keys[index1].localPosition, Mathf.Pow(blend, 1f));
		transform.localRotation = Quaternion.Lerp(keys[index0].localRotation, keys[index1].localRotation, Mathf.Pow(blend, 1.05f));
	}
}
