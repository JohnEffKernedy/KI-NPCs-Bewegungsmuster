using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float degreesPerSecond = 10;
    public Vector3 axis = Vector3.up;

    private void Start()
    {
        axis.Normalize();
    }

    void Update () {
        transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * degreesPerSecond, axis);
	}
}
