using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingContainer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 childPos0 = Vector3.zero;
		Vector3 childPos1 = Vector3.zero;

		print(transform.childCount);

		if (transform.childCount > 0)
			childPos0 = transform.GetChild(0).position;
		if (transform.childCount > 1)
			childPos1 = transform.GetChild(1).position;
		
		transform.position = GameObject.FindGameObjectWithTag("Sphere").transform.position;

		if (transform.childCount > 0)
			transform.GetChild(0).position = childPos0;
		if (transform.childCount > 1)
			transform.GetChild(1).position = childPos1;		

		transform.localScale = new Vector3(.9997f, .99985f, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
