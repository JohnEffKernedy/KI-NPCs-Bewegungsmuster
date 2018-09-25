using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    SphereCollider col;

    // movement
    public float speed = 0.7f;
    float minSpeed = 0.2f;
    float maxSpeed = 1.2f;
    float acceleration = 0f;
    public float attackDistance = 5f;

    // flocking
    public float groupDistance = 2.0f;
    public float avoidDistance = 0.3f;

    // predictability
    public float speedChangeLikelihood = 1 / 200f;
    public float applyRulesLikelihood = 1 / 5f;



    float rotationSpeed = 5.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;

    bool attacking = false;
    bool turning = false;

    public Vector3 goalPos;
    Flock flock;

    float startTime;
    float duration = 3.0f;

    float returnDist = 1.0f;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
        flock = GetComponentInParent<Flock>();
        goalPos = flock.goalPos;
	}
	
	// Update is called once per frame
	void Update () {
        goalPos = flock.goalPos;
        // fly towards player when within range
        if (Vector3.Distance(this.transform.position, Camera.main.transform.position) < attackDistance)
        {
            attacking = true;
        }
        else attacking = false;

        if(attacking)
        {
            Vector3 directionOfPlayer = Camera.main.transform.position - transform.position;
            turnTo(directionOfPlayer);
        }

        //randomly change speed
        if (Random.Range(0f, 1f) < speedChangeLikelihood)
            speed = Random.Range(minSpeed, maxSpeed);


        //randomly apply rules if not attacking
        if(Random.Range(0f, 1f) < applyRulesLikelihood && !attacking)
            ApplyRules();

        float t = (Time.time - startTime / duration);
        transform.Translate(0, 0, Time.deltaTime * Mathf.SmoothStep(acceleration, speed, t));
	}




    void ApplyRules ()
    {
        List<GameObject> gos;
        gos = Flock.allEnemies;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        float dist;

        int groupSize = 0;


        // when other enemies are within group forming range, collect their positions and speed to calculate the average
        // and adjust the avoid vector for neighbours within avoid distance
        foreach (GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if(dist <= groupDistance)
                {
                    vCentre += go.transform.position;
                    groupSize++;

                    if(dist < avoidDistance)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Movement neighbour = go.GetComponent<Movement>();
                    gSpeed = gSpeed + neighbour.speed;
                }
            }
        }

        // when in a group, find average speed/position and position of player and correct direction and speed accordingly
        if(groupSize > 0)
        {
            vCentre = vCentre / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            turnTo(direction);
        }
    }





    private void turnTo(Vector3 direction)
    {
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                    Quaternion.LookRotation(direction),
                                    rotationSpeed * Time.deltaTime);
    }

    // after hitting the player, tell flockmanager 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) {
            Debug.Log("Hit Player");
            flock.OnPlayerHit(gameObject);
        }
    }
    
    // setter methods for flock manager
    
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void setSpeedRange(float minSpeed, float maxSpeed)
    {
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
    }

    public void setGroupDistance(float groupDistance)
    {
        this.groupDistance = groupDistance;
    }

    public void setAvoidDistance(float avoidDistance)
    {
        this.avoidDistance = avoidDistance;
    }

    public void setSpeedChangeLikelihood (float speedChangeLikelihood)
    {
        this.speedChangeLikelihood = speedChangeLikelihood;
    }

    public void setApplyRulesLikelihood (float applyRulesLikelihood)
    {
        this.applyRulesLikelihood = applyRulesLikelihood;
    }

    public void setAttackDistance(float attackDistance)
    {
        this.attackDistance = attackDistance;
    }

}
