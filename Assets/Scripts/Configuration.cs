using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants{

	public enum Target                  // Enum of target types
	{
		Top,
		Bottom,
		Nothing
	};

	public class Configuration : MonoBehaviour {

		[Header("Camera")]
		[Tooltip("Time to fade camera in/out")]
		public float FadeTime = 2.0f;

		[Header("Movement")]
		[Tooltip("Lerp time for camera movment")]
		public float CameraTravelTime = 5.0f;

		[Tooltip("Lerp time for system movement")]
		public float SystemTravelTime = 10.0f;

		[Tooltip("Time in seconds the elevator will take to travel from the ring to the ship or vice versa.")]
		public float ElevatorTravelTime = 10.0f;

		[Tooltip("Time the elevator waits at it's destination")]
		public float ElevatorWaitTime = 0.0f;

		[Tooltip("Lerp time for tram movement with stops")]
		public float TramTravelTimeStops = 10.0f;

		[Tooltip("Lerp time for tram movement without stops")]
		public float TramTravelTime = 10.0f;

		[Header("Ring Constants")]
		[Tooltip("Latitude of the ring")]
		public float RingLatitude = -34.0f;

		public static Configuration Instance;

		void Awake()
		{
			Instance = this;
		}
	}
}
