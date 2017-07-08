using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1 : MonoBehaviour {

    public Transform key0;
    public Transform key1;
    public Transform key2;
    public Transform key3;
    public Transform key4;
    public Transform key5;
    public Transform key6;
    public Transform key7;
    public float TotalTime = 10.0f;
    // Use this for initialization
    void Start () {

    }

    void Update() {
        int Scene = (int)Mathf.Floor(Time.unscaledTime / TotalTime);
        float Blend = Mathf.Min (Time.unscaledTime / TotalTime - Scene, 1.0f);
        switch (Scene) {
        case 0:
            transform.localPosition = Vector3.Lerp(key0.position, key1.position, Mathf.Pow(Blend, 1f));
            break;
        case 1:
            transform.localPosition = Vector3.Lerp(key1.position, key2.position, Mathf.Pow(Blend, 1f));
            break;
        case 2:
            transform.localPosition = Vector3.Lerp(key2.position, key3.position, Mathf.Pow(Blend, 1f));
            break;
        case 3:
            transform.localPosition = Vector3.Lerp(key3.position, key4.position, Mathf.Pow(Blend, 1f));
            break;
        case 4:
            transform.localPosition = Vector3.Lerp(key4.position, key5.position, Mathf.Pow(Blend, 1f));
            break;
        case 5:
            transform.localPosition = Vector3.Lerp(key5.position, key6.position, Mathf.Pow(Blend, 1f));
            break;
        case 6:
            transform.localPosition = Vector3.Lerp(key6.position, key7.position, Mathf.Pow(Blend, 1f));
            break;
        }
    }
}
