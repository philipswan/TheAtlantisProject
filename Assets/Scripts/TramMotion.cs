using System.Collections;
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
	private enum AccelerationState {Accelerate, Cruise, Decelerate, Slowest, None}
	private AccelerationState accelerationState;
	private float accelerationStartTime;
	private bool parametersSet;
	private float blendTime;

	private AnimationClip clip;
	private Animator anim;
	private AnimationEvent evt;
	private int accelerationHash = Animator.StringToHash("Acceleration");
	private int cruiseHash = Animator.StringToHash("Cruise");
	private int decelerationHash = Animator.StringToHash("Deceleration");
	private int waitHash = Animator.StringToHash("Wait");

	void Awake()
	{
		travelTram = false;
		parametersSet = false;

//		print(accelerationHash + " " + cruiseHash + " " + decelerationHash + " " + waitHash);
		anim = GetComponent<Animator>();
//		evt = new AnimationEvent();
//
//		evt.intParameter = 12345;
//		evt.time = 2.0f;
//		evt.functionName = "PrintEvent";
//
		clip = anim.runtimeAnimatorController.animationClips[0].cu;
		//anim.speed = 1/10;

//		clip.AddEvent(evt);
	}

//	public void PrintEvent(int i)
//	{
//		print("PrintEvent: " + i + " called at: " + Time.time);
//	}

	// Use this for initialization
	void Start () {
		config = Constants.Configuration.Instance;
		highlited = false;
		accelerationInstance = 0;
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
		else
		{
			Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / travelTime);
			Blend = Mathf.Min ((Time.unscaledTime - startTime) / blendTime - Scene, 1.0f);

			if (Scene != prevScene)
			{
				prevScene = Scene;
				sceneSwitchTime = Time.unscaledTime;
			}

			if ((Scene + 1) % 5 != 0)
			{
				if ((acceleration * (Time.unscaledTime - accelerationStartTime)) < topSpeed || accelerationState == AccelerationState.Slowest || accelerationState == AccelerationState.Decelerate)
				{
					if (accelerationState != AccelerationState.Accelerate)
					{
						accelerationState = AccelerationState.Accelerate;
					}

					accelerationInstance++;
					if (name == "Bottom Right Tram 0")
					{
//						print("acceleration, Scene: " + Scene);
//						print("Current velocity: " + (acceleration * (Time.unscaledTime - accelerationStartTime)));
					}
				}
				else
				{
					if (accelerationState != AccelerationState.Cruise)
					{
						accelerationState = AccelerationState.Cruise;
					}

					if (name == "Bottom Right Tram 0")
					{
//						print("cruise, Scene: " + Scene);
					}
				}
			}
			else
			{
				if ((acceleration * (Time.unscaledTime - accelerationStartTime)) < topSpeed || accelerationState == AccelerationState.Cruise)
				{
					if (accelerationState != AccelerationState.Decelerate)
					{
						accelerationState = AccelerationState.Decelerate;
					}

					if (name == "Bottom Right Tram 0")
					{
//						print("deceleration, Scene: " + Scene);
//						print("Current velocity: " + acceleration * (Time.unscaledTime - accelerationStartTime));
					}
				}
				else
				{
					if (accelerationState != AccelerationState.Slowest)
					{
						accelerationState = AccelerationState.Slowest;
					}

					if (name == "Bottom Right Tram 0")
					{
//						print("slowest, Scene: " + Scene);
					}
				}
			}
		}
			
		if (Input.GetKeyDown(KeyCode.Space))
		{
			anim.SetTrigger("Cruise");
		}

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
		cruiseTime = 1 / _cruiseTime;
		accelerationTime = 1 / _accelerationTime;
		insertedKeys = _insertedKeys;

		travelTime = _cruiseTime;

		sceneSwitchTime = Time.unscaledTime;
		anim.speed = accelerationTime;

		parametersSet = true;
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
	private List<Keyframe[]> GetPositionKeyframes(Vector3 startPos, Vector3 endPos, AccelerationState state, int samples = 1000)
	{
		float[] accelerationFrame = new float[samples];	// Inialize array of size samples to hold all acceleration values for each keyframe
		int accelerationStopIndex = samples * (accelerationTime / cruiseTime);
		int accelerationStartIndex = samples * (1 - (accelerationTime / cruiseTime));

		// Calculate the acceleration per index
		for (int i=0; i<samples; i++)
		{
			// If we are accelerating
			if (state == AccelerationState.Accelerate)
			{
				// If we're still in the acceleration phase
				if (i <= accelerationStopIndex)
				{
					accelerationFrame[i] = (i / accelerationStopIndex) * accelerationTime * acceleration;
				}
				// If we're done accelerating
				else
				{
					accelerationFrame[i] = accelerationTime * acceleration;
				}
			}
			// If we a decelerating
			else if (state == AccelerationState.Decelerate)
			{
				// If we're decelerating
				if (i >= accelerationStartIndex)
				{
					accelerationFrame[i] = -((i - accelerationStartIndex) / ( samples - accelerationStartIndex)) * accelerationTime * acceleration;
				}
				// If we're still cruising
				else
				{
					accelerationFrame[i] = 0.0f;
				}
			}
		}

		Keyframe[] keyframesX = new Keyframe[samples];	// Initialize array to hold x coord keyframes

		for (int i=0; i<keyframesX.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesX[i] = new Keyframe((i/keyframesX.Length), Mathf.Lerp(startPos.x, endPos.x, 1/keyframesX.Length) + accelerationFrame[i]);
		}

		Keyframe[] keyframesY = new Keyframe[samples];	// Initialize array to hold y coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe((i/keyframesY.Length), Mathf.Lerp(startPos.y, endPos.y, 1/keyframesY.Length) + accelerationFrame[i]);
		}

		Keyframe[] keyframesZ = new Keyframe[samples];	// Initialize array to hold z coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe((i/keyframesZ.Length), Mathf.Lerp(startPos.z, endPos.z, 1/keyframesZ.Length) + accelerationFrame[i]);
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
	private List<Keyframe[]> GetRotationKeyframes(Quaternion startRot, Quaternion endRot, AccelerationState state, int samples = 1000)
	{
		float[] accelerationFrame = new float[samples];
		int accelerationStopIndex = samples * (accelerationTime / cruiseTime);
		int accelerationStartIndex = samples * (1 - (accelerationTime / cruiseTime));

		// Calculate the acceleration per index
		for (int i=0; i<samples; i++)
		{
			// If we are accelerating
			if (state == AccelerationState.Accelerate)
			{
				// If we're still in the acceleration phase
				if (i <= accelerationStopIndex)
				{
					accelerationFrame[i] = (i / accelerationStopIndex) * accelerationTime * acceleration;
				}
				// If we're done accelerating
				else
				{
					accelerationFrame[i] = accelerationTime * acceleration;
				}
			}
			// If we a decelerating
			else if (state == AccelerationState.Decelerate)
			{
				// If we're decelerating
				if (i >= accelerationStartIndex)
				{
					accelerationFrame[i] = -((i - accelerationStartIndex) / ( samples - accelerationStartIndex)) * accelerationTime * acceleration;
				}
				// If we're still cruising
				else
				{
					accelerationFrame[i] = 0.0f;
				}
			}
		}

		Keyframe[] keyframesX = new Keyframe[samples];	// Initialize array to hold x coord keyframes

		for (int i=0; i<keyframesX.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesX[i] = new Keyframe((i/keyframesX.Length), Mathf.Lerp(startRot.x, endRot.x, 1/keyframesX.Length) + accelerationFrame[i]);
		}

		Keyframe[] keyframesY = new Keyframe[samples];	// Initialize array to hold y coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe((i/keyframesY.Length), Mathf.Lerp(startRot.y, endRot.y, 1/keyframesY.Length) + accelerationFrame[i]);
		}

		Keyframe[] keyframesZ = new Keyframe[samples];	// Initialize array to hold z coord keyframes

		for (int i=0; i<keyframesY.Length; i++)
		{
			// Lerp through points by sample interval
			keyframesY[i] = new Keyframe((i/keyframesZ.Length), Mathf.Lerp(startRot.z, endRot.z, 1/keyframesZ.Length) + accelerationFrame[i]);
		}

		List<Keyframe[]> keyframes = new List<Keyframe[]>(){keyframesX, keyframesY, keyframesZ};
		return keyframes;
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
