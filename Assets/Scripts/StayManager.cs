using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayManager : MonoBehaviour {

	public List<Material> defaultMaterials;
	public List<Material> highlightMaterials;
	private bool highlited;

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
