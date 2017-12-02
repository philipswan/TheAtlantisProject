using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTransition : MonoBehaviour {
	public Transform FinalKey;						// Transform holding scale info for sphere to be reset to
	public int UppserScaleLimit = 10900;			// How large the user is allowed to scale the system
	public int LowerScaleLimit = 500;					// How small the user is allowed to scale the system

	private GameObject player;						// Holds reference to player object

	private Vector3 velocity;						// Velocity cap for smoothdamp
	private Vector3 initialPlayerPosition;			// Position of player's original positoin to be reset to
	private bool resettingScale;					// Flag to check if scale reset coroutine is running
	private bool resettingPosition;					// Flag to check if position reset coroutine is running

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		initialPlayerPosition = player.transform.position;
	}

	// Use this for initialization
	void Start () {
		resettingScale = false;
		resettingPosition = false;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		// Control system scale
		if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LHandTrigger))
		{
			UpdateScale(false);
		}
		else if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RHandTrigger))
		{
			UpdateScale(true);
		}
		else if(OVRInput.GetDown(OVRInput.RawButton.X) || OVRInput.GetDown(OVRInput.RawButton.Y))
		{
			if (!resettingScale)
			{
				StartCoroutine("ResetScale");
			}
		}

		// Toggle automatic rotation
		if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
		{
			Rotation.Instance.enabled = !Rotation.Instance.enabled;
		}
		// Reset player position
		else if(OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.B))
		{
			if (!resettingPosition)
			{
				StartCoroutine("ResetPosition");
			}
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

		resettingScale = false;
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

		resettingPosition = false;
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

		resettingPosition = false;
	}

	/// <summary>
	/// Resets the scale of the system.
	/// </summary>
	private IEnumerator ResetScale()
	{
		resettingScale = true;
		float travelTime = Vector3.Distance(transform.localScale, FinalKey.localScale) * 5e-5f;

		while (transform.localScale != FinalKey.localScale && resettingScale)
		{
			transform.localScale = Vector3.SmoothDamp(transform.localScale, FinalKey.localScale, ref velocity, travelTime);
			yield return null;
		}

		resettingScale = false;
	}

	/// <summary>
	/// Resets the position of the player's position
	/// </summary>
	/// <returns>The position.</returns>
	private IEnumerator ResetPosition()
	{
		resettingPosition = true;
		float travelTime = Vector3.Distance(player.transform.localPosition, initialPlayerPosition);

		while (player.transform.position != initialPlayerPosition && resettingPosition)
		{
			player.transform.position = Vector3.SmoothDamp(player.transform.position, initialPlayerPosition, ref velocity, Mathf.Min(travelTime/2, 10));
			yield return null;
		}

		resettingPosition = false;
	}
}
