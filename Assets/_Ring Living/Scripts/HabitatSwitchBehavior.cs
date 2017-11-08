using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NewtonVR;

public class HabitatSwitchBehavior : MonoBehaviour
{

	public NVRSwitch nvrSwitch;
	public bool lastState = true;

	void Awake()
	{
		nvrSwitch = this.gameObject.GetComponent<NVRSwitch>();

		if (nvrSwitch == null)
			Debug.Log("WTF???");
		
		Debug.Log(nvrSwitch.CurrentState);
		lastState = nvrSwitch.CurrentState;
	}

	void Update ()
	{
		if (nvrSwitch.CurrentState != lastState)
		{
			lastState = !lastState;
			//EventsManagerBehavior.instance.ToggleTrigger("habitat_switch", lastState);
		}
	}
}
