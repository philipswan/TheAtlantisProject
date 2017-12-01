using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
	}

	private void CheckStatus()
	{
		// returns true if the primary button (typically “A”) is currently pressed.
		OVRInput.Get(OVRInput.Button.One); 

		// returns true if the primary button (typically “A”) was pressed this frame.
		OVRInput.GetDown(OVRInput.Button.One); 

		// returns true if the “B” button was released this frame.
		OVRInput.GetUp(OVRInput.Button.Two); 

		// returns true if the “B” button was Pressed this frame.
		OVRInput.GetDown(OVRInput.Button.Two); 

		// returns a Vector2 of the secondary (typically the Right) thumbstick’s current state. 
		// (X/Y range of -1.0f to 1.0f)
		OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick); 

		// returns true if the secondary thumbstick is currently pressed (clicked as a button)
		OVRInput.Get(OVRInput.Button.SecondaryThumbstick); 

		// returns true if the secondary thumbstick has been moved upwards more than halfway.  
		// (Up/Down/Left/Right - Interpret the thumbstick as a D-pad).
		OVRInput.Get(OVRInput.Button.SecondaryThumbstick); 

		// returns a float of the right index finger trigger’s current state.  
		// (range of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger); 

		// returns true if the right index finger trigger has been pressed more than halfway.  
		// (Interpret the trigger as a button).
		OVRInput.Get(OVRInput.RawButton.RIndexTrigger); 

		// returns a float of the right hand trigger's current state.
		// (ragne of 0.0f to 1.0f)
		OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);

		// returns true if the right hand trigger has been pressed more than halfway.  
		// (Interpret the trigger as a button).
		OVRInput.Get(OVRInput.RawButton.RHandTrigger);
	}
}
