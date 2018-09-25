using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolTarget : MonoBehaviour
{
    public delegate void PistolHitHandler(GameObject hitGameObject, float someParameter);

    public event PistolHitHandler ThisPistolTargetWasHit;

    public SphereCollider sphereCollider;

    public void Hit(float someParameter)
    {
        if (ThisPistolTargetWasHit != null)
        {
            ThisPistolTargetWasHit(this.gameObject, someParameter);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region tracking all instances

    public static List<PistolTarget> instances = new List<PistolTarget>();

    private void OnEnable()
    {
        if (sphereCollider == null)
        {
            sphereCollider = transform.GetComponentInChildren<SphereCollider>();
        }

        instances.Add(this);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    #endregion


}
