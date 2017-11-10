using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerState : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// returns true if the primary button (typically “A”) is currently pressed.
		OVRInput.Get(OVRInput.Button.One); 

		// returns true if the primary button (typically “A”) was pressed this frame.
		OVRInput.GetDown(OVRInput.Button.One); 

		// returns true if the “X” button was released this frame.
		OVRInput.GetUp(OVRInput.RawButton.X); 

		// returns a Vector2 of the primary (typically the Left) thumbstick’s current state. 
		// (X/Y range of -1.0f to 1.0f)
		OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick); 

		// returns true if the primary thumbstick is currently pressed (clicked as a button)
		OVRInput.Get(OVRInput.Button.PrimaryThumbstick); 

		// returns true if the primary thumbstick has been moved upwards more than halfway.  
		// (Up/Down/Left/Right - Interpret the thumbstick as a D-pad).
		OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp); 

		// returns a float of the secondary (typically the Right) index finger trigger’s current state.  
		// (range of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger); 

		// returns a float of the left index finger trigger’s current state.  
		// (range of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger); 

		// returns true if the left index finger trigger has been pressed more than halfway.  
		// (Interpret the trigger as a button).
		OVRInput.Get(OVRInput.RawButton.LIndexTrigger); 

		// returns true if the secondary gamepad button, typically “B”, is currently touched by the user.
		OVRInput.Get(OVRInput.Touch.Two);

		// returns true after a Gear VR touchpad tap
		OVRInput.GetDown(OVRInput.Button.One);

		// returns true on the frame when a user’s finger pulled off Gear VR touchpad controller on a swipe down
		OVRInput.GetDown(OVRInput.Button.DpadDown);

		// returns true the frame AFTER user’s finger pulled off Gear VR touchpad controller on a swipe right
		OVRInput.GetUp(OVRInput.RawButton.DpadRight);

		// returns true if the Gear VR back button is pressed
		OVRInput.Get(OVRInput.Button.Two);    

		// Returns true if the the Gear VR Controller trigger is pressed down
		OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);

		// Queries active Gear VR Controller touchpad click position 
		// (normalized to a -1.0, 1.0 range, where -1.0, -1.0 is the lower-left corner)
		OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);

		// If no controller is specified, queries the touchpad position of the active Gear VR Controller
		OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

		// returns true if the Gear VR Controller back button is pressed
		OVRInput.Get(OVRInput.Button.Back);   

		// recenters the active Gear VR Controller. Has no effect for other controller types.
		OVRInput.RecenterController();

		// recenters right Gear VR Controller (even if it is not active)
		OVRInput.RecenterController(OVRInput.Controller.RTrackedRemote);

		// returns true on the frame when a user’s finger pulled off Gear VR Controller back button
		OVRInput.GetDown(OVRInput.Button.Back);
	}
}
