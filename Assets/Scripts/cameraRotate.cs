using UnityEngine;
using System.Collections;

public class cameraRotate : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey (KeyCode.LeftArrow))
		{
			transform.Rotate (0,10*Time.deltaTime,0);
		}
		else if (Input.GetKey (KeyCode.RightArrow))
		{
			transform.Rotate (0,-10*Time.deltaTime,0);
		}
		
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Application.Quit ();
		}
	}
}
