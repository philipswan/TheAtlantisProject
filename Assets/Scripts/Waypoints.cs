using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct Waypoint {
//    public Vector3 position;
//    public Quaternion rotation;
//    public Vector3 scale;
//} 

public class Waypoints : MonoBehaviour {

    [System.NonSerialized]
    public Transform[] waypoints;
    // Use this for initialization

    void Start()
    {
        waypoints = new Transform[10];
        for (int i = 0; i < waypoints.Length; i++) {
            GameObject obj = new GameObject("waypoint " + i);
            waypoints[i] = obj.transform;
            obj.transform.SetParent(transform);

            //    waypoints[i] = new Vector3(0, 0, 0);
            //.position.Set(0, 0, 0);

            //    waypoints[i].localRotation = new Quaternion(0, 0, 0, 0);
            //    waypoints[i].localScale= new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
