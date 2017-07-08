using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Renderer renderer = go.GetComponent<Renderer>();
        //renderer.material = Resources.Load("TestMaterial") as Material;
        //renderer.material.mainTexture = Resources.Load("glass") as Texture;
        renderer.material = Resources.Load("earthMat") as Material;
        //renderer.material.mainTexture = Resources.Load("NewEarthMat") as Texture;
        renderer.material.SetTextureScale("_MainTex", new Vector2(4f, 4f));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
