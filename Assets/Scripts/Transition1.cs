using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1 : MonoBehaviour {

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
            break;
        case 1:
            transform.localPosition = Vector3.Lerp(Keys[0].position, Keys[1].position, Mathf.Pow(Blend, 1f));
            break;
        case 2:
            transform.localPosition = Vector3.Lerp(Keys[1].position, Keys[1].position, Mathf.Pow(Blend, 1f));
            break;
        case 3:
            transform.localPosition = Vector3.Lerp(Keys[1].position, Keys[2].position, Mathf.Pow(Blend, 1f));
            break;
        case 4:
            transform.localPosition = Vector3.Lerp(Keys[2].position, Keys[2].position, Mathf.Pow(Blend, 1f));
            break;
        case 5:
            transform.localPosition = Vector3.Lerp(Keys[2].position, Keys[3].position, Mathf.Pow(Blend, 1f));
            break;
        case 6:
            transform.localPosition = Vector3.Lerp(Keys[3].position, Keys[3].position, Mathf.Pow(Blend, 1f));
            break;
        }
    }
}
