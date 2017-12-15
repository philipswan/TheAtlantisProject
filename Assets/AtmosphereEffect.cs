using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereEffect : MonoBehaviour {

	private const int earthAtmHeight = 60000;
	private const int earthRadius = 6371000;

	private const float kr = 1;
	private const float km = 1;
	private const float g = -0.75f;

	private float systemAtmHeight;
	private float systemRadius;
	private float cameraHeight;
	private float scaling;
	private float eSun;
	private float scale;

	private GameObject sun;

	private Transform system;

	private Vector3 lsHeading;
	private Vector3 lsDirection;

	private Material ae;

	private SphereCollider sc;

	// Use this for initialization
	void Start () {
		sun = GameObject.FindGameObjectWithTag("Sun");
		sc = GetComponent<SphereCollider>();
		system = transform.parent;
		ae = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (ae == null)
		{ return; }

		systemRadius = system.localScale.x / 2;
		scaling = systemRadius / earthRadius;
		systemAtmHeight = earthAtmHeight * scaling + systemRadius;

		lsHeading = sun.transform.position - transform.position;
		lsDirection = lsHeading / lsHeading.magnitude;

		cameraHeight = Vector3.Distance(Camera.main.transform.position, sc.ClosestPoint(Camera.main.transform.position));

		eSun = sun.GetComponent<Light>().intensity;

		scale = 1 / (systemAtmHeight - systemRadius);

		ae.SetVector("_CameraPosition", Camera.main.transform.position);
		ae.SetVector("_LightDir", lsDirection);
		ae.SetFloat("_CameraHeight", cameraHeight);
		ae.SetFloat("_CameraHeight2", cameraHeight * cameraHeight);
		ae.SetFloat("_OuterRadius", systemAtmHeight);
		ae.SetFloat("_OuterRadius2", systemAtmHeight * systemAtmHeight);
		ae.SetFloat("_InnerRadius", systemRadius);
		ae.SetFloat("_InnerRadius2", systemRadius * systemRadius);
		ae.SetFloat("_KrESun", km * eSun);
		ae.SetFloat("_KmESun", km * eSun);
		ae.SetFloat("_Kr4PI", kr * 4 * Mathf.PI);
		ae.SetFloat("_Km4PI", km * 4 * Mathf.PI);
		ae.SetFloat("_Scale", scale);
		ae.SetFloat("_ScaleDepth", 0.25f);
		ae.SetFloat("_ScaleOverScaleDepth", scale / 0.25f);
		ae.SetFloat("_Samples", 2f);
		ae.SetFloat("_G", g);
		ae.SetFloat("_G2", g * g);

	}

	public void OnMaterialsSet()
	{
		ae = GetComponent<MeshRenderer>().materials[2];
	}
}
