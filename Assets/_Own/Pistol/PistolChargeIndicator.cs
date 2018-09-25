using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PistolChargeIndicator : MonoBehaviour
{
    [HideInInspector]
    private Material chargeIndicatorMaterial;
    [HideInInspector]
    public Color initialEmissionColor;

    [HideInInspector]
    private PistolCharge pistolCharge;

    public float minValue = 0;
    public float maxValue = 1;

    [HideInInspector]
    int[] oldTriangles;

    void Start()
    {
        Debug.Assert(minValue < maxValue);

        pistolCharge = transform.GetComponentInParent<PistolCharge>();
        Debug.Assert(pistolCharge != null);

        chargeIndicatorMaterial = GetComponent<MeshRenderer>().material;
        initialEmissionColor = chargeIndicatorMaterial.GetColor("_EmissionColor");
    }

    void Update()
    {
        float value = Mathf.Min((pistolCharge.currentShots - minValue) / (maxValue - minValue), 1);

        Color newColor = initialEmissionColor * value;
        chargeIndicatorMaterial.SetColor("_EmissionColor", newColor);
    }
}
