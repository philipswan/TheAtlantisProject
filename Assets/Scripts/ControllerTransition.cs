using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTransition : MonoBehaviour {

	public static ControllerTransition Instance;	// Reference to the script
	public Transform FinalSystemKey;				// Transform holding scale info for sphere to be reset to
	public Transform FinalCamerakey;				// Transform holding position info for the player to be reset to
	public int UppserScaleLimit = 10900;			// How large the user is allowed to scale the system
	public int LowerScaleLimit = 500;				// How small the user is allowed to scale the system

	private GameObject player;						// Holds reference to player object

	private Vector3 velocity;						// Velocity cap for smoothdamp
	private float fadeTime;							// Time for camera fade
	private bool resettingTransform;				// Flag to check if reset transform coroutine is running
	private Constants.Configuration config;			// Holds reference to config file

	void Awake() {
		Instance = this;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Use this for initialization
	void Start () {
		resettingTransform = false;
		velocity = Vector3.zero;

		config = Constants.Configuration.Instance;
		fadeTime = config.FadeTime;
	}
	
	// Update is called once per frame
	void Update () {
		// Toggle automatic rotation
		if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
		{
			Rotation.Instance.enabled = !Rotation.Instance.enabled;
		}

		// Only allow control if the transform is not being reset
		if (!resettingTransform)
		{
			// Button A is reserved for the floating menu
			if(OVRInput.GetDown(OVRInput.RawButton.X) || OVRInput.GetDown(OVRInput.RawButton.Y))
			{
				// Reset the scale
				StartCoroutine("ResetTransform");
				return;
			}
			// Control system scale
			if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LHandTrigger))
			{
				// Scale system up
				UpdateScale(false);
			}
			else if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RHandTrigger))
			{
				// Scale system down
				UpdateScale(true);
			}
				
			// Control the player's position
			float distance1 = Vector3.Distance(Vector3.zero, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
			if (distance1 > 0)
			{
				distance1 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < 0 ? -distance1 : distance1;
				UpdatePosition(distance1);
			}
			float distance2 = Vector3.Distance(Vector3.zero, OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));
			if (distance2 > 0)
			{
				UpdatePosition(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));
			}
		}
	}

	/// <summary>
	/// Toggle script enable
	/// </summary>
	public void Activate(bool _state)
	{
		enabled = _state;
	}

	/// <summary>
	/// Updates the scale of the system. Called when triggers are pressed.
	/// </summary>
	/// <param name="scaleUp">If set to <c>true</c> scale up.</param>
	private void UpdateScale(bool scaleUp)
	{
		if (scaleUp)
		{
			if (transform.localScale.x >= UppserScaleLimit)
			{ return; }

			transform.localScale = transform.localScale + transform.localScale * 0.01f;
		}
		else
		{
			if (transform.localScale.x <= LowerScaleLimit)
			{ return; }

			transform.localScale = transform.localScale - transform.localScale * 0.01f;
		}
	}

	/// <summary>
	/// Move camera in x-z plane
	/// </summary>
	/// <param name="input">Input.</param>
	private void UpdatePosition(Vector2 input)
	{
		input *= 0.1f;
		player.transform.position += Camera.main.transform.forward * input.y;
		player.transform.position += new Vector3(input.x, 0, 0);
	}

	/// <summary>
	/// Move camera on y axis
	/// </summary>
	/// <param name="dist">Dist.</param>
	private void UpdatePosition(float dist)
	{
		dist *= 0.1f;
		player.transform.position = new Vector3(player.transform.position.x,
												player.transform.position.y + dist,
												player.transform.position.z);
	}

	/// <summary>
	/// Resets the transform of the system.
	/// </summary>
	private IEnumerator ResetTransform()
	{
		if (Vector3.Distance(transform.localScale, FinalSystemKey.localScale) == 0 &&
			Vector3.Distance(player.transform.position, FinalCamerakey.position) == 0)
		{
			yield break;
		}

		resettingTransform = true;

		float startTime = Time.time;
		Camera.main.SendMessage("FadeCamera", false);

		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}

		transform.localScale = FinalSystemKey.localScale;
		player.transform.position = FinalCamerakey.position;

		startTime = Time.time;
		Camera.main.SendMessage("FadeCamera", true);

		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}

		resettingTransform = false;
	}
}
