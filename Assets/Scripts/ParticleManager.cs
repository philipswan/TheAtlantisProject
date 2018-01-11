using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

	private float blend;
	private float startTime;
	private Vector3 targetScale;

	// Use this for initialization
	void Start () {
		startTime = -1;
		targetScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (startTime != -1)
		{
			blend = Mathf.Min((Time.unscaledTime - startTime) / 7 ,1);

			if (blend < 1)
			{
				UpdateSystem(blend);
			}
			if (blend == 1)
			{
				UpdateSystem(blend);
				enabled = false;
			}
		}
	}

	public void StartLaunch()
	{
		startTime = Time.unscaledTime;
		GetComponent<ParticleSystem>().Play();

		if (transform.childCount > 0)
		{
			for (int i=0; i<transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	/// <summary>
	/// Move the system to the proper transform
	/// </summary>
	/// <param name="blend">Blend.</param>
	private void UpdateSystem(float blend)
	{
		transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, blend);
	}
}
