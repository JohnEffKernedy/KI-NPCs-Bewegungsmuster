using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWingRotation : MonoBehaviour {

    public MovementDerivativesTracker movementDerivativesTracker;

	void Update () {
        float forwardSpeed = Vector3.Dot(movementDerivativesTracker.smoothedVelocity, transform.forward);
        transform.localRotation = Quaternion.AngleAxis(forwardSpeed * 20, Vector3.right);
	}
}
