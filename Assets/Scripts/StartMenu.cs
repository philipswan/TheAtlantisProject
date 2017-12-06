using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour {

	private float fadeTime;							// Time for camera fade
	private bool starting;							// Flag to check if coroutine is running
	private Constants.Configuration config;			// Holds reference to config file

	// Use this for initialization
	void Start () {
		starting = false;

		config = Constants.Configuration.Instance;
		fadeTime = config.FadeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(OVRInput.Get(OVRInput.Button.Any) && !starting)
		{
			StartCoroutine("StartScene");
		}
	}

	private IEnumerator StartScene()
	{
		starting = true;

		GetComponent<AudioSource>().Play();

		// Fade camera out
		float startTime = Time.time;
		Camera.main.SendMessage("FadeCamera", false);

		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}

		//begin transition scripts and disable this script
		Transition1b.Instance.enabled = true;
		TramCars.Instance.ActivateTrams();
		transform.GetChild(0).gameObject.SetActive(false);

		// Find the background music, stop it and store its reference to play it later
		AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach( AudioSource audioS in sources) {
			if (audioS.gameObject.name == "BackgroundMusic")
			{
				audioS.Play();
				break;
			}
		}

		// Fade camera in
		startTime = Time.time;
		Camera.main.SendMessage("FadeCamera", true);

		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}
	}
}
