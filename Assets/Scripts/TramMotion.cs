using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	public List<Quaternion> rotations = new List<Quaternion>();		// Rotation of the tram at its destination
	public List<Vector3> positions = new List<Vector3>();			// All destinatins for the tram
	private Constants.Configuration config;							// Holds reference to config script
	private float startTime;										// Starting time of the movement. Reset when a cycle is completed
	private bool keysSet;											// Flag controlling activation of tram motion
	private bool travelTram;										// Set true if the tram does not stop
	private Vector3 velocity;										// Speed cap for smoothdamp

	void Awake()
	{
		velocity = Vector3.zero;
		travelTram = false;
		keysSet = false;
	}

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (!keysSet)
		{ return; }
		int Scene = 0;
		float Blend = 0.0f;

		if (travelTram)
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.TramTravelTime);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.TramTravelTime - Scene, 1.0f);
		}
		else
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.TramTravelTimeStops);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.TramTravelTimeStops - Scene, 1.0f);
		}

		if (Scene < positions.Count * 2)
		{
			UpdateSystem(Scene, Blend, travelTram);
		}
		else
		{
			startTime = Time.unscaledTime;
		}
	}

	void OnEnable()
	{
		startTime = Time.unscaledTime;
		keysSet = true;
	}

	/// <summary>
	/// Adds the rotation to the list
	/// </summary>
	/// <param name="_key">Key.</param>
	public void AddRotation(Quaternion rot)
	{
		rotations.Add(rot);
	}

	/// <summary>
	/// Sets the rotations to the list
	/// </summary>
	/// <param name="_key">Key.</param>
	public void AddRotation(List<Quaternion> rot)
	{
		rotations = new List<Quaternion>(rot);
	}

	/// <summary>
	/// Adds the position to the list.
	/// </summary>
	/// <param name="pos">Position.</param>
	public void AddPosition(Vector3 pos)
	{
		positions.Add(pos);
	}

	/// <summary>
	/// Sets the positions to the list
	/// </summary>
	/// <param name="pos">Position.</param>
	public void AddPosition(List<Vector3> pos)
	{
		positions = new List<Vector3>(pos);
	}

	public void SetTravelTram()
	{
		travelTram = true;
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
	private void UpdateSystem(int scene, float blend, bool makeStops)
	{
		if (!makeStops)
		{
			int index0 = max(0, min(positions.Count - 1, (int)Mathf.Floor(scene / 2)));
			int index1 = max(0, min(positions.Count - 1, (int)Mathf.Floor((scene+1) / 2)));

			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, positions[index1], ref velocity, config.TramTravelTimeStops/2);
			transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[index1], Mathf.Pow(blend, 1.05f));

			///print(Vector3.Distance(transform.localPosition, positions[index1]));

//			transform.localPosition = Vector3.Lerp(positions[index0], positions[index1], blend);

			return;
		}
		else
		{
			int index0 = (scene > 0) ? (scene - 1) : 0;

			transform.localPosition = Vector3.Lerp(positions[index0], positions[scene], blend);
			transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[scene], Mathf.Pow(blend, 1.05f));
		}
	}
}
