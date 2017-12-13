using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OctahedronSphereTester : MonoBehaviour {

	public static OctahedronSphereTester Instance;					// Reference to script
	public List<Material> Earthmaterials = new List<Material>();	// Earth materials

	public int subdivisions = 0;
	public float radius = 1f;

	private void Awake () {
		Instance = this;
		GetComponent<MeshFilter>().mesh = OctahedronSphereCreator.Create(subdivisions, radius);
	}

	/// <summary>
	/// Set sphere materials once the user starts the transition
	/// </summary>
	public void SetMaterials()
	{
		GetComponent<MeshRenderer>().materials = Earthmaterials.ToArray();
	}
}