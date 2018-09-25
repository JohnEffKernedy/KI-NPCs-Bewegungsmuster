using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextDisplay : MonoBehaviour {

    [HideInInspector]
    private Text textComponent;

    [HideInInspector]
    private Spawner[] spawners;

    private float smoothedFrameTime = 0;

    private string lastSpeechResult = "";
    private DateTime timestampOfLastSpeechResult = DateTime.Now;

	// Use this for initialization
	void Start () {
        textComponent = GetComponent<Text>();
        spawners = FindObjectsOfType<Spawner>();
    }

    private void OnEnable()
    {
        SpeechRecognition.OnSpeechResult += OnSpeechResult;
    }

    private void OnDisable()
    {
        SpeechRecognition.OnSpeechResult -= OnSpeechResult;
    }

    private void OnSpeechResult(string text)
    {
        lastSpeechResult = text;
        timestampOfLastSpeechResult = DateTime.Now;
    }

    // Update is called once per frame
    void Update () {
        const float smoothingFactor = 1;
        smoothedFrameTime = Mathf.Lerp(smoothedFrameTime, Time.deltaTime, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactor));

        if (Time.frameCount % 10 == 0)
        {
            int frametimeMS = (int)(smoothedFrameTime * 1000);
            int fps = (int)(1f / smoothedFrameTime);

            int numEnemies = 0;
            foreach (var spawner in spawners)
            {
                numEnemies += spawner.children.transform.childCount;
            }

            string line1 = frametimeMS + "ms " + fps + "FPS";
            string line2 = numEnemies + " Enemies";
            string line3 = timestampOfLastSpeechResult.AddSeconds(5.0) > DateTime.Now ? lastSpeechResult : "";
            textComponent.text = line1 + "\n" + line2 + "\n" + line3;
        }
	}
}
