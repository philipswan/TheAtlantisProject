using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereManager : MonoBehaviour {

	public static AtmosphereManager Instance;
	public Material Atmosphere; 
	public int subdivisions = 0;
	public float radius = 1f;

	// Use this for initialization
	void Start () {
		Instance = this;
		GetComponent<MeshFilter>().mesh = OctahedronSphereCreator.Create(subdivisions, radius);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMaterial()
	{
		GetComponent<MeshRenderer>().material = Atmosphere;
	}
}
