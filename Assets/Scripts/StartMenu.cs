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
		if(OVRInput.Get(OVRInput.Button.Any) || Input.GetKeyDown(KeyCode.Space) && !starting)
		{
			StartCoroutine("StartScene");
		}
	}

	/// <summary>
	/// Fade the camera and enable components
	/// </summary>
	/// <returns>The scene.</returns>
	private IEnumerator StartScene()
	{
		starting = true;

		// Fade camera out
		float startTime = Time.time;
		Camera.main.SendMessage("FadeCamera", false);

		// Wait while the camera fades
		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}
			
		SunTransition.Instance.enabled = true;	// Enable the sun transition script
		Transition1b.Instance.enabled = true;	// Enable transition script
		TramCars.Instance.ActivateTrams();	// Activate trams and their movement
		AtmosphereManager.Instance.SetMaterial();	// Activate the atmospheric scattering

		yield return null; 

		OctahedronSphereTester.Instance.SetMaterials();	// Set earth mats
		transform.GetChild(0).gameObject.SetActive(false);	// Disable text

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

		// Wait while the camera fades
		while (Time.time - startTime < fadeTime)
		{
			yield return null;
		}

		// Deactive since we have started
		gameObject.SetActive(false);
	}
}
