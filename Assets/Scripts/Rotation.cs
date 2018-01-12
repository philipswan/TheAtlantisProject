using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
	public static Rotation Instance;

	// Use this for initialization
	void Start () {
		Instance = this;

		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(-0.006f * Time.deltaTime, 0, 0.03125f * Time.deltaTime);
    }
}
