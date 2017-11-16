using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	private List<Quaternion> rotations = new List<Quaternion>();
	private List<Vector3> positions = new List<Vector3>();
	private Constants.Configuration config;						// Holds reference to config script

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		int Scene = (int)Mathf.Floor(Time.unscaledTime / config.TramTravelTime);
		float Blend = Mathf.Min (Time.unscaledTime / config.TramTravelTime - Scene, 1.0f);

		if (Scene < positions.Count * 2)
		{
			UpdateSystem(Scene, Blend);
		}
		else
		{
			// Restart cycle here	
		}
	}

	/// <summary>
	/// Adds key to list
	/// </summary>
	/// <param name="_key">Key.</param>
	public void AddRotation(Quaternion rot)
	{
		rotations.Add(rot);
	}

	// Adds position to list
	public void AddPosition(Vector3 pos)
	{
		positions.Add(pos);
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
		int index0 = max(0, min(positions.Count - 1, (int)Mathf.Floor(scene / 2)));
		int index1 = max(0, min(positions.Count - 1, (int)Mathf.Floor((scene+1) / 2)));

		transform.localPosition = Vector3.Lerp(positions[index0], positions[index1], blend);
		transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[index1], Mathf.Pow(blend, 1.05f));
	}
}
