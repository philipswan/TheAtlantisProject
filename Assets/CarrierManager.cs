using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierManager : MonoBehaviour {

	public List<Material> DefulatMaterials = new List<Material>();
	public List<Material> HighlightMaterials = new List<Material>();

	bool highlited;

	// Use this for initialization
	void Start () {
		highlited  = false;
		transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefulatMaterials.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMaterials()
	{
		if (highlited)
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefulatMaterials.ToArray();
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.001f);
		}

		highlited = ! highlited;
	}
}
