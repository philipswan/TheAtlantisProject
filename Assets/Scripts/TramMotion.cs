﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramMotion : MonoBehaviour {

	public List<Quaternion> rotations = new List<Quaternion>();			// Rotation of the tram at its destination
	public List<Vector3> positions = new List<Vector3>();				// All destinatins for the tram
	public List<Material> HighlightMaterials = new List<Material>();	// Regarul materials + highlight material

	private Constants.Configuration config;								// Holds reference to config script
	private int Scene;
	private int prevScene;
	private float sceneSwitchTime;
	private int insertedKeys;
	private int accelerationInstance;
	private float startTime;											// Starting time of the movement. Reset when a cycle is completed
	public float accelerationTime;
	public float travelTime;
	public float topSpeed;
	public float acceleration;
	public float cruiseTime;
	private bool travelTram;											// Set true if the tram does not stop
	private Material[] DefaultMaterials;								// Regular materials
	private bool highlited;												// Current materials used
	private enum AccelerationState {Accelerate, Cruise, Decelerate, Slowest, Wait, None}
	private AccelerationState accelerationState;
	private float accelerationStartTime;
	private bool parametersSet;
	private float blendTime;

	private AnimationClip clip;
	private Animation anim;
	private AnimationEvent evt;
	private int currentClip;
	private int waitOffset;
	private List<string> clips = new List<string>();
	private List<AnimationState> states = new List<AnimationState>();
	private float speed;

	void Awake()
	{
		travelTram = false;
		parametersSet = false;

		anim = GetComponent<Animation>();
	}

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
		highlited = false;
		accelerationInstance = 0;
		currentClip = 0;
		waitOffset = 0;
		accelerationState = AccelerationState.None;
		prevScene = -999;
	
		DefaultMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
		for (int i=HighlightMaterials.Count-1; i<DefaultMaterials.Length; i++)
		{
			HighlightMaterials.Add(DefaultMaterials[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!parametersSet)
		{ return; }

		float Blend = 0.0f;

		if (travelTram)
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / travelTime);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / travelTime - Scene, 1.0f);

			if ((acceleration * (Time.unscaledTime - startTime)) < topSpeed)
			{
				accelerationInstance++;
				Blend += acceleration * (Time.unscaledTime - startTime) - topSpeed;
			}
			else
			{
				if (name == "Bottom Right Tram 0")
				{
				}
			}

		}
			
//		if (Input.GetKeyDown(KeyCode.Space))
//		{
//			anim.SetTrigger("Cruise");
//		}

//		if (Scene < positions.Count - 1 && !travelTram)
//		{
//			UpdateSystem(Scene, Blend, travelTram);
//		}
//		else if (Scene < positions.Count && travelTram)
//		{
//			UpdateSystem(Scene, Blend, travelTram);
//		}
//		else
//		{
//			startTime = Time.unscaledTime;
//		}
	}

	void LateUpdate()
	{
		if (anim.isPlaying)
		{
//			print("name: " + clips[currentClip] + " index: " + currentClip + " wait offset: " + waitOffset);
			if (!anim.IsPlaying(clips[currentClip]))
			{
				if (currentClip < clips.Count -1 )
				{
					currentClip++;
					states[currentClip].speed = 1 / speed;
				}
				if (currentClip % 5 == 0 && currentClip != 0)
				{
					waitOffset++;
				}
			}
			if (currentClip % (5 + waitOffset) == 0 || currentClip == 0)
			{
				if (accelerationState != AccelerationState.Accelerate)
				{
					speed = accelerationTime + cruiseTime;
					accelerationState = AccelerationState.Accelerate;
				}
				if (speed > cruiseTime)
				{
					speed -= Time.deltaTime;
				}
				else
				{
					speed = cruiseTime;
				}
//				print("accelerating, speed: " + states[currentClip].speed);

			}
			else if (currentClip % (3 + waitOffset) == 0)
			{
				if (accelerationState != AccelerationState.Decelerate)
				{
					speed = cruiseTime;
					accelerationState = AccelerationState.Decelerate;
				}
				if (speed < cruiseTime + accelerationTime)
				{
					speed += Time.deltaTime;
				}
				else
				{
					speed = cruiseTime = accelerationTime;
				}
//				print("decelerating, speed: " + states[currentClip].speed);

			}
			else if (currentClip % (4 + waitOffset) == 0)
			{
				if (accelerationState != AccelerationState.Wait)
				{
					accelerationState = AccelerationState.Wait;
					speed = cruiseTime;
				}
//				print("waiting, speed: " + states[currentClip].speed);

			}
			else
			{
				if (accelerationState != AccelerationState.Cruise)
				{
					accelerationState = AccelerationState.Cruise;
					speed = cruiseTime;
				}
//				print("cruising, speed: " + states[currentClip].speed);

			}
			states[currentClip].speed = 1 / speed;
		}
	}

	void OnEnable()
	{
		startTime = Time.unscaledTime;
		accelerationTime = Time.unscaledTime;
	}

	/// <summary>
	/// Toggle highlight material when selected on controller menu
	/// </summary>
	public void SetMaterials()
	{
		if (highlited)
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = DefaultMaterials;
		}
		else
		{
			transform.GetChild(0).GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetFloat("_Outline", 0.001f);
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(0.9568f,0.2627f,0.2118f, 1));
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.001f);
			transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetColor("_Color", new Color(1,1,1,1));
		}

		highlited = ! highlited;
	}

	/// <summary>
	/// Sets the speeds.
	/// </summary>
	/// <param name="_acceleration">Acceleration.</param>
	/// <param name="_topSpeed">Top speed.</param>
	/// <param name="_travelTime">Travel time.</param>
	public void SetSpeeds(float _acceleration, float _topSpeed, float _cruiseTime, float _accelerationTime, int _insertedKeys = 0)
	{
		acceleration = _acceleration;
		topSpeed = _topSpeed;
		cruiseTime = _cruiseTime;
		accelerationTime = _accelerationTime;
		insertedKeys = _insertedKeys;

		travelTime = _cruiseTime;

		sceneSwitchTime = Time.unscaledTime;
//		anim.speed = accelerationTime;

		if (name == "Bottom Right Tram 0")
			StartCoroutine("CreateClips");
	}

	/// <summary>
	/// Adds the rotation to the list
	/// </summary>
	/// <param name="rot">Rot.</param>
	public void AddRotation(Quaternion rot)
	{
		rotations.Add(rot);
	}

	/// <summary>
	/// Sets the rotations to the list
	/// </summary>
	/// <param name="rot">Rot.</param>
	public void AddRotation(List<Quaternion> rot)
	{
		rotations = new List<Quaternion>(rot);
	}

	/// <summary>
	/// Adds the position to the list.
	/// </summary>
	/// <param name="pos">Position.</param>
	public void AddPosition(Vector3 pos)
	{
		positions.Add(pos);
	}

	/// <summary>
	/// Sets the positions to the list
	/// </summary>
	/// <param name="pos">Position.</param>
	public void AddPosition(List<Vector3> pos)
	{
		positions = new List<Vector3>(pos);
	}

	/// <summary>
	/// Sets the travel tram (tram does not make stops)
	/// </summary>
	public void SetTravelTram()
	{
		travelTram = true;
	}

	/// <summary>
	/// Gets the position keyframes.
	/// </summary>
	/// <returns>The position keyframes.</returns>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	/// <param name="state">State.</param>
	/// <param name="samples">Samples.</param>
	private List<Keyframe[]> GetPositionKeyframes(Vector3 startPos, Vector3 endPos, int samples = 1000)
	{
//		float[] accelerationFrame = new float[samples];	// Inialize array of size samples to hold all acceleration values for each keyframe
//		int accelerationStopIndex = samples * (accelerationTime / cruiseTime);
//		int accelerationStartIndex = samples * (1 - (accelerationTime / cruiseTime));
//
//		// Calculate the acceleration per index
//		for (int i=0; i<samples; i++)
//		{
//			// If we are accelerating
//			if (state == AccelerationState.Accelerate)
//			{
//				// If we're still in the acceleration phase
//				if (i <= accelerationStopIndex)
//				{
//					accelerationFrame[i] = (i / accelerationStopIndex) * accelerationTime * acceleration;
//				}
//				// If we're done accelerating
//				else
//				{
//					accelerationFrame[i] = accelerationTime * acceleration;
//				}
//			}
//			// If we a decelerating
//			else if (state == AccelerationState.Decelerate)
//			{
//				// If we're decelerating
//				if (i >= accelerationStartIndex)
//				{
//					accelerationFrame[i] = -((i - accelerationStartIndex) / ( samples - accelerationStartIndex)) * accelerationTime * acceleration;
//				}
//				// If we're still cruising
//				else
//				{
//					accelerationFrame[i] = 0.0f;
//				}
//			}
//		}

		Keyframe[] keyframesX = new Keyframe[samples];	// Initialize array to hold x coord keyframes

		for (int i=0; i<keyframesX.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesX[i] = new Keyframe(((float)i/keyframesX.Length), Mathf.Lerp(startPos.x, endPos.x, (float)i/keyframesX.Length));
		}

		Keyframe[] keyframesY = new Keyframe[samples];	// Initialize array to hold y coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe(((float)i/keyframesY.Length), Mathf.Lerp(startPos.y, endPos.y, (float)i/keyframesY.Length));
		}

		Keyframe[] keyframesZ = new Keyframe[samples];	// Initialize array to hold z coord keyframes

		for (int i=0; i<keyframesZ.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesZ[i] = new Keyframe(((float)i/keyframesZ.Length), Mathf.Lerp(startPos.z, endPos.z, (float)i/keyframesZ.Length));
		}
			
		List<Keyframe[]> keyframes = new List<Keyframe[]>(){keyframesX, keyframesY, keyframesZ};
		return keyframes;
	}

	/// <summary>
	/// Gets the rotation keyframes.
	/// </summary>
	/// <returns>The rotation keyframes.</returns>
	/// <param name="startRot">Start rot.</param>
	/// <param name="endRot">End rot.</param>
	/// <param name="state">State.</param>
	/// <param name="samples">Samples.</param>
	private List<Keyframe[]> GetRotationKeyframes(Quaternion startRot, Quaternion endRot, int samples)
	{
//		float[] accelerationFrame = new float[samples];
//		int accelerationStopIndex = samples * (accelerationTime / cruiseTime);
//		int accelerationStartIndex = samples * (1 - (accelerationTime / cruiseTime));
//
//		// Calculate the acceleration per index
//		for (int i=0; i<samples; i++)
//		{
//			// If we are accelerating
//			if (state == AccelerationState.Accelerate)
//			{
//				// If we're still in the acceleration phase
//				if (i <= accelerationStopIndex)
//				{
//					accelerationFrame[i] = (i / accelerationStopIndex) * accelerationTime * acceleration;
//				}
//				// If we're done accelerating
//				else
//				{
//					accelerationFrame[i] = accelerationTime * acceleration;
//				}
//			}
//			// If we a decelerating
//			else if (state == AccelerationState.Decelerate)
//			{
//				// If we're decelerating
//				if (i >= accelerationStartIndex)
//				{
//					accelerationFrame[i] = -((i - accelerationStartIndex) / ( samples - accelerationStartIndex)) * accelerationTime * acceleration;
//				}
//				// If we're still cruising
//				else
//				{
//					accelerationFrame[i] = 0.0f;
//				}
//			}
//		}

		Keyframe[] keyframesX = new Keyframe[samples];	// Initialize array to hold x coord keyframes

		for (int i=0; i<keyframesX.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesX[i] = new Keyframe(((float)i/keyframesX.Length), Mathf.Lerp(startRot.x, endRot.x, (float)i/keyframesX.Length));
		}

		Keyframe[] keyframesY = new Keyframe[samples];	// Initialize array to hold y coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe(((float)i/keyframesY.Length), Mathf.Lerp(startRot.y, endRot.y, (float)i/keyframesY.Length));
		}

		Keyframe[] keyframesZ = new Keyframe[samples];	// Initialize array to hold z coord keyframes

		for (int i=0; i<keyframesZ.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesZ[i] = new Keyframe(((float)i/keyframesZ.Length), Mathf.Lerp(startRot.z, endRot.z, (float)i/keyframesZ.Length));
		}

		Keyframe[] keyframesW = new Keyframe[samples];	// Initialize array to hold w coord keyframes

		for (int i=0; i<keyframesW.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesW[i] = new Keyframe(((float)i/keyframesW.Length), Mathf.Lerp(startRot.w, endRot.w, (float)i/keyframesW.Length));
		}

		List<Keyframe[]> keyframes = new List<Keyframe[]>(){keyframesX, keyframesY, keyframesZ, keyframesW};
		return keyframes;
	}

	/// <summary>
	/// Creates the curves for the x, y, and z positions or rotations
	/// </summary>
	/// <returns>The curve.</returns>
	/// <param name="keyframes">Keyframes.</param>
	private List<AnimationCurve> CreateCurve(List<Keyframe[]> keyframes)
	{
		AnimationCurve localxPos = new AnimationCurve(keyframes[0]);
		AnimationCurve localyPos = new AnimationCurve(keyframes[1]);
		AnimationCurve localzPos = new AnimationCurve(keyframes[2]);
		AnimationCurve localxRot = new AnimationCurve(keyframes[3]);
		AnimationCurve localyRot = new AnimationCurve(keyframes[4]);
		AnimationCurve localzRot = new AnimationCurve(keyframes[5]);
		AnimationCurve localwRot = new AnimationCurve(keyframes[6]);

		List<AnimationCurve> curves = new List<AnimationCurve>(){localxPos, localyPos, localzPos, localxRot, localyRot, localzRot, localwRot};
		return curves;
	}

	/// <summary>
	/// Creates an animation clip and adds it to the list
	/// </summary>
	/// <returns>The clip.</returns>
	/// <param name="curves">Curves.</param>
	private AnimationClip CreateClip(List<AnimationCurve> curves)
	{
		AnimationClip clip = new AnimationClip();
		clip.legacy = true;

		clip.SetCurve("", typeof(Transform), "localPosition.x", curves[0]);
		clip.SetCurve("", typeof(Transform), "localPosition.y", curves[1]);
		clip.SetCurve("", typeof(Transform), "localPosition.z", curves[2]);
		clip.SetCurve("", typeof(Transform), "localRotation.x", curves[3]);
		clip.SetCurve("", typeof(Transform), "localRotation.y", curves[4]);
		clip.SetCurve("", typeof(Transform), "localRotation.z", curves[5]);
		clip.SetCurve("", typeof(Transform), "localRotation.w", curves[6]);

		return clip;
	}

	/// <summary>
	/// Create all animation clips
	/// </summary>
	/// <returns>The clips.</returns>
	private IEnumerator CreateClips()
	{
		for (int i=0; i<positions.Count-1; i++)
		{
			List<Keyframe[]> keyframes = new List<Keyframe[]>();
			List<Keyframe[]> keyPositions = new List<Keyframe[]>();
			List<Keyframe[]> keyRotations = new List<Keyframe[]>();
			List<AnimationCurve> curves = new List<AnimationCurve>();
			AnimationClip clip;

			// If we're at a habitat, create a clip with the same start/end position/rotation to stop the tram
			if (i % 5 == 0 && i != 0)
			{
				// Get the keyframes between the two positions
				keyPositions = GetPositionKeyframes(positions[i], positions[i], 1000);
				keyRotations = GetRotationKeyframes(rotations[i], rotations[i], 1000);

				// Combine the x, y, and z coordinates for position and rotation into one list
				for (int j=0; j<keyPositions.Count; j++)
				{
					keyframes.Add(keyPositions[j]);
				}
				for (int j=0; j<keyRotations.Count; j++)
				{
					keyframes.Add(keyRotations[j]);
				}

				// Create animation curves from the keyframes
				curves = CreateCurve(keyframes);

				clip = CreateClip(curves);

				keyframes.Clear();
				keyPositions.Clear();
				keyRotations.Clear();
				curves.Clear();

				clips.Add("clip " + i);

				anim.AddClip(clip, "clip " + i);
				anim.PlayQueued("clip " + i);
			}

			// Get the keyframes between the two positions
			keyPositions = GetPositionKeyframes(positions[i], positions[i+1], 1000);
			keyRotations = GetRotationKeyframes(rotations[i], rotations[i+1], 1000);

			// Combine the x, y, and z coordinates for position and rotation into one list
			for (int j=0; j<keyPositions.Count; j++)
			{
				keyframes.Add(keyPositions[j]);
			}
			for (int j=0; j<keyRotations.Count; j++)
			{
				keyframes.Add(keyRotations[j]);
			}
				
			// Create animation curves from the keyframes
			curves = CreateCurve(keyframes);

			clip = CreateClip(curves);
				
			clips.Add("clip " + i);
			anim.AddClip(clip, "clip " + i);
			if (i == 0)
			{
				anim.Play("clip " + i);
				states.Add(anim["clip " + i]);
			}
			else if (i > 1)
			{
				states.Add(anim.PlayQueued("clip " + i));
			}
				
			parametersSet = true;

			yield return null;
		}
	}

	/// <summary>
	/// Max the specified a and b.
	/// </summary>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	private int max(int a, int b)
	{
		return (a > b) ? a : b;
	}

	/// <summary>
	/// Minimum the specified a and b.
	/// </summary>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	private int min(int a, int b)
	{
		return (a < b) ? a : b;
	}

	/// <summary>
	/// Updates the tram transform.
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="blend">Blend.</param>
	private void UpdateSystem(int scene, float blend, bool _travelTram)
	{
		int index1Position = (scene < positions.Count - 1) ? (scene + 1) : positions.Count - 1;
		int index1Rotatoin = (scene < rotations.Count - 1) ? (scene + 1) : rotations.Count - 1;

		if (_travelTram)
		{
			transform.localPosition = Vector3.Lerp(positions[scene], positions[index1Position], blend);
			transform.localRotation = Quaternion.Lerp(rotations[scene], rotations[index1Rotatoin], blend);
		}
		else
		{
			transform.localPosition = Vector3.Lerp(positions[scene], positions[index1Position], blend);
			transform.localRotation = Quaternion.Lerp(rotations[scene], rotations[index1Rotatoin], blend);
		}
	}
}
