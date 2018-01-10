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
        acceleration = 0.01f;
        launched = false;
	}
	
	// Update is called once per frame
	void Update () {
//        if (Input.GetKeyDown("space") && !launched)
//        {
//            startTime = Time.time;
//            StartCoroutine("Launch");
//            launched = true;
//        }

    }

	public void StartLaunch()
	{
		if (!launched)
		{
			startTime = Time.unscaledTime;
			StartCoroutine("Launch");
			launched = true;

			GetComponent<AudioSource>().Play();
		}
	}

    private IEnumerator Launch()
    {
        while (true)
        {
			transform.position += transform.up * 0.5f * acceleration *(Time.unscaledTime - startTime);

			if (Vector3.Distance(Camera.main.transform.position, transform.position) > 150)
			{
				gameObject.SetActive(false);
				yield break;
			}
				
            yield return null;
        }
    }
}
