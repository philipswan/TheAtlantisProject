using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TramMotion : MonoBehaviour {

    #region Public Properites
    public List<Material> HighlightMaterials = new List<Material>();             // Regular materials + highlight material
    #endregion

    #region Private Properties
	private bool travelTram;                                                     // Set true if the tram does not stop
    private bool highlited;												         // Current materials used
    private bool finishedKeyPositions;
    private bool finishedKeyRotations;

    private int currentClip;                                                     // Index of current clip being played
    private int waitOffset;                                                      // Offset index to ensure no positions are skipped when a wait animation is created
    private float accelerationTime;                                              // Time it takes to accelerate to top speed
    private float cruiseTime;                                                    // Time it takes to travel between two keys at top speed
    private float speed;                                                         // Speed of the animation clip
    private float clipSwitchTime;                                                // Used to decelerate the tram so it stops at habitat, not before it
    private float waitTime;                                                      // How long the tram waits at the habitat
	private float startTime;
	private Vector3 velocity;													 // Speed cap for smoothdamp
	private string clipName;
	private int clipInCycle;
	private int clipCount;

    private List<Vector3> positions = new List<Vector3>();                       // All destinatins for the tram
    private List<Quaternion> rotations = new List<Quaternion>();                 // Rotation of the tram at its destination
    private List<string> clipNames = new List<string>();                         // Names assigned to clips to reference them
    private List<AnimationState> states = new List<AnimationState>();
    private List<Keyframe[]> keyframes = new List<Keyframe[]>();
    private List<AnimationCurve> curves = new List<AnimationCurve>();            // List of animation states to adjust their speeds (speed = animation clip speed / desired speed)
	private List<AnimationClip> clips = new List<AnimationClip>();
	private Material[] DefaultMaterials;                                         // Regular materials

    private List<Keyframe[]> keyPositions = new List<Keyframe[]>();              // List of keyframes for the x, y, and z positions for the current clip
    private List<Keyframe[]> keyRotations = new List<Keyframe[]>();              // List of keyframes for the w, x, y, and z rotations for the current clip

    private enum AccelerationState { Accelerate, Cruise, Decelerate, Wait, None }  // Enumeration of possible acceleration states
    private AccelerationState accelerationState;                                   // Enumaration accessor
    private Animation anim;                                                        // Reference to animation component

    private Constants.Configuration config;										 // Holds reference to config file
    #endregion

    #region Mono Methods
    void Awake()
    {
        travelTram = false;
        anim = GetComponent<Animation>();
    }

    // Use this for initialization
    void Start() {
        config = Constants.Configuration.Instance;
		tag = "Tram";

        highlited = false;
        currentClip = 0;
        waitOffset = 0;
		clipInCycle = 0;
		clipCount = 0;
        accelerationState = AccelerationState.None;

        // Set the highlighted materials
        DefaultMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
        for (int i = HighlightMaterials.Count - 1; i < DefaultMaterials.Length; i++)
        {
            HighlightMaterials.Add(DefaultMaterials[i]);
        }

        // Now that our arrays have been initialized, we can create our first two clips
		if (!travelTram)
		{
			StartCoroutine("CreateClips", 2);
		}
		else
		{
			startTime = Time.unscaledTime;
		}
    }

    // Update is called once per frame
    void Update() {
		if (travelTram)
		{
			int Scene = 0;
			float Blend = 0.0f;

			if (travelTram)
			{
				Scene = (int)Mathf.Floor((Time.unscaledTime - startTime) / config.TramTravelTime);
				Blend = Mathf.Min ((Time.unscaledTime - startTime) / config.TramTravelTime - Scene, 1.0f);
			}

			if (Scene < positions.Count - 1 && !travelTram)
			{
				UpdateSystem(Scene, Blend);
			}
			else if (Scene < positions.Count && travelTram)
			{
				UpdateSystem(Scene, Blend);
			}
			else
			{
				startTime = Time.unscaledTime;
			}
		}
    }

    // Used to update animation state
    void LateUpdate()
    {
        // Only proceed if we have animations to play
		if (!travelTram && anim.GetClipCount() > 0)
        {
            // If the clip is no longer playing, create a new one and increase the clip index by 1
            if (!anim.IsPlaying(clipNames[currentClip]))
            {
                if (currentClip < states.Count - 1)
                {
					currentClip++;
					clipSwitchTime = Time.unscaledTime;

                    StartCoroutine("CreateClips", 1);

					// Get the name of the clip without the number at the end
					clipName = clipNames[currentClip].Substring(0, clipNames[currentClip].Length - (2 + (int)Mathf.Floor(currentClip / 10)));
				}
            }
			else if (!anim.isPlaying)
			{
				DestroyClips();
			}
				
            MoveTramWithStops(clipName);

            // Set the clips speed
            states[currentClip].speed = 1 / speed;
        }
    }
    #endregion

    #region Public Methods
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
            // Set materials to highlighted ones and adjust shader values
            transform.GetChild(0).GetComponent<MeshRenderer>().materials = HighlightMaterials.ToArray();
            transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetFloat("_Outline", 0.001f);
            transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(0.9568f, 0.2627f, 0.2118f, 1));
            transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.001f);
            transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetColor("_Color", new Color(1, 1, 1, 1));
        }

        highlited = !highlited;
    }

    /// <summary>
    /// Set the movement parameters for the tram
    /// </summary>
    /// <param name="_acceleration"></param>
    /// <param name="_topSpeed"></param>
    /// <param name="_cruiseTime"></param>
    /// <param name="_accelerationTime"></param>
	public void SetSpeeds(float _cruiseTime, float _accelerationTime)
    {
        cruiseTime = _cruiseTime;
        accelerationTime = _accelerationTime;
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
    /// Sets the positions to the list
    /// </summary>
    /// <param name="pos">Position.</param>
    public void AddPosition(List<Vector3> pos)
    {
        positions = new List<Vector3>(pos);
    }

    /// <summary>
    /// Sets the name of animations
    /// </summary>
    /// <param name="names"></param>
    public void AddClipNames(List<string> names)
    {
        clipNames = new List<string>(names);
    }

    /// <summary>
    /// Sets the travel tram (tram does not make stops)
    /// </summary>
    public void SetTravelTram()
    {
        travelTram = true;
    }

	public void SetWaitTime(int _waitTime)
	{
		waitTime = waitTime;
		waitTime /= 100;
	}
    #endregion

    #region Private Methods
    /// <summary>
    /// Controls the movement of the tram if it makes stops
    /// Has an acceleration, deceleration, cruise, and wait state
    /// </summary>
    /// <param name="clipName"></param>
    private void MoveTramWithStops(string clipName)
    {
        if (clipName == "Accelerate")
        {
            // If we just switched to accelerating, set the state and the speed
            if (accelerationState != AccelerationState.Accelerate)
            {
                speed = accelerationTime + cruiseTime;
                accelerationState = AccelerationState.Accelerate;
            }
            // accelerate every frame until we're at full speed
            if (speed > cruiseTime)
            {
                speed -= Time.deltaTime;
            }
            else
            {
                speed = cruiseTime;
            }
        }
        else if (clipName == "Decelerate")
        {
            // If we just switched to decelerating, set the state and speed
            if (accelerationState != AccelerationState.Decelerate)
            {
                speed = cruiseTime;
                accelerationState = AccelerationState.Decelerate;
            }
            // Only start decelerating when we have accelerationTime seconds left until we reach the next key
            if ((Time.unscaledTime - clipSwitchTime) >= (cruiseTime - accelerationTime))
            {
                speed += Time.deltaTime;
            }
            else
            {
                speed = cruiseTime;
            }

        }
        else if (clipName == "Wait")
        {
            // If we just switched to waiting, set the state and the speed
            // This will make the tram wait at a key
            if (accelerationState != AccelerationState.Wait)
            {
                accelerationState = AccelerationState.Wait;
                speed = config.TramWaitTime;
            }
        }
        else
        {
            // If we just swithced to cruising, set the state and speed
            // This will set the tram to move at its top speed
            if (accelerationState != AccelerationState.Cruise)
            {
                accelerationState = AccelerationState.Cruise;
                speed = cruiseTime;
            }
        }
    }

    /// <summary>
    /// Creates the curves for the x, y, and z (and w for rot) positions and rotations
    /// </summary>
    /// <returns>The curve.</returns>
    /// <param name="keyframes">Keyframes.</param>
    private List<AnimationCurve> CreateCurve(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        AnimationCurve localxPos = AnimationCurve.Linear(0, startPos.x, 1, endPos.x);
        AnimationCurve localyPos = AnimationCurve.Linear(0, startPos.y, 1, endPos.y);
        AnimationCurve localzPos = AnimationCurve.Linear(0, startPos.z, 1, endPos.z);
        AnimationCurve localxRot = AnimationCurve.Linear(0, startRot.x, 1, endRot.x);
        AnimationCurve localyRot = AnimationCurve.Linear(0, startRot.y, 1, endRot.y);
        AnimationCurve localzRot = AnimationCurve.Linear(0, startRot.z, 1, endRot.z);
        AnimationCurve localwRot = AnimationCurve.Linear(0, startRot.w, 1, endRot.w);

        List<AnimationCurve> curves = new List<AnimationCurve>() { localxPos, localyPos, localzPos, localxRot, localyRot, localzRot, localwRot };
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
    /// Adds a new animation clip to the queue
    /// If this is the first clip, play it immediately
    /// </summary>
    /// <param name="clipsToCreate"></param>
    /// <returns></returns>
	private IEnumerator CreateClips(int clipsToCreate)
    {
        for (int i = 0; i < clipsToCreate; i++)
        {
            // Clear lists from previous clip
            curves.Clear();

            // Set up our variables
            AnimationClip clip;
            string newClipName;
            Vector3 startPos, endPos;
            Quaternion startRot, endRot;
            int index = anim.GetClipCount();

//			if (name == "0")
//			{
//				print("index: " + index + " offset: " + waitOffset + " cycle: " + clipInCycle);
//			}
            // If we're at a wait anim, set the start and end values to eachother
			if (clipInCycle % 5 == 0 && clipInCycle != 0)
            {
				startPos = positions[index - waitOffset];
                endPos = startPos;

				startRot = rotations[index - waitOffset];
                endRot = startRot;

                waitOffset++;
				clipInCycle = 0;
            }
            // Create an animation that moves to the next position and rotation
            else
            {
                startPos = positions[index - waitOffset];
                endPos = positions[index + 1 - waitOffset];

                startRot = rotations[index - waitOffset];
                endRot = rotations[index + 1 - waitOffset];

				clipInCycle++;

            }
				
            // Create animation curves from the keyframes
            curves = CreateCurve(startPos, endPos, startRot, endRot);

            // Create the animation clip
            clip = CreateClip(curves);

			yield return new WaitForSeconds(waitTime);

			// Set the reference name of the clip
			newClipName = clipNames[index];
            // Add clip to animation
			anim.AddClip(clip, newClipName);
			// Add clip to list to reference it later
			clips.Add(clip);

            // If this is the first clip, play it immediately
            if (index == 0)
            {
				// Get the name of the clip without the number at the end
				clipName = clipNames[currentClip].Substring(0, clipNames[currentClip].Length - 2);

				anim.Play(newClipName);
				states.Add(anim[newClipName]); // Add animation state reference to list to allow us to change its speed in LateUpdate
            }
            // If it isn't, add it to the queue
            else
            {
				states.Add(anim.PlayQueued(newClipName));  // Add animation state reference to list to allow us to change its speed in LateUpdate
            }

			clipCount++;

//			if (name == "0")
//			{
//				print("Capacity: " + states.Capacity + " Count: " + states.Count);
//			}
        }
    }

	/// <summary>
	/// Destroy clip it index in list
	/// </summary>
	/// <param name="index">Index.</param>
	private void DestroyClips()
	{
//		print("index: " + index + " name: " + clipNames[index] + " name: " + name);
		for (int i=0; i<clipNames.Count; i++)
		{
			anim.RemoveClip(clipNames[i]);
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
	private void UpdateSystem(int scene, float blend)
	{
		int index0 = (scene > 0) ? (scene - 1) : 0;
		int index1 = index0 + 1;

		if (scene == 0)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, positions[scene], ref velocity, config.TramTravelTime);
		}
		else
		{
			transform.localPosition = Vector3.Lerp(positions[index0], positions[index1], Mathf.Pow(blend, 1.05f));
		}
		transform.localRotation = Quaternion.Lerp(rotations[index0], rotations[scene], Mathf.Pow(blend, 1.05f));

	}
    #endregion
}
