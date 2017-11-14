using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActivity : MonoBehaviour {

	public UnityEvent ButtonAction;		// Action to perform when the elevator button is pressed

	private bool eventCalled;			// Was the event already called this press?

	// Use this for initialization
	void Start () {
		eventCalled = false;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other)
	{		
		if (!eventCalled)
		{
			if ((OVRInput.Get(OVRInput.RawButton.LHandTrigger) && other.name == "hand_left") 
				|| (OVRInput.Get(OVRInput.RawButton.RHandTrigger) && other.name == "hand_right"))
			{
				GetComponent<AudioSource>().Play();

				ButtonAction.Invoke();
				eventCalled = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		eventCalled = false;
	}
}
