using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetColliderActive()
	{
		if (GetComponent<MeshCollider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
		}
		MeshCollider mc = GetComponent<MeshCollider>();
		mc.enabled = true;
		mc.inflateMesh = true;
		mc.convex = true;
	}
}
