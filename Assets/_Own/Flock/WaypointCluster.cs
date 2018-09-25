﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCluster : MonoBehaviour {

    public Transform[] waypoints;

    public MoveOnPath flockGoal;

    public List<WaypointCluster> validClusters;

    public bool isSpawner = false;

    public Color lineColor = Color.cyan;

    // Use this for initialization
    void Start () {
        if(isSpawner)
        {
            InvokeRepeating("spawnFlockGoal", 5, 60);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void spawnFlockGoal()
    {
        int index = Random.Range(0, waypoints.Length);
        MoveOnPath mop = Instantiate(flockGoal, waypoints[index].position, Quaternion.identity);
        mop.currentCluster = this;

    }
    
    private void OnDrawGizmosSelected()
    {
        waypoints = GetComponentsInChildren<Transform>();
         for(int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);
        }
    }
                                        

}
