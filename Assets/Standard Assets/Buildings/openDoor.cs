using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour {

    public Transform player;
    public Animator myAnimator;

	// Use this for initialization
	void Start () {
        myAnimator = transform.GetComponent<Animator>();
        Debug.Log("myAnimator = " + myAnimator);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "OVRPlayerController")
        {
            Debug.Log("player hit the door");
            myAnimator.Play("Opening");
        }
    }
}
