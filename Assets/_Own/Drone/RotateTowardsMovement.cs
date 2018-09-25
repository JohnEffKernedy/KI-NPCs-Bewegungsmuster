using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMovement : MonoBehaviour {

    public MovementDerivativesTracker movementDerivativesTracker;
	
	void Update () {
        Vector3 forward = movementDerivativesTracker.velocity.normalized;
        float forwardSpeed = Vector3.Dot(movementDerivativesTracker.velocity, transform.forward);

        //forward.y = -forwardSpeed;
        //forward.Normalize();
        //transform.localRotation = Quaternion.FromToRotation(Vector3.forward, forward);

        Vector3 horizontalForward = forward;
        horizontalForward.y = 0;
        horizontalForward.Normalize();
        Quaternion horizontalRotation = Quaternion.FromToRotation(Vector3.forward, horizontalForward);

        Quaternion targetRotation = horizontalRotation * Quaternion.AngleAxis(forwardSpeed * 10, Vector3.right);
        const float smoothingFactor = 2;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactor));
	}
}
