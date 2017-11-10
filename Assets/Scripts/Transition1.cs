using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class Transition1 : MonoBehaviour {

	[Tooltip("The transforms that the system will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();		// Holds list of all transforms for transitions
	public static Transition1 Instance;							// Holds reference to this script
	[HideInInspector]
	public bool MoveCamera;										// Flag controlling vector3 damp movement

	private Vector3 velocity;									// Velocity for vector3 damp movement
	private ElevatorMotion userElevatorMotion;					// The user's elevator
	private Vector3 target;										// Custom target for the camera to move to
	private bool ready;											// Flag controlling if the transition should begin. Only start after the system transition is over
	private float startTime;									// Time the transition began
	private float buffer;										// Multiplier to buffer the target position with target.up
	private Constants.Configuration config;						// Holds referenct to the config file

	// Use this for initialization
	void Awake () {
		Instance = this;
		ready = false;
		MoveCamera = false;
		velocity = Vector3.zero;
		buffer = 0.0f;
	}

	void Start()
	{
		config = Constants.Configuration.Instance;
	}

	void Update() {
		if (!ready)
		{ return; }

		int Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.CameraTravelTime);
		float Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.CameraTravelTime - Scene, 1.0f);

		if (Scene < Keys.Count * 2)
		{
			UpdateCamera(Scene, Blend);
		}
		else if (MoveCamera)
		{
			//transform.localPosition = Vector3.Lerp(transform.localPosition, target.position + target.up * buffer, 0.5f);
			transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, Time.deltaTime);
			//transform.localPosition = Vector3.Lerp(transform.localPosition, target.position + target.up, 1);
			//transform.localPosition = Vector3.SmoothDamp(transform.position, target.position, ref velocity, 5);
			//if (Vector3.Distance(transform.localPosition, target.localPosition) < 0.00003f)
			//{
			//	moveCamera = false;
			//}
		}
	}

	/// <summary>
	/// Add transform to the Keys list
	/// </summary>
	/// <param name="_transform">Transform.</param>
	public void UpdateKeys(Transform _transform, float _buffer = 0.0f)
	{
		buffer = _buffer;
		Keys.Add(_transform);
	}


	/// <summary>
	/// Start the transition
	/// </summary>
	public void BeginTransition()
	{
		startTime = Time.unscaledTime;
		ready = true;
	}

	/// <summary>
	/// Move camera to specified target over time
	/// </summary>
	/// <param name="target">Target.</param>
	public void MoveCameraToElevator()
	{
		GameObject[] elevators = GameObject.FindGameObjectsWithTag("Elevator");

		foreach (GameObject elevator in elevators)
		{
			if (elevator.GetComponent<ElevatorMotion>().UserElevator == true)
			{
				userElevatorMotion = elevator.GetComponent<ElevatorMotion>();
				target = elevator.transform.TransformVector(elevator.transform.TransformVector(userElevatorMotion.CableTop));
				MoveCamera = true;
				break;
			}
		}
	}

	/// <summary>
	/// Update the camera position
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="blend">Blend.</param>
	private void UpdateCamera(int scene, float blend)
	{
		int index = (int)Mathf.Floor(scene/2);

		if (scene % 2 == 0 || index == Keys.Count-2)
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index].position  + Keys[index].up * buffer, Mathf.Pow(blend, 1f));
		}
		else if (index < Keys.Count-1)
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index+1].position + Keys[index+1].up * buffer, Mathf.Pow(blend, 1f));
		}
	}
}
