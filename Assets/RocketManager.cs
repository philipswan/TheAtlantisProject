using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour {

    // Space shuttle max acceleration ~= 29m/s^2
    // Launch velocity ~= 1341.12 m/s

    private float startTime;
    private float acceleration;
    private bool launched;

    // Use this for initialization
    void Start () {
        acceleration = 29;
        launched = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space") && !launched)
        {
            startTime = Time.time;
            StartCoroutine("Launch");
            launched = true;
        }

    }

    private IEnumerator Launch()
    {
        while (true)
        {
            transform.position = transform.up * 0.5f * acceleration * Mathf.Pow((Time.unscaledTime - startTime), 2);

            yield return null;
        }
    }
}
