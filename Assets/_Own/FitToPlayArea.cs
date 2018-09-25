using System.Collections;
using UnityEngine;
using Valve.VR;

public class FitToPlayArea : MonoBehaviour
{

    public SteamVR_PlayArea steamVR_PlayArea;

    public void OnEnable()
    {
        if (Application.isPlaying)
        {
            // this code is copied from SteamVR_PlayArea.cs

            // If we want the configured bounds of the user,
            // we need to wait for tracking.
            StartCoroutine(UpdateBounds());
        }
    }

    IEnumerator UpdateBounds()
    {
        var chaperone = OpenVR.Chaperone;
        if (chaperone == null)
        {
            yield break;
        }

        while (chaperone.GetCalibrationState() != ChaperoneCalibrationState.OK)
        {
            yield return null;
        }

        ScaleToPlayArea();
    }


    void ScaleToPlayArea()
    {
        var rect = new HmdQuad_t();
        if (!SteamVR_PlayArea.GetBounds(steamVR_PlayArea.size, ref rect))
        {
            return;
        }
        Vector3 average = Vector3.zero;
        foreach (var corner in new HmdVector3_t[] { rect.vCorners0, rect.vCorners1, rect.vCorners2, rect.vCorners3 })
        {
            average += new Vector3(corner.v0, corner.v1, corner.v2);
        }
        average /= 4;

        transform.position = average;
        Debug.Log("Moved '" + this.name + "' to the center of the SteamVR_PlayArea bounds.");
    }
}
