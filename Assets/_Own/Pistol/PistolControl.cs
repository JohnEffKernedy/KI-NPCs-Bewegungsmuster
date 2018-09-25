using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolControl : MonoBehaviour {

    public Transform muzzleTransform;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletImpactPrefab;
    public VolumetricLines.VolumetricLineBehavior volumetricLineBehaviorPrefab;

    [Tooltip("Maximum angle of deviation for which the pistol's 'aim bot' will still find the target.")]
    public float aimBotMaxAngleDegrees = 2;

	void Start () {
        if (muzzleTransform == null)
        {
            muzzleTransform = transform.Find("Muzzle");
        }
    }

    public void OnTriggerPressed(object sender, ClickedEventArgs e)
    {
        PistolCharge pistolCharge = GetComponent<PistolCharge>();
        if (pistolCharge != null && pistolCharge.isActiveAndEnabled)
        {
            if (pistolCharge.currentShots < 1)
            {
                return;
            }
            pistolCharge.currentShots -= 1.0f;
        }

        PistolTarget closestPistolTarget = null;
        float closestDistance = float.PositiveInfinity;
        Vector3 pointOnTarget = Vector3.zero;

        foreach (PistolTarget pistolTarget in PistolTarget.instances)
        {
            SphereCollider targetSphere = pistolTarget.sphereCollider;

            Vector3 targetPosition = targetSphere.transform.TransformPoint(targetSphere.center);
            Vector3 localTargetPosition = muzzleTransform.InverseTransformPoint(targetPosition);

            float distanceMuzzleToTargetSphere = localTargetPosition.magnitude;
            if (distanceMuzzleToTargetSphere > 0) // else division by zero
            {
                var angleToTarget = Vector3.Angle(localTargetPosition, Vector3.forward);
                float additionalSlackDegrees = Mathf.Atan(targetSphere.radius / distanceMuzzleToTargetSphere) * Mathf.Rad2Deg;
                if (angleToTarget <= aimBotMaxAngleDegrees + additionalSlackDegrees)
                {
                    // ok, target is in viewcone of weapon's aimbot

                    Vector3 pointNearTarget = muzzleTransform.position + muzzleTransform.forward * Mathf.Max(distanceMuzzleToTargetSphere - targetSphere.radius, 0);
                    pointOnTarget = targetSphere.ClosestPoint(pointNearTarget); // this point would actually be shot at
                    Debug.DrawLine(muzzleTransform.position, pointOnTarget, Color.green);

                    RaycastHit raycastHit;
                    if (Physics.Raycast(muzzleTransform.position, (pointOnTarget - muzzleTransform.position).normalized, out raycastHit))
                    {
                        if (raycastHit.collider == targetSphere)
                        {
                            // ok, line of sight is free

                            closestDistance = localTargetPosition.magnitude;
                            closestPistolTarget = pistolTarget;
                        }
                    }
                }
            }
        }

        Vector3 shotDirection;
        if (closestPistolTarget != null)
        {
            Debug.DrawLine(muzzleTransform.position, pointOnTarget, Color.blue, 1.0f);
            shotDirection = (pointOnTarget - muzzleTransform.position).normalized;

            closestPistolTarget.Hit(42.0f);
        } else
        {
            shotDirection = muzzleTransform.forward;
        }

        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, muzzleTransform.position, muzzleTransform.rotation);
        }

        {
            RaycastHit raycastHit;
            if (Physics.Raycast(muzzleTransform.position, shotDirection, out raycastHit))
            {
                if (bulletImpactPrefab != null)
                {
                    Instantiate(bulletImpactPrefab, raycastHit.point, Quaternion.FromToRotation(Vector3.forward, raycastHit.normal), raycastHit.transform);
                }

                if (volumetricLineBehaviorPrefab != null)
                {
                    var volumetricLineBehavior = Instantiate(volumetricLineBehaviorPrefab);
                    volumetricLineBehavior.StartPos = muzzleTransform.position / volumetricLineBehaviorPrefab.transform.localScale.x;
                    volumetricLineBehavior.EndPos = raycastHit.point / volumetricLineBehaviorPrefab.transform.localScale.x;
                }
            }
        }
    }
    public void OnTriggerReleased(object sender, ClickedEventArgs e)
    {
    }
}
