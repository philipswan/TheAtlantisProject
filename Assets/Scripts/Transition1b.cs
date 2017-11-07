using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1b : MonoBehaviour {
	[Tooltip("The transforms that the system will move to in the order that they entered.")]
	public List<Transform> Keys = new List<Transform>();
    public float TotalTime = 10.0f;

    // Use this for initialization
    void Start () {

    }

    void Update() {
        int Scene = (int)Mathf.Floor(Time.unscaledTime / TotalTime);
        float Blend = Mathf.Min (Time.unscaledTime / TotalTime - Scene, 1.0f);
        switch (Scene) {
        case 0:
            transform.localPosition = Vector3.Lerp(Keys[0].position, Keys[0].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (Keys[0].localRotation, Keys[0].localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (Keys[0].localScale, Keys[0].localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 1:
            transform.localPosition = Vector3.Lerp(Keys[0].position, Keys[1].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (Keys[0].localRotation, Keys[1].localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (Keys[0].localScale, Keys[1].localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 2:
            transform.localPosition = Vector3.Lerp(Keys[1].position, Keys[1].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (Keys[1].localRotation, Keys[1].localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (Keys[1].localScale, Keys[1].localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 3:
            transform.localPosition = Vector3.Lerp(Keys[1].position, Keys[2].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (Keys[1].localRotation, Keys[2].localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (Keys[1].localScale, Keys[2].localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 4:
            transform.localPosition = Vector3.Lerp(Keys[2].position, Keys[2].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (Keys[2].localRotation, Keys[2].localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (Keys[2].localScale, Keys[2].localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 5:
            transform.localPosition = Vector3.Lerp(Keys[2].position, Keys[3].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp(Keys[2].localRotation, Keys[3].localRotation, Mathf.Pow(Blend, 1.05f));
            transform.localScale = Vector3.Lerp(Keys[2].localScale, Keys[3].localScale, Mathf.Pow(Blend, 1.75f));
            break;
        case 6:
            transform.localPosition = Vector3.Lerp(Keys[3].position, Keys[3].position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp(Keys[3].localRotation, Keys[3].localRotation, Mathf.Pow(Blend, 1.05f));
            transform.localScale = Vector3.Lerp(Keys[3].localScale, Keys[3].localScale, Mathf.Pow(Blend, 1.75f));
            break;
		default:
			transform.localPosition = Vector3.Lerp(Keys[Keys.Count-1].position, Keys[Keys.Count-1].position, Mathf.Pow(Blend, 1f));
			transform.localRotation = Quaternion.Lerp(Keys[Keys.Count-1].localRotation, Keys[Keys.Count-1].localRotation, Mathf.Pow(Blend, 1.05f));
			transform.localScale = Vector3.Lerp(Keys[Keys.Count-1].localScale, Keys[Keys.Count-1].localScale, Mathf.Pow(Blend, 1.75f));
			break;
        }
    }
}
