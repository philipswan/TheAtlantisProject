using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayManager : MonoBehaviour {

	public List<Material> defaultMaterials;		// Regular materials
	public List<Material> highlightMaterials;	// Regarul materials + highlight material

	private bool highlited;						// Current materials used

	// Use this for initialization
	void Start () {
		highlited = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Sets the materials.
	/// </summary>
	/// <param name="_default">Default.</param>
	/// <param name="_highlight">Highlight.</param>
	public void SetMaterialLists(List<Material> _default, List<Material> _highlight)
	{
		defaultMaterials = new List<Material>(_default);
		highlightMaterials = new List<Material>(_highlight);
	}

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
	public void SetMaterials()
	{
		if (highlited)
		{
			GetComponent<MeshRenderer>().materials = defaultMaterials.ToArray();
		}
		else
		{
			GetComponent<MeshRenderer>().materials = highlightMaterials.ToArray();
			GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 1000);
		}

		highlited = ! highlited;
	}
}
