using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour {

	public List<Material> DefaultMaterials = new List<Material>();		// Regular materials
	public List<Material> HighlightMaterials = new List<Material>();	// Regarul materials + highlight material

	private bool highlited;												// Current materials used

	// Use this for initialization
	void Start () {
		tag = "Elevator";
		highlited = false;
		transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefaultMaterials.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
	public void SetMaterials()
	{
		if (highlited)
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefaultMaterials.ToArray();
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.005f);
		}

		highlited = ! highlited;
	}
}
