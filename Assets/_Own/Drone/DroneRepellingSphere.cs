using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DroneRepellingSphere : MonoBehaviour
{
    [Tooltip("Within this sphere the repelling force is maximal.")]
    public SphereCollider innerSphereCollider;
    [Tooltip("Outside of this sphere the repelling force is zero.")]
    public SphereCollider outerSphereCollider;

    [HideInInspector]
    public static List<DroneRepellingSphere> instances = new List<DroneRepellingSphere>();

    private void OnEnable()
    {
        instances.Add(this);

        var sphereCollidersSmallToBig = GetComponents<SphereCollider>().OrderBy(sphereCollider => sphereCollider.radius);
        innerSphereCollider = sphereCollidersSmallToBig.ElementAt(0);
        outerSphereCollider = sphereCollidersSmallToBig.ElementAt(1);

        Debug.Assert(innerSphereCollider.radius > 0);
        Debug.Assert(outerSphereCollider.radius > innerSphereCollider.radius);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    public Vector3 QueryNormalizedRepellingVector(Vector3 queryPoint)
    {
        Vector3 localQueryPoint = transform.InverseTransformPoint(queryPoint);
        float distanceFromCenter = localQueryPoint.magnitude;
        if (distanceFromCenter < innerSphereCollider.radius * 0.01f || distanceFromCenter >= outerSphereCollider.radius)
        {
            return Vector3.zero;
        }
        else
        {
            float alpha = (outerSphereCollider.radius - distanceFromCenter) / (outerSphereCollider.radius - innerSphereCollider.radius);
            Vector3 localRepellingForce = localQueryPoint.normalized * Mathf.Clamp01(alpha);
            return transform.TransformDirection(localRepellingForce);
        }
    }
}
