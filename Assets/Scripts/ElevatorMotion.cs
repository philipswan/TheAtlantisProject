using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ElevatorMotion : MonoBehaviour {

	[Tooltip("Time in seconds the elevator will take to travel from the ring to the ship or vice versa.")]
	public float TravelTime = 10;

	public bool automatic;					// Should the elevator move automatically
	public AudioClip ElevatorDing;			// Played when destination is reached
	public AudioClip ElevatorMusic;			// Played during the ride

	public Vector3 CableTop             	// Position at the top the of the cable that the elevator should travel to (the ring)
	{get; private set;}

	public Vector3 CableBotton          	// Position at the bottom of the cable that the elevator should travel to (the ship)
	{get; private set;}

	public bool UserElevator				// Is this the user's elevator?
	{get; private set;}

	public Target CurrentTarget				// Enum holding the current target (or destination) of the elevator
	{get; private set;}    			  

	private Vector3 velocity;           	// Velocity of the object when starting movement
	private Vector3 targetPos;          	// Position of the destination of the elevator
	private bool targetSet;             	// Flag set when both targets have been set from ElevatorCables.cs
	private Vector3 button1Pos;				// Holds button1 position to prevent drifting when moving
	private Vector3 button2Pos;				// Holds button2 position to prevent drifting when moving
	private GameObject backgroundMusic;		// Holds reference to background clip game object
	private float botBuffer;				// Distance between carrier and elevator to stop motion
	private float topBuffer;				// Distance between ring and elevator to stop motion
	private float buffer;					// Holds current buffer

	// Use this for initialization on creation
	void Awake () {
		CurrentTarget = Target.Bottom;
		velocity = Vector3.zero;
		automatic = false;
		UserElevator = false;
		botBuffer = 0.00015f;
		topBuffer = 0.0003f;
	}

	// Use this for initialization on first frame
	void Start()
	{
		// Store button positions to prevent drift in update method
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

		// Keep buttons in the same local position to prevent drift
		transform.GetChild(1).transform.localPosition = button1Pos;
		transform.GetChild(2).transform.localPosition = button2Pos;

		// If we are close enough to the current target, switch to the other one
		if (Vector3.Distance(transform.localPosition, targetPos) < buffer)
		{
			UpdateTarget();
			if (UserElevator)
			{
				Transition1.Instance.MoveCamera = false;
				automatic = false;

				GetComponent<AudioSource>().clip = ElevatorDing;
				GetComponent<AudioSource>().Play();
				backgroundMusic.GetComponent<AudioSource>().Play();	// Continue playing background music
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
			buffer = botBuffer;
			break;
		case Target.Bottom:
			CurrentTarget = Target.Top;
			buffer = topBuffer;
			break;
		default:
			CurrentTarget = Target.Bottom;
			buffer = botBuffer;
			break;
		}
	}

	/// <summary>
	/// Set destinations for the elevators and start them
	/// </summary>
	/// <param name="_cableTop">Cable top.</param>
	/// <param name="_cableBot">Cable bot.</param>
	/// <param name="_automatic">If set to <c>true</c> automatic.</param>
	/// <param name="_userElevator">If set to <c>true</c> user elevator.</param>
	public void SetPositions(Vector3 _cableTop, Vector3 _cableBot, bool _automatic, bool _userElevator = false)
	{
		CableTop = _cableTop;
		CableBotton = _cableBot;
		automatic = _automatic;
		UserElevator = _userElevator;

		UpdateTarget();	// Set the target of the elevator. If it is not the user elevator, this also sets it in motion
	}

	/// <summary>
	/// Start elevator movement
	/// </summary>
	public void StartElevator()
	{
		if (automatic)
		{ return; }

		automatic = true;

		// Find the background music, stop it and store its reference to play it later
		AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach( AudioSource audioS in sources) {
			if (audioS.gameObject.name == "BackgroundMusic")
			{
				backgroundMusic = audioS.gameObject;
				audioS.Stop();
				break;
			}
		}

		GetComponent<AudioSource>().clip = ElevatorMusic;
		GetComponent<AudioSource>().Play();
	}
}
