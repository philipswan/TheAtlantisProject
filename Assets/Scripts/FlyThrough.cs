using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyThrough : MonoBehaviour {

    public Waypoints KeyManager;
    public float TotalTime = 10.0f;
    public int Mode = 0;
    public Transform key5;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        switch (Mode)
        {
            case 0:
            case 1:
            case 2:
            default:
                transform.position = KeyManager.waypoints[Mode].transform.position;
                transform.rotation = KeyManager.waypoints[Mode].transform.rotation;
                transform.localScale = KeyManager.waypoints[Mode].transform.localScale;
                //transform.localScale.Set(100.0f / KeyManager.waypoints[Mode].transform.localScale.x, 100.0f / KeyManager.waypoints[Mode].transform.localScale.y, 100.0f / KeyManager.waypoints[Mode].transform.localScale.z);
                //transform.localScale.Set(key5.localScale.x, key5.localScale.y, key5.localScale.z);
                break;
        }
    }
}
