using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class DynDNSUpdater : MonoBehaviour {

    public string dyndnsName = "unnamed-vr-project.happyforever.com";
    public string refreshUrl = "https://freedns.afraid.org/dynamic/update.php?VVhoS0JhMzFVMVVBQUh1MWJEUUFBQUFmOjE3NjE5Njc0";

	void OnEnable () {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        Debug.Log("Refreshing DynDNS mapping '" + dyndnsName + "' ...");

        UnityWebRequest www = UnityWebRequest.Get(refreshUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log("Result of refreshing DynDNS mapping for '" + dyndnsName + "': " + www.downloadHandler.text);
        }
    }
}
