using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuManager : MonoBehaviour {

	private bool activated;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(OVRInput.GetDown(OVRInput.Button.Start))
		{
			if (activated)
			{
				DeactivateChildren();
			}
			else
			{
				ActivateChildren();
			}
		}
	}

	/// <summary>
	/// Activates the children.
	/// </summary>
	public void ActivateChildren()
	{
		for (int i=0; i<transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}

		activated = true;
	}

	/// <summary>
	/// Deactivates the children.
	/// </summary>
	public void DeactivateChildren()
	{
		for (int i=0; i<transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}

		activated = false;
	}
}
