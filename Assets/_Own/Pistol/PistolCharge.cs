using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistolCharge : MonoBehaviour {

    [Tooltip("Number of shots that the pistol currently can fire. One click of the trigger consumes 1.0 shots.")]
    public float currentShots;
    [Tooltip("Maximum number of shots that the pistol can store for immediate fire.")]
    public float maxCharge;
    [Tooltip("Unit: Shots per second")]
    public float rechargeRate;

    public Text ammoDisplayText;

	void Start () {
        currentShots = maxCharge;
    }

    void Update()
    {
        currentShots += Time.deltaTime * rechargeRate;
        currentShots = Mathf.Min(currentShots, maxCharge);

        if (ammoDisplayText != null)
        {
            int fullShots = (int)Mathf.Floor(currentShots);
            if (fullShots.ToString() != ammoDisplayText.text)
            {
                ammoDisplayText.text = fullShots.ToString();
            }
        }
    }
}
