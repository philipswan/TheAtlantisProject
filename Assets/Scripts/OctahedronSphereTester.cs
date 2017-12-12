using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OctahedronSphereTester : MonoBehaviour {

	public static OctahedronSphereTester Instance;
	public List<Material> Earthmaterials = new List<Material>();

	public int subdivisions = 0;
	public float radius = 1f;

	private void Awake () {
		Instance = this;
		GetComponent<MeshFilter>().mesh = OctahedronSphereCreator.Create(subdivisions, radius);
	}

	public void SetMaterials()
	{
		GetComponent<MeshRenderer>().materials = Earthmaterials.ToArray();
	}
}