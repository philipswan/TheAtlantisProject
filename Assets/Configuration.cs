using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants{

	public enum Target                  // Enum of target types
	{
		Top,
		Bottom
	};

	public class Configuration : MonoBehaviour {

		[Header("Movement")]
		[Tooltip("Lerp time for camera movment")]
		public int CameraTravelTime = 5;

		[Tooltip("Lerp time for system movement")]
		public int SystemTravelTime = 10;

		[Tooltip("Lerp time for tram movement")]
		public int TramTravelTime = 10;

		[Header("Ring Constants")]
		[Tooltip("Latitude of the ring")]
		public float RingLatitude = -34f;

		public static Configuration Instance;

		void Awake()
		{
			Instance = this;
		}
	}
}
