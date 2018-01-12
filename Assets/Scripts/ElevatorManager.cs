using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour {

	private Material[] defaultMaterials = new Material[1];				// Regular materials
	private List<Material> highlightMaterials = new List<Material>();	// Regarul materials + highlight material

	private bool highlited;												// Current materials used

	// Use this for initialization
	void Start () {
		tag = "Elevator";
		highlited = false;

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
		if (highlited)
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = defaultMaterials;
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = highlightMaterials.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.005f);
		}

		highlited = ! highlited;
	}

	private void LoadMaterials()
	{
		defaultMaterials[0] = Resources.Load("defaultMat")  as Material;

		highlightMaterials.Add(defaultMaterials[0]);
		highlightMaterials.Add(Resources.Load("Silhouetted Diffuse") as Material);
	}
}
