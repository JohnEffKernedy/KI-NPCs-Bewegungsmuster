using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDerivativesTracker : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 smoothedVelocity;
    public Quaternion angularVelocity;

    [HideInInspector]
    private Vector3 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 deltaPosition = transform.position - oldPosition;
        velocity = deltaPosition / Time.deltaTime;
        const float smoothingFactor = 10;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, velocity, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactor));

        oldPosition = transform.position;
    }
}
