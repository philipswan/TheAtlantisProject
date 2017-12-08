using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour {

	public static CombineMeshes Instance;
	public Material[] Materials;

	private List<GameObject> habitatsList = new List<GameObject>();
	private GameObject[] habitatsArray;
	// Use this for initialization
	void Start () {
		Instance = this;
//		Combine();
//		DestroyHabitats();
//		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
//		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
//
//		int i = 0;
//		while (i < meshFilters.Length) {
//			combine[i].mesh = meshFilters[i].sharedMesh;
//			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//			meshFilters[i].gameObject.active = false;
//			i++;
//		}
//
//		transform.GetComponent<MeshFilter>().mesh = new Mesh();
//		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
//		transform.gameObject.active = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void GetHabitats()
	{

	}

	//Similar to Unity's reference, but with different materials
	//http://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
	void Combine() 
	{
		//Lists that holds mesh data that belongs to each submesh
		List<CombineInstance> mat5 = new List<CombineInstance>();
		List<CombineInstance> mat17= new List<CombineInstance>();
		List<CombineInstance> mat21 = new List<CombineInstance>();
		List<CombineInstance> mat23= new List<CombineInstance>();

		//Get all meshfilters from this habitat, true to also find deactivated children
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);

		//Loop through all children
		for (int j = 0; j < meshFilters.Length; j++)
		{
			MeshFilter meshFilter = meshFilters[j];

			CombineInstance combine = new CombineInstance();

			//Determine material type
			MeshRenderer meshRender = meshFilter.GetComponent<MeshRenderer>();

			//Modify the material name, because Unity adds (Instance) to the end of the name
			string materialName = meshRender.material.name.Replace(" (Instance)", "");

			switch (materialName)
			{
			case "material_5___31749":
				combine.mesh = meshFilter.mesh;
				combine.transform = meshFilter.transform.localToWorldMatrix;

				//Add it to the list of mat5 mesh data
				mat5.Add(combine);
				break;
			case "material_17___31749":
				combine.mesh = meshFilter.mesh;
				combine.transform = meshFilter.transform.localToWorldMatrix;

				//Add it to the list of mat17 mesh data
				mat17.Add(combine);
				break;
			case "material_21___31749":
				combine.mesh = meshFilter.mesh;
				combine.transform = meshFilter.transform.localToWorldMatrix;

				//Add it to the list of mat21 mesh data
				mat21.Add(combine);
				break;
			case "material_23___31749":
				combine.mesh = meshFilter.mesh;
				combine.transform = meshFilter.transform.localToWorldMatrix;

				//Add it to the list of mat23 mesh data
				mat23.Add(combine);
				break;
			}
		}

		//First we need to combine the meshes
		Mesh combined5Mesh = new Mesh();
		combined5Mesh.CombineMeshes(mat5.ToArray());

		Mesh combined17Mesh = new Mesh();
		combined17Mesh.CombineMeshes(mat17.ToArray());

		Mesh combined21Mesh = new Mesh();
		combined5Mesh.CombineMeshes(mat21.ToArray());

		Mesh combined23Mesh = new Mesh();
		combined17Mesh.CombineMeshes(mat23.ToArray());

		//Create the array that will form the combined mesh
		CombineInstance[] totalMesh = new CombineInstance[4];

		totalMesh[0].mesh = combined5Mesh;
		totalMesh[0].transform = transform.localToWorldMatrix;
		totalMesh[1].mesh = combined17Mesh;
		totalMesh[1].transform = transform.localToWorldMatrix;
		totalMesh[2].mesh = combined21Mesh;
		totalMesh[2].transform = transform.localToWorldMatrix;
		totalMesh[3].mesh = combined23Mesh;
		totalMesh[3].transform = transform.localToWorldMatrix;

		//Create the final combined mesh
		Mesh combinedAllMesh = new Mesh();

		//Make sure it's set to false to get 2 separate meshes
		combinedAllMesh.CombineMeshes(totalMesh, false, true);
		gameObject.AddComponent<MeshFilter>();
		GetComponent<MeshFilter>().mesh = combinedAllMesh;
		GetComponent<MeshRenderer>().materials = Materials;
	}

	private void DestroyHabitats()
	{
		foreach (GameObject go in habitatsList)
		{
			Destroy(go.transform.parent.parent.gameObject);
		}
		habitatsList.Clear();
	}
}
