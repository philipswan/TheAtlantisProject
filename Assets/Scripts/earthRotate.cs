using UnityEngine;
using System.Collections;

public class earthRotate : MonoBehaviour {
	
	private float oldX;

	// Use this for initialization
	void Start () 
	{
		oldX = Input.mousePosition.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (oldX != Input.mousePosition.x)
		{
			transform.Rotate (0,20*Time.deltaTime*(-Input.mousePosition.x+oldX),0);
		}
		
		oldX = Input.mousePosition.x;
	}
}
