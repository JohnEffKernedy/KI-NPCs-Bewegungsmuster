using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script should be attached to the root pbject of all wand controllers
[RequireComponent(typeof(SteamVR_TrackedController))]
public class SnapPistolToController : MonoBehaviour
{

    public PistolControl pistolPrefab;
    private PistolControl pistolControl;

    void Start()
    {
        Debug.Assert(pistolPrefab != null);

        this.GetComponent<SteamVR_TrackedController>().TriggerClicked += TriggerClicked;
        this.GetComponent<SteamVR_TrackedController>().Gripped += Gripped;
    }

    private void Gripped(object sender, ClickedEventArgs e)
    {
        // pistolControl.gameObject.transform.parent != transform
        if (pistolControl == null)
        {
            // spawn pistol
            pistolControl = Instantiate<PistolControl>(pistolPrefab);

            var pistolTransform = pistolControl.gameObject.transform;
            
            // connect pistol to controller events
            this.GetComponent<SteamVR_TrackedController>().TriggerClicked += pistolControl.OnTriggerPressed;
            this.GetComponent<SteamVR_TrackedController>().TriggerUnclicked += pistolControl.OnTriggerReleased;

            // make controller invisible
            transform.Find("Model").gameObject.SetActive(false);

            // make pistol a child of the controller, so it moves with it
            var localPosition = pistolTransform.localPosition;
            var localRotation = pistolTransform.localRotation;
            pistolTransform.parent = transform;
            pistolTransform.localPosition = localPosition;
            pistolTransform.localRotation = localRotation;
        }
        else
        {
            var pistolTransform = pistolControl.gameObject.transform;

            // disconnect pistol from controller events
            this.GetComponent<SteamVR_TrackedController>().TriggerClicked -= pistolControl.OnTriggerPressed;
            this.GetComponent<SteamVR_TrackedController>().TriggerUnclicked -= pistolControl.OnTriggerReleased;

            // make controller visible
            transform.Find("Model").gameObject.SetActive(true);

            // destroy pistol
            Destroy(pistolControl.gameObject);
            pistolControl = null;
        }
    }

    private void TriggerClicked(object sender, ClickedEventArgs e)
    {
    }

    void Update()
    {

    }
}
