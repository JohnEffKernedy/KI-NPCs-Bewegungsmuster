using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public List<WaypointCluster> spawner;
    public float spawnRate = 20f;

	// Use this for initialization
	void Start () {
        // for demo purposes, spawn rates should be set from AI Manager
        InvokeRepeating("ActivateRandomSpawner", 2, spawnRate);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ActivateRandomSpawner()
    {
        int index = Random.Range(0, spawner.Count);
        spawner[index].spawnFlockGoal();
    }
}
