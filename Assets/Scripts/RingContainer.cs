using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingContainer : MonoBehaviour {
	public bool UpdateScale = false;

	// Use this for initialization
	void Start () {
		List<Vector3> children = new List<Vector3>();

		for (int i=0; i<transform.childCount; i++)
		{
			children.Add(transform.GetChild(i).position);
		}

		transform.position = GameObject.FindGameObjectWithTag("Positioner").transform.position;

		for (int i=0; i<transform.childCount; i++)
		{
			transform.GetChild(i).position = children[i];
		}
			
		if (UpdateScale)
		{
			transform.localScale = new Vector3(0.99965f, 0.99985f, 1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
