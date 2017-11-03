using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePosition : MonoBehaviour
{

    public float RingLatitude = -40;
    private float EarthRadius = 1;
    private float RingAltitude = 32f / 6371f;
    private Vector3 EarthPosition;

    // Use this for initialization
    void Start()
    {
        //EarthPosition.x = -Mathf.Cos(RingLatitude * Mathf.PI / 180) / 2 * (EarthRadius + RingAltitude) / EarthRadius;
        //EarthPosition.y = -Mathf.Sin(RingLatitude * Mathf.PI / 180) / 2 * (EarthRadius + RingAltitude) / EarthRadius;
        //y = 0.4478496f;
        //EarthPosition.z = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.localPosition = EarthPosition;
        transform.Rotate(0, -0.013f * Time.deltaTime, 0);
    }

}