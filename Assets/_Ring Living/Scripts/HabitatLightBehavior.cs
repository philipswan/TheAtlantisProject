using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NewtonVR;

// This class is garbage and should exist.  Lights and walls should have behaviors that register with an event registry.  The switch (or whatever) could set them off.
public class HabitatLightBehavior : MonoBehaviour {

	public float OnIntensity = 1f;
	public float OffIntensity = 0f;

	private void Start()
	{
		//EventsManagerBehavior.instance.ToggleListen("habitat_switch", Toggle);
	}

	void Toggle(bool data, EventArgs args)
	{
		Debug.Log("Ping from " + this.name + ": " + data);
		GetComponent<Light>().intensity = (data) ? OnIntensity : OffIntensity;
	}
}
