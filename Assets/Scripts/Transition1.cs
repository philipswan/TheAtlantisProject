using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1 : MonoBehaviour {

	[Tooltip("The transforms that the system will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();		// Holds list of all transforms for transitions
	public float TotalTime = 10.0f;								// Time for each transition
	public static Transition1 Instance;							// Holds reference to this script

	private Vector3 velocity;									// Velocity for vector3 damp movement
	private Transform target;									// Custom target for the camera to move to
	private bool ready;											// Flag controlling if the transition should begin. Only start after the system transition is over
	private bool moveCamera;									// Flag controlling vector3 damp movement
	private float startTime;									// Time the transition began

	// Use this for initialization
	void Awake () {
		Instance = this;
		ready = false;
		moveCamera = false;
		velocity = Vector3.zero;
	}

	void Update() {
		if (!ready)
		{ return; }

		int Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / TotalTime);
		float Blend = Mathf.Min ((Time.unscaledTime - startTime) / TotalTime - Scene, 1.0f);

		if (Scene < Keys.Count * 2)
		{
			UpdateCamera(Scene, Blend);
		}
		else if (moveCamera)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.position, target.position, ref velocity, 10);
			if (Vector3.Distance(transform.position, target.position) < 1)
			{
				moveCamera = false;
			}
		}
	}

	/// <summary>
	/// Add transform to the Keys list
	/// </summary>
	/// <param name="_transform">Transform.</param>
	public void UpdateKeys(Transform _transform)
	{
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
	public void MoveCamera(Transform _target)
	{
		target = _target;
		moveCamera = true;
	}

	/// <summary>
	/// Update the camera position
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="blend">Blend.</param>
	private void UpdateCamera(int scene, float blend)
	{
		int index = (int)Mathf.Floor(scene/2);

		if (scene % 2 == 0 || index == Keys.Count-1)
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index].position, Mathf.Pow(blend, 1f));
		}
		else if (index < Keys.Count-1)
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index+1].position, Mathf.Pow(blend, 1f));
		}
	}
}
