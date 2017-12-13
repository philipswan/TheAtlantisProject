using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabitatManager : MonoBehaviour {

	public List<Material> HighlightMaterial = new List<Material>();		// Regular materials
	public List<Material> DefaultMaterial = new List<Material>();		// Regarul materials + highlight material

	private bool highlited;												// Current materials used

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
	public void SetMaterials()
	{
		// We only need to change the largest part of the habitat
		if (highlited)
		{
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials = DefaultMaterial.ToArray();
		}
		else
		{
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials = HighlightMaterial.ToArray();
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.5f);
		}

		highlited = ! highlited;
	}
}
