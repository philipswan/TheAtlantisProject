using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1b : MonoBehaviour {
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
            transform.localRotation = Quaternion.Lerp (key0.localRotation, key1.localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (key0.localScale, key1.localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 1:
            transform.localPosition = Vector3.Lerp(key1.position, key2.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (key1.localRotation, key2.localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (key1.localScale, key2.localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 2:
            transform.localPosition = Vector3.Lerp(key2.position, key3.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (key2.localRotation, key3.localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (key2.localScale, key3.localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 3:
            transform.localPosition = Vector3.Lerp(key3.position, key4.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (key3.localRotation, key4.localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (key3.localScale, key4.localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 4:
            transform.localPosition = Vector3.Lerp(key4.position, key5.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp (key4.localRotation, key5.localRotation, Mathf.Pow (Blend, 1.05f));
            transform.localScale = Vector3.Lerp (key4.localScale, key5.localScale, Mathf.Pow (Blend, 1.75f));
            break;
        case 5:
            transform.localPosition = Vector3.Lerp(key5.position, key6.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp(key5.localRotation, key6.localRotation, Mathf.Pow(Blend, 1.05f));
            transform.localScale = Vector3.Lerp(key5.localScale, key6.localScale, Mathf.Pow(Blend, 1.75f));
            break;
        case 6:
            transform.localPosition = Vector3.Lerp(key6.position, key7.position, Mathf.Pow(Blend, 1f));
            transform.localRotation = Quaternion.Lerp(key6.localRotation, key7.localRotation, Mathf.Pow(Blend, 1.05f));
            transform.localScale = Vector3.Lerp(key6.localScale, key7.localScale, Mathf.Pow(Blend, 1.75f));
            break;
        }
    }
}
