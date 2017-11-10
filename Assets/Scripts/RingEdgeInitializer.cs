using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEdgeInitializer : MonoBehaviour {
    
	private float EarthRadius = 1;
    private float RingAltitude = 32f / 6371f;
    private Vector3 EarthPosition;
	private Constants.Configuration config;			// Holds renference to config file

    // Use this for initialization
    void Start () {
		config = Constants.Configuration.Instance;

		EarthPosition.x = Mathf.Cos(config.RingLatitude * Mathf.PI / 180) / 2 * (EarthRadius + RingAltitude) / EarthRadius;
		EarthPosition.y = Mathf.Sin(config.RingLatitude * Mathf.PI / 180) / 2 * (EarthRadius + RingAltitude) / EarthRadius;
        //y = 0.4478496f;
        EarthPosition.z = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localPosition = EarthPosition;
    }
}
