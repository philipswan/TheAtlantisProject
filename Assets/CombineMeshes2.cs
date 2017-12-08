using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
		print(meshFilters.Length);
		gameObject.AddComponent<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
		var index = 0;

		for (var i = 0; i < meshFilters.Length; i++)
		{
			if (meshFilters[i].sharedMesh == null) continue;
			combine[index].mesh = meshFilters[i].sharedMesh;
			//combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
			//meshFilters[i].renderer.enabled = false;
		}

		GetComponent<MeshFilter>().mesh = new Mesh();
		GetComponent<MeshFilter>().mesh.CombineMeshes (combine);
		GetComponent<MeshRenderer>().material = meshFilters[1].GetComponent<MeshRenderer>().sharedMaterial;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
