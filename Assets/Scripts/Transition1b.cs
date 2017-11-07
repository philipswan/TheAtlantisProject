using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1b : MonoBehaviour {
	[Tooltip("The transforms that the system will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();		// Holds all transforms
    public float TotalTime = 10.0f;								// Total transition time for each case
	public static Transition1b Instance;						// Access script from other objects in the scene

    // Use this for initialization
    void Start () {
		Instance = this;
    }

    void Update() {
        int Scene = (int)Mathf.Floor(Time.unscaledTime / TotalTime);
        float Blend = Mathf.Min (Time.unscaledTime / TotalTime - Scene, 1.0f);

		UpdateSystem(Scene, Blend);
    }

	private void UpdateSystem(int scene, float blend)
	{
		int index = (int)Mathf.Floor(scene/2);

		if (scene % 2 == 0)
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index].position, Mathf.Pow(blend, 1f));
			transform.localRotation = Quaternion.Lerp (Keys[index].localRotation, Keys[index].localRotation, Mathf.Pow (blend, 1.05f));
			transform.localScale = Vector3.Lerp (Keys[index].localScale, Keys[index].localScale, Mathf.Pow (blend, 1.75f));
		}
		else
		{
			transform.localPosition = Vector3.Lerp(Keys[index].position, Keys[index+1].position, Mathf.Pow(blend, 1f));
			transform.localRotation = Quaternion.Lerp (Keys[index].localRotation, Keys[index+1].localRotation, Mathf.Pow (blend, 1.05f));
			transform.localScale = Vector3.Lerp (Keys[index].localScale, Keys[index+1].localScale, Mathf.Pow (blend, 1.75f));
		}
	}

	/// <summary>
	/// Set the transform for the user's elevator
	/// </summary>
	/// <param name="_elevator">Elevator.</param>
	public void MoveToElevator(Transform _elevator)
	{
		Keys.Add(_elevator);
	}
}
