using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ElevatorMotion : MonoBehaviour {

	[Tooltip("Time in seconds the elevator will take to travel from the ring to the ship or vice versa.")]
	public float TravelTime = 10;

	public bool automatic;				// Should the elevator move automatically

	public Vector3 CableTop             // Position at the top the of the cable that the elevator should travel to (the ring)
	{get; private set;}

	public Vector3 CableBotton          // Position at the bottom of the cable that the elevator should travel to (the ship)
	{get; private set;}

	public bool UserElevator			// Is this the user's elevator?
	{get; private set;}

	public Target CurrentTarget			// Enum holding the current target (or destination) of the elevator
	{get; private set;}    			  

	private Vector3 velocity;           // Velocity of the object when starting movement
	private Vector3 targetPos;          // Position of the destination of the elevator
	private bool targetSet;             // Flag set when both targets have been set from ElevatorCables.cs
	private Vector3 button1Pos;			// Holds button1 position to prevent drifting when moving
	private Vector3 button2Pos;			// Holds button2 position to prevent drifting when moving


	// Use this for initialization
	void Awake () {
		CurrentTarget = Target.Bottom;
		velocity = Vector3.zero;
		automatic = false;
		UserElevator = false;
	}

	void Start()
	{
		button1Pos = transform.FindChild("Button 1").localPosition;
		button2Pos = transform.FindChild("Button 2").localPosition;
	}

	// Update is called once per frame
	void Update () {
		// If the targets have not been created and set yet, return. Otherwise we will get null errors
		if (!targetSet || !automatic)
		{
			return;
		}

		// Move towards the current target using SmoothDamp
		switch (CurrentTarget)
		{
		case Target.Top:
			targetPos = CableTop;
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, CableTop, ref velocity, TravelTime);
			break;
		case Target.Bottom:
			targetPos = CableBotton;
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, CableBotton, ref velocity, TravelTime);
			break;
		default:
			targetPos = CableTop;
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, CableTop, ref velocity, TravelTime);
			break;
		}

		transform.GetChild(1).transform.localPosition = button1Pos;
		transform.GetChild(2).transform.localPosition = button2Pos;

		// If we are close enough to the current target, switch to the other one
		if (Vector3.Distance(transform.localPosition, targetPos) < 0.0003f)
		{
			UpdateTarget();
			if (UserElevator)
			{
				Transition1.Instance.MoveCamera = false;
				automatic = false;

				GetComponent<AudioSource>().Stop();
			}
		}
	}

	/// <summary>
	/// Change target for the elevator to move towards
	/// </summary>
	public void UpdateTarget()
	{
		targetSet = true;

		// Switch the target position
		switch (CurrentTarget)
		{
		case Target.Top:
			CurrentTarget = Target.Bottom;
			break;
		case Target.Bottom:
			CurrentTarget = Target.Top;
			break;
		default:
			CurrentTarget = Target.Bottom;
			break;
		}
	}

	/// <summary>
	/// Set destinations for the elevators and start them
	/// </summary>
	/// <param name="_cableTop">Cable top.</param>
	/// <param name="_cableBot">Cable bot.</param>
	/// <param name="_automatic">If set to <c>true</c> automatic.</param>
	public void SetPositions(Vector3 _cableTop, Vector3 _cableBot, bool _automatic, bool _userElevator = false)
	{
		CableTop = _cableTop;
		CableBotton = _cableBot;
		automatic = _automatic;
		UserElevator = _userElevator;

		if (!automatic)
		{
			//transform.localPosition = CableBotton;
			//transform.localPosition -= transform.forward * 0.001f;
		}

		UpdateTarget();
	}

	/// <summary>
	/// Start elevator movement
	/// </summary>
	public void StartElevator()
	{
		automatic = true;

		GetComponent<AudioSource>().Play();
	}
}
