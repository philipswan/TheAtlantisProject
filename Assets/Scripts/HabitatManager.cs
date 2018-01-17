using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabitatManager : MonoBehaviour {

	private  Material[] defaultMaterial = new Material[1];				// Regarul materials + highlight material
	private List<Material> highlightMaterial = new List<Material>();	// Regular materials

	private bool highlited;												// Current materials used

	// Use this for initialization
	void Start () {
		LoadMaterials();
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
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = defaultMaterial;
			transform.GetChild(1).GetComponent<MeshRenderer>().materials = defaultMaterial;
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = highlightMaterial.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.01f);

			transform.GetChild(1).GetComponent<MeshRenderer>().materials = highlightMaterial.ToArray();
			transform.GetChild(1).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.01f);
		}

		highlited = ! highlited;
	}

	private void LoadMaterials()
	{
		defaultMaterial[0] = Resources.Load("01___Default") as Material;

		highlightMaterial.Add(defaultMaterial[0]);
		highlightMaterial.Add(Resources.Load("Silhouetted Diffuse") as Material);
	}
}
