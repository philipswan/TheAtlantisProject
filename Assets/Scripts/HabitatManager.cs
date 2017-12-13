using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabitatManager : MonoBehaviour {

	public List<Material> HighlightMaterial = new List<Material>();
	public List<Material> DefulatMaterial = new List<Material>();

	private bool highlited;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMaterials()
	{
		if (highlited)
		{
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials = DefulatMaterial.ToArray();
		}
		else
		{
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials = HighlightMaterial.ToArray();
			transform.GetChild(0).transform.GetChild(7).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.5f);
		}

		highlited = ! highlited;
	}
}
