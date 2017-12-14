using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OctahedronSphereTester : MonoBehaviour {

	public static OctahedronSphereTester Instance;					// Reference to script
	public List<Material> EarthMaterials = new List<Material>();	// Earth materials

	public int subdivisions = 0;
	public float radius = 1f;

	private void Awake () {
		Instance = this;
		GetComponent<MeshFilter>().mesh = OctahedronSphereCreator.Create(subdivisions, radius);

		Vector3 center = GetComponent<MeshFilter>().mesh.bounds.center;
		Vector3 edge = GetComponent<MeshFilter>().mesh.vertices[0];
		print(GetComponent<MeshFilter>().mesh.bounds.extents);
		//float distance = Vector3.Distance(center, edge);
		//print(distance);
	}

	void Update()
	{
//		print(GetComponent<MeshFilter>().mesh.bounds.extents.magnitude * 5800);

//		Vector3 center = GetComponent<MeshFilter>().mesh.bounds.center;
//		Vector3 edge = GetComponent<MeshFilter>().mesh.vertices[0];
//		float distance = Vector3.Distance(center, edge);
//		print(distance);
	}

	/// <summary>
	/// Set sphere materials once the user starts the transition
	/// </summary>
	public void SetMaterials()
	{
		GetComponent<MeshRenderer>().materials = EarthMaterials.ToArray();
	}
}