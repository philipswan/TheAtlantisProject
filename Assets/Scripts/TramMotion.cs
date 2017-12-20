using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	public List<Quaternion> rotations = new List<Quaternion>();			// Rotation of the tram at its destination
	public List<Vector3> positions = new List<Vector3>();				// All destinatins for the tram
	public List<Material> HighlightMaterials = new List<Material>();	// Regarul materials + highlight material

	private Constants.Configuration config;								// Holds reference to config script
	private int Scene;
	private int prevScene;
	private float sceneSwitchTime;
	private int insertedKeys;
	private int accelerationInstance;
	private float startTime;											// Starting time of the movement. Reset when a cycle is completed
	public float accelerationTime;
	public float travelTime;
	public float topSpeed;
	public float acceleration;
	public float cruiseTime;
	private bool travelTram;											// Set true if the tram does not stop
	private Material[] DefaultMaterials;								// Regular materials
	private bool highlited;												// Current materials used
	private enum AccelerationState {Accelerate, Cruise, Decelerate, Slowest, None}
	private AccelerationState accelerationState;
	private float accelerationStartTime;

	void Awake()
	{
		travelTram = false;
	}

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
		highlited = false;
		accelerationInstance = 0;
		accelerationState = AccelerationState.None;
		prevScene = -999;

		DefaultMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
		for (int i=HighlightMaterials.Count-1; i<DefaultMaterials.Length; i++)
		{
			HighlightMaterials.Add(DefaultMaterials[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float Blend = 0.0f;

		if (travelTram)
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / travelTime);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / travelTime - Scene, 1.0f);

			if ((acceleration * (Time.unscaledTime - startTime)) < topSpeed)
			{
				accelerationInstance++;
				Blend += acceleration * (Time.unscaledTime - startTime) - topSpeed;
			}
			else
			{
				if (name == "Bottom Right Tram 0")
				{
				}
			}

		}
		else
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / travelTime);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / travelTime - Scene, 1.0f);

			if (Scene != prevScene)
			{
				prevScene = Scene;
				sceneSwitchTime = Time.unscaledTime;
			}

			if ((Scene + 1) % 5 != 0 || ((Scene + 1) % 5 == 0) && (Time.unscaledTime - sceneSwitchTime) < (travelTime - accelerationTime))
			{
				if ((acceleration * (Time.unscaledTime - accelerationStartTime)) < topSpeed || accelerationState == AccelerationState.Slowest || accelerationState == AccelerationState.Decelerate)
				{
					if (accelerationState != AccelerationState.Accelerate)
					{
						accelerationState = AccelerationState.Accelerate;
						accelerationStartTime = Time.unscaledTime;
						accelerationInstance = 0;
					}

					accelerationInstance++;
					if (name == "Bottom Right Tram 0")
					{
						print("acceleration, Scene: " + Scene);
						print("Current velocity: " + (acceleration * (Time.unscaledTime - accelerationStartTime)));
					}

					Blend +=  acceleration * (Time.unscaledTime - accelerationStartTime) - topSpeed;
				}
				else
				{
					if (accelerationState != AccelerationState.Cruise)
					{
						accelerationState = AccelerationState.Cruise;
						accelerationStartTime = 0;
					}

					if (name == "Bottom Right Tram 0")
					{
						print("cruise, Scene: " + Scene);
					}
				}
			}
			else
			{
				if ((acceleration * (Time.unscaledTime - accelerationStartTime)) < topSpeed || accelerationState == AccelerationState.Cruise)
				{
					if (accelerationState != AccelerationState.Decelerate)
					{
						accelerationState = AccelerationState.Decelerate;
						accelerationStartTime = Time.unscaledTime;
					}

					if (name == "Bottom Right Tram 0")
					{
						print("deceleration, Scene: " + Scene);
						print("Current velocity: " + acceleration * (Time.unscaledTime - accelerationStartTime));
					}

					Blend -= acceleration * (Time.unscaledTime - accelerationStartTime);
				}
				else
				{
					if (accelerationState != AccelerationState.Slowest)
					{
						accelerationState = AccelerationState.Slowest;
						accelerationStartTime = 0;
					}

					if (name == "Bottom Right Tram 0")
					{
						print("slowest, Scene: " + Scene);
					}

					Blend -= topSpeed;
				}
			}
		}

		Blend = Mathf.Min(Blend, 1);

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
		accelerationTime = Time.unscaledTime;
	}

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
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
	/// <param name="rot">Rot.</param>
	public void AddRotation(Quaternion rot)
	{
		rotations.Add(rot);
	}

	/// <summary>
	/// Sets the rotations to the list
	/// </summary>
	/// <param name="rot">Rot.</param>
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
	/// Sets the speeds.
	/// </summary>
	/// <param name="_acceleration">Acceleration.</param>
	/// <param name="_topSpeed">Top speed.</param>
	/// <param name="_travelTime">Travel time.</param>
	public void SetSpeeds(float _acceleration, float _topSpeed, float _cruiseTime, float _accelerationTime, int _insertedKeys = 0)
	{
		acceleration = _acceleration;
		topSpeed = _topSpeed;
		cruiseTime = _cruiseTime;
		accelerationTime = _accelerationTime;
		insertedKeys = _insertedKeys;

		travelTime = cruiseTime;
	}

	/// <summary>
	/// Returns the first digit of an int
	/// </summary>
	/// <returns>The first digit.</returns>
	/// <param name="i">The index.</param>
	private int GetFirstDigit(int i)
	{
		if (i >= 100000000) i /= 100000000;
		if (i >= 10000) i /= 10000;
		if (i >= 100) i /= 100;
		if (i >= 10) i /= 10;

		return i;
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
		int index1Position = (scene < positions.Count - 1) ? (scene + 1) : positions.Count - 1;
		int index1Rotatoin = (scene < rotations.Count - 1) ? (scene + 1) : rotations.Count - 1;

		if (_travelTram)
		{
			transform.localPosition = Vector3.Lerp(positions[scene], positions[index1Position], blend);
			transform.localRotation = Quaternion.Lerp(rotations[scene], rotations[index1Rotatoin], blend);
		}
		else
		{
			transform.localPosition = Vector3.Lerp(positions[scene], positions[index1Position], blend);
			transform.localRotation = Quaternion.Lerp(rotations[scene], rotations[index1Rotatoin], blend);
		}
	}
}
