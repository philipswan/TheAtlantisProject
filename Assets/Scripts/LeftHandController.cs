using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandController : MonoBehaviour {

	private Collider collider;

	// Use this for initialization
	void Start () {
		collider = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerStay(Collider other)
	{
		if (OVRInput.Get(OVRInput.RawButton.LHandTrigger) && other.gameObject.name == "home_button_PLY")
		{

		}
	}

	private void CheckState()
	{
		// returns true if the “X” button was released this frame.
		OVRInput.GetUp(OVRInput.RawButton.X); 

		// returns true if the “X” button was Pressed this frame.
		OVRInput.GetDown(OVRInput.RawButton.X); 

		// returns true if the “Y” button was released this frame.
		OVRInput.GetUp(OVRInput.RawButton.Y); 

		// returns true if the “Y” button was Pressed this frame.
		OVRInput.GetDown(OVRInput.RawButton.Y); 

		// returns a Vector2 of the primary (typically the Left) thumbstick’s current state. 
		// (X/Y range of -1.0f to 1.0f)
		OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick); 

		// returns true if the primary thumbstick is currently pressed (clicked as a button)
		OVRInput.Get(OVRInput.Button.PrimaryThumbstick); 

		// returns true if the primary thumbstick has been moved upwards more than halfway.  
		// (Up/Down/Left/Right - Interpret the thumbstick as a D-pad).
		OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp); 

		// returns a float of the left index finger trigger’s current state.  
		// (range of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger); 

		// returns true if the left index finger trigger has been pressed more than halfway.  
		// (Interpret the trigger as a button).
		OVRInput.Get(OVRInput.RawButton.LIndexTrigger); 

		// returns a float of the left hand trigger's current state.
		// (ragne of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);

		// returns true if the left hand trigger has been pressed more than halfway.  
		// (Interpret the trigger as a button).
		OVRInput.Get(OVRInput.RawButton.LHandTrigger);
	}
}
