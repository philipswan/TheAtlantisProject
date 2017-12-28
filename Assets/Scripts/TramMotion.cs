using System.Collections;
using System.Collections.Generic;
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

    private List<Vector3> positions = new List<Vector3>();                       // All destinatins for the tram
    private List<Quaternion> rotations = new List<Quaternion>();                 // Rotation of the tram at its destination
    private List<string> clipNames = new List<string>();                         // Names assigned to clips to reference them
    private List<AnimationState> states = new List<AnimationState>();
    private List<Keyframe[]> keyframes = new List<Keyframe[]>();
    private List<AnimationCurve> curves = new List<AnimationCurve>();            // List of animation states to adjust their speeds (speed = animation clip speed / desired speed)
    private Material[] DefaultMaterials;                                         // Regular materials

    private List<Keyframe[]> keyPositions = new List<Keyframe[]>();              // List of keyframes for the x, y, and z positions for the current clip
    private List<Keyframe[]> keyRotations = new List<Keyframe[]>();              // List of keyframes for the w, x, y, and z rotations for the current clip
    private Keyframe[] rotationKeyframesX;                                       // Initialize array to hold x rotation coord keyframes
    private Keyframe[] rotationKeyframesY;                                       // Initialize array to hold y rotation coord keyframes
    private Keyframe[] rotationKeyframesZ;                                       // Initialize array to hold z rotation coord keyframes
    private Keyframe[] rotationKeyframesW;                                       // Initialize array to hold w rotation coord keyframes
    private Keyframe[] positionKeyframesX;                                       // Initialize array to hold x position coord keyframes
    private Keyframe[] positionKeyframesY;                                       // Initialize array to hold y position coord keyframes
    private Keyframe[] positionKeyframesZ;                                       // Initialize array to hold z position coord keyframes

    private enum AccelerationState { Accelerate, Cruise, Decelerate, Wait, None }  // Enumeration of possible acceleration states
    private AccelerationState accelerationState;                                   // Enumaration accessor
    private Animation anim;                                                        // Reference to animation component

    private Constants.Configuration config;										// Holds reference to config file
    #endregion

    #region Mono Methods
    void Awake()
    {
        travelTram = false;
        anim = GetComponent<Animation>();
    }

    // Use this for initialization
    void Start() {
        highlited = false;
        currentClip = 0;
        waitOffset = 0;
        accelerationState = AccelerationState.None;
        config = Constants.Configuration.Instance;

        // Set the highlighted materials
        DefaultMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
        for (int i = HighlightMaterials.Count - 1; i < DefaultMaterials.Length; i++)
        {
            HighlightMaterials.Add(DefaultMaterials[i]);
        }

        int samples = config.Samples;

        rotationKeyframesX = new Keyframe[samples];
        rotationKeyframesY = new Keyframe[samples];
        rotationKeyframesZ = new Keyframe[samples];
        rotationKeyframesW = new Keyframe[samples];
        positionKeyframesX = new Keyframe[samples];
        positionKeyframesY = new Keyframe[samples];
        positionKeyframesZ = new Keyframe[samples];

        // Now that our arrays have been initialized, we can create our first two clips
        StartCoroutine("CreateClips", 2);
    }

    // Update is called once per frame
    void Update() {
    }

    // Used to update animation state
    void LateUpdate()
    {
        // Only proceed if we have animations to play
        if (anim.GetClipCount() > 0)
        {
            // Get the name of the clip without the number at the end
            string clipName = clipNames[currentClip].Substring(0, clipNames[currentClip].Length - 2); ;

            // Debug info
            //if (name == "Bottom Right Tram 0")
            //{
            //    print("name: " + name + " index: " + currentClip + " wait offset: " + waitOffset + " acceleration state: " + accelerationState.ToString());
            //}

            // If the clip is no longer playing, create a new one and increase the clip index by 1
            if (!anim.IsPlaying(clipNames[currentClip]))
            {
                if (currentClip < clipNames.Count - 1)
                {
                    currentClip++;
                    clipSwitchTime = Time.unscaledTime;

                    StartCoroutine("CreateClips", 1);
                }
            }

            // If the tram makes stops at the habitats
            if (!travelTram)
            {
                MoveTramWithStops(clipName);
            }
            // If the tram does not make stops
            else
            {
                MoveTramWithoutStops();
            }

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
                speed = cruiseTime;
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
    /// Controls movement of a tram if it does not makes stops
    /// Only accelerates and then maintains top speed
    /// </summary>
    private void MoveTramWithoutStops()
    {
        if (currentClip == 0)
        {
            // Since we start at rest, accelerate to top speed
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
        }
        else
        {
            // Once we hit top speed, maintain it
            if (accelerationState != AccelerationState.Cruise)
            {
                accelerationState = AccelerationState.Cruise;
                speed = cruiseTime;
            }
        }
    }

    /// <summary>
    /// Gets the position keyframes.
    /// </summary>
    /// <returns>The position keyframes.</returns>
    /// <param name="startPos">Start position.</param>
    /// <param name="endPos">End position.</param>
    /// <param name="state">State.</param>
    /// <param name="samples">Samples.</param>
    private IEnumerator GetPositionKeyframes(object[] parameters)
    {
        Vector3 startPos = (Vector3)parameters[0];
        Vector3 endPos = (Vector3)parameters[1];

        for (int i = 0; i < positionKeyframesX.Length; i++)
        {
            // Lerp through points by sample interval
            positionKeyframesX[i] = new Keyframe(((float)i / positionKeyframesX.Length), Mathf.Lerp(startPos.x, endPos.x, (float)i / positionKeyframesX.Length));
        }

        yield return null;

        for (int i = 0; i < positionKeyframesY.Length; i++)
        {
            // Lerp through points by sample interval
            positionKeyframesY[i] = new Keyframe(((float)i / positionKeyframesY.Length), Mathf.Lerp(startPos.y, endPos.y, (float)i / positionKeyframesY.Length));
        }

        yield return null;

        for (int i = 0; i < positionKeyframesZ.Length; i++)
        {
            // Lerp through points by sample interval
            positionKeyframesZ[i] = new Keyframe(((float)i / positionKeyframesZ.Length), Mathf.Lerp(startPos.z, endPos.z, (float)i / positionKeyframesZ.Length));
        }

        yield return null;

        keyPositions.Clear();

        keyPositions.Add(positionKeyframesX);
        keyPositions.Add(positionKeyframesY);
        keyPositions.Add(positionKeyframesZ);

        finishedKeyPositions = true;
    }

    /// <summary>
    /// Gets the rotation keyframes.
    /// </summary>
    /// <returns>The rotation keyframes.</returns>
    /// <param name="startRot">Start rot.</param>
    /// <param name="endRot">End rot.</param>
    /// <param name="state">State.</param>
    /// <param name="samples">Samples.</param>
    private IEnumerator GetRotationKeyframes(object[] parameters)
    {
        Quaternion startRot = (Quaternion)parameters[0];
        Quaternion endRot = (Quaternion)parameters[1];

        for (int i = 0; i < rotationKeyframesX.Length; i++)
        {
            // Lerp through points by sample interval
            rotationKeyframesX[i] = new Keyframe(((float)i / rotationKeyframesX.Length), Mathf.Lerp(startRot.x, endRot.x, (float)i / rotationKeyframesX.Length));
        }

        yield return null;

        for (int i = 0; i < rotationKeyframesY.Length; i++)
        {
            // Lerp through points by sample interval
            rotationKeyframesY[i] = new Keyframe(((float)i / rotationKeyframesY.Length), Mathf.Lerp(startRot.y, endRot.y, (float)i / rotationKeyframesY.Length));
        }

        yield return null;

        for (int i = 0; i < rotationKeyframesZ.Length; i++)
        {
            // Lerp through points by sample interval
            rotationKeyframesZ[i] = new Keyframe(((float)i / rotationKeyframesZ.Length), Mathf.Lerp(startRot.z, endRot.z, (float)i / rotationKeyframesZ.Length));
        }

        yield return null;

        for (int i = 0; i < rotationKeyframesW.Length; i++)
        {
            // Lerp through points by sample interval
            rotationKeyframesW[i] = new Keyframe(((float)i / rotationKeyframesW.Length), Mathf.Lerp(startRot.w, endRot.w, (float)i / rotationKeyframesW.Length));
        }

        yield return null;

        keyRotations.Clear();

        keyRotations.Add(rotationKeyframesX);
        keyRotations.Add(rotationKeyframesY);
        keyRotations.Add(rotationKeyframesZ);
        keyRotations.Add(rotationKeyframesW);

        finishedKeyRotations = true;
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
            keyPositions.Clear();
            keyRotations.Clear();
            keyframes.Clear();
            curves.Clear();

            // Set up our variables
            AnimationClip clip;
            string clipName;
            Vector3 startPos, endPos;
            Quaternion startRot, endRot;
            int index = anim.GetClipCount();
            finishedKeyRotations = false;
            finishedKeyPositions = false;
            float waitTime;
            float.TryParse(name, out waitTime);
            waitTime /= 10;

            // If we're at a wait anim, set the start and end values to eachother
            if (index % 5 == 0 && index != 0 && !travelTram)
            {
                startPos = positions[index];
                endPos = startPos;

                startRot = rotations[index];
                endRot = startRot;

                waitOffset++;
            }
            // Create an animation that moves to the next position and rotation
            else
            {
                startPos = positions[index - waitOffset];
                endPos = positions[index + 1 - waitOffset];

                startRot = rotations[index - waitOffset];
                endRot = rotations[index + 1 - waitOffset];
            }

            // Get the keyframes between the two positions
            object[] parameters = new object[2]{startPos, endPos};
            StartCoroutine("GetPositionKeyframes", parameters);

            parameters = new object[2]{startRot, endRot};
            StartCoroutine("GetRotationKeyframes", parameters);

            while (!finishedKeyPositions || !finishedKeyRotations)
            {
                yield return null;
            }

            // Combine the x, y, and z coordinates for position and rotation into one list
            for (int j = 0; j < keyPositions.Count; j++)
            {
                keyframes.Add(keyPositions[j]);
            }
            for (int j = 0; j < keyRotations.Count; j++)
            {
                keyframes.Add(keyRotations[j]);
            }

            yield return new WaitForSeconds(waitTime);

            // Create animation curves from the keyframes
            curves = CreateCurve(keyframes);

            // Create the animation clip
            clip = CreateClip(curves);

            // Set the reference name of the clip
            clipName = clipNames[index];
            // Add clip to animation
            anim.AddClip(clip, clipName);

            // If this is the first clip, play it immediately
            if (index == 0)
            {
                anim.Play(clipName);
                states.Add(anim[clipName]); // Add animation state reference to list to allow us to change its speed in LateUpdate
            }
            // If it isn't, add it to the queue
            else
            {
                states.Add(anim.PlayQueued(clipName));  // Add animation state reference to list to allow us to change its speed in LateUpdate
            }
        }
    }
    #endregion
}
