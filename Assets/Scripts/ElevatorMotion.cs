using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ElevatorMotion : MonoBehaviour {
	
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

	public Target PreviousTarget			// Enum holding the current target (or destination) of the elevator
	{get; private set;}    	

	private Vector3 velocity;           	// Velocity of the object when starting movement
	private Vector3 targetPos;          	// Position of the destination of the elevator
	private bool targetSet;             	// Flag set when both targets have been set from ElevatorCables.cs
	private Vector3 button1Pos;				// Holds button1 position to prevent drifting when moving
	private Vector3 button2Pos;				// Holds button2 position to prevent drifting when moving
	private GameObject backgroundMusic;		// Holds reference to background clip game object
	private GameObject button1;
	private GameObject button2;
	private float botBuffer;				// Distance between carrier and elevator to stop motion
	private float topBuffer;				// Distance between ring and elevator to stop motion
	private float buffer;					// Holds current buffer
	private float startWaitTime;			// When the elevator reached it's destination
	private Constants.Configuration config;	// Holds reference to config script

	// Use this for initialization on creation
	void Awake () {
		CurrentTarget = Target.Bottom;
		PreviousTarget = CurrentTarget;

		automatic = false;
		UserElevator = false;

		velocity = Vector3.zero;
		botBuffer = 0.00015f;
		topBuffer = 0.0003f;
	}

	// Use this for initialization on first frame
	void Start()
	{
		config = Constants.Configuration.Instance;

		// Store button positions to prevent drift in update method
		button1Pos = transform.GetChild(0).Find("Button 1").localPosition;
		button2Pos = transform.GetChild(0).Find("Button 2").localPosition;

		// Store button gameobject references
		button1 = transform.GetChild(0).Find("Button 1").gameObject;
		button2 = transform.GetChild(0).Find("Button 2").gameObject;
	}

	// Update is called once per frame
	void Update () {
		// If the targets have not been created and set yet, return. Otherwise we will get null errors
		if (!targetSet || !automatic)
		{
			return;
		}

		bool update = false;
		// Move towards the current target using SmoothDamp
		switch (CurrentTarget)
		{
		case Target.Top:
			targetPos = CableTop;
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, CableTop, ref velocity, config.ElevatorTravelTime);
			break;
		case Target.Bottom:
			targetPos = CableBotton;
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, CableBotton, ref velocity, config.ElevatorTravelTime);
			break;
		case Target.Nothing:
			//print(Time.unscaledTime - startWaitTime);
			// If we haven't waited long enough, set the targetPos to key value to prevent UpdatePosition from getting
			// called. Else, set the target and localPos to the same
			if (Time.unscaledTime - startWaitTime < config.ElevatorWaitTime)
			{
				//print("waiting");
				targetPos = new Vector3(999, 999, 999);
			}
			else
			{
				//print("here");
				//targetPos = transform.localPosition;
				update = true;
			}
			break;
		default:
			break;
		}

		// Keep buttons in the same local position to prevent drift
		button1.transform.localPosition = button1Pos;
		button2.transform.localPosition = button2Pos;

		// If we are close enough to the current target, switch to the other one
		if (Vector3.Distance(transform.localPosition, targetPos) <= 1e-5f || update)
		{
			update = false;
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
		Target target = CurrentTarget == Target.Nothing ? PreviousTarget : CurrentTarget;
		PreviousTarget = CurrentTarget;

		if (CurrentTarget != Target.Nothing)
		{
			startWaitTime = Time.unscaledTime;
			CurrentTarget = Target.Nothing;
			return;
		}
			
		// Switch the target position
		switch (target)
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
	/// Start elevator movement from button press
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
