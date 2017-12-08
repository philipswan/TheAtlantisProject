using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureManager : MonoBehaviour {
	public float MaxRaycastDistance = 10;
	public LayerMask Mask;

	private RaycastHit hit;
	private GameObject prevObject;
	private GameObject currentObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Physics.Raycast(transform.position, transform.forward, out hit, MaxRaycastDistance, Mask))
		{
			prevObject = currentObject;
			prevObject.SendMessage("OnGazeExited");

			currentObject = hit.transform.gameObject;
			currentObject.SendMessage("OnGazeEntered");
		}
	}
}
