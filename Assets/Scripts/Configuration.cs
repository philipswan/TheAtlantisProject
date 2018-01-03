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

		[Header("System Movement")]
		[Tooltip("Lerp time for camera movment")]
		public float CameraTravelTime = 5.0f;

		[Tooltip("Lerp time for system movement")]
		public float SystemTravelTime = 10.0f;

        [Header("Elevator Movement")]
        [Tooltip("Time in seconds the elevator will take to travel from the ring to the ship or vice versa.")]
		public float ElevatorTravelTime = 10.0f;

		[Tooltip("Time the elevator waits at it's destination")]
		public float ElevatorWaitTime = 0.0f;

        [Header("Tram Movement")]
        [Tooltip("Rate of acceleration for trams (m/s^2)")]
		public float TramAcceleration = 11.7f;

		[Tooltip("Top speed for trams (m/s)")]
		public float TramVelocity = 223.5f;

        [Tooltip("Time tram waits at habitats(s)")]
        public float TramWaitTime = 5f;

        [Header("Ring Constants")]
		[Tooltip("Latitude of the ring")]
		public float RingLatitude = -34.0f;

        private float transitRingDiamter;

        private float ringDiameter;

		public static Configuration Instance;

        public float TransitRingDiamter
        {
            get
            {
                return transitRingDiamter;
            }

            set
            {
                transitRingDiamter = value;
            }
        }

        public float RingDiameter
        {
            get
            {
                return ringDiameter;
            }

            set
            {
                ringDiameter = value;
            }
        }

        void Awake()
		{
			Instance = this;

            ringDiameter = -1;
            transitRingDiamter = -1;
		}
	}
}
