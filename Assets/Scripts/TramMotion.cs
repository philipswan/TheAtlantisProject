using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	public List<Quaternion> rotations = new List<Quaternion>();		// Rotation of the tram at its destination
	public List<Vector3> positions = new List<Vector3>();			// All destinatins for the tram
	private Material[] DefaultMaterials;
	public List<Material> HighlightMaterials = new List<Material>();

	bool highlited;
	private Constants.Configuration config;							// Holds reference to config script
	private float startTime;										// Starting time of the movement. Reset when a cycle is completed
	private bool travelTram;										// Set true if the tram does not stop
	private Vector3 velocity;										// Speed cap for smoothdamp

	void Awake()
	{
		velocity = Vector3.zero;
		travelTram = false;
	}

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
		highlited = false;

		DefaultMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
		for (int i=HighlightMaterials.Count-1; i<DefaultMaterials.Length; i++)
		{
			HighlightMaterials.Add(DefaultMaterials[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
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

		if (Scene < positions.Count - 1 && !travelTram)
		{
			UpdateSystem(Scene, Blend, travelTram);
		}
		else if (Scene < positions.Count && travelTram)
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
	}

	public void SetMaterials()
	{
		if (highlited)
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefaultMaterials;
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetFloat("_Outline", 0.001f);
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(0.9568f,0.2627f,0.2118f, 1));
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.001f);
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetColor("_Color", new Color(1,1,1,1));
		}

		highlited = ! highlited;
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

	/// <summary>
	/// Sets the travel tram (tram does not make stops)
	/// </summary>
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
	private void UpdateSystem(int scene, float blend, bool _travelTram)
	{
		int index0 = (scene > 0) ? (scene - 1) : 0;

		if (_travelTram)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, positions[scene], ref velocity, config.TramTravelTime);
			transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[scene], Mathf.Pow(blend, 1.05f));
		}
		else
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, positions[scene], ref velocity, config.TramTravelTimeStops);
			transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[scene], Mathf.Pow(blend, 1.05f));
		}

	}
}
