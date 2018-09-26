using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnPath : MonoBehaviour {

    public Flock flockPrefab;

    public int currentWaypointID = 0;
    public float speed;
    public float reachDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public string pathName;

    Vector3 last_pos;
    Vector3 current_pos;

    public WaypointCluster currentCluster;
    Transform currentGoal;

    private Flock flock;

	// Use this for initialization
	void Start ()
    {
        flock = Instantiate(flockPrefab, transform.position, Quaternion.identity, this.transform);
        Debug.Log(currentCluster);
        currentGoal = NextGoal();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(currentGoal != null)
        {
            Vector3 toWaypoint = (currentGoal.position - transform.position);
            float distanceToWaypoint = toWaypoint.magnitude;
            if (reachDistance < distanceToWaypoint)
            {
                transform.Translate(toWaypoint.normalized * Time.deltaTime * speed);
            }
            else
            {
                currentGoal = NextGoal();
            }
        }
	}

    private Transform NextGoal()
    {
        List<WaypointCluster> validClusters = currentCluster.validClusters;
        if(validClusters.Count == 0)
        {
            flock.OnLastClusterReached();
            return null;
        }
        int index = Random.Range(0, validClusters.Count);
        currentCluster = validClusters[index];
        Transform[] waypoints = currentCluster.GetComponentsInChildren<Transform>();
        index = Random.Range(0, waypoints.Length);
        return waypoints[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
