using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTransition : MonoBehaviour {
	public Transform FinalKey;

	private float startTime;
	private float travelTime;
	private Vector3 velocity;
	private bool resettingScale;

	// Use this for initialization
	void Start () {
		resettingScale = false;
		travelTime = 1;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LHandTrigger))
		{
			UpdateScale(false);
		}
		else if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RHandTrigger))
		{
			UpdateScale(true);
		}
		else if(OVRInput.GetDown(OVRInput.RawButton.X) || OVRInput.GetDown(OVRInput.RawButton.Y))
		{
			if (!resettingScale)
			{
				StartCoroutine("ResetScale");
			}
		}
	}

	/// <summary>
	/// Updates the scale of the system. Called when triggers are pressed.
	/// </summary>
	/// <param name="scaleUp">If set to <c>true</c> scale up.</param>
	private void UpdateScale(bool scaleUp)
	{
		if (scaleUp)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + transform.localScale * 0.01f, 1);
		}
		else
		{
			transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale - transform.localScale * 0.01f, 1);
		}
		resettingScale = false;
	}

	private void UpdatePosition()
	{

	}

	/// <summary>
	/// Resets the scale of the system.
	/// </summary>
	private IEnumerator ResetScale()
	{
		resettingScale = true;
		float travelTime = Vector3.Distance(transform.localScale, FinalKey.localScale) * 5e-5f;
		print(Vector3.Distance(transform.localScale, FinalKey.localScale));
		while (transform.localScale != FinalKey.localScale && resettingScale)
		{
			transform.localScale = Vector3.SmoothDamp(transform.localScale, FinalKey.localScale, ref velocity, 1);
			yield return null;
		}

		resettingScale = false;
	}
}
