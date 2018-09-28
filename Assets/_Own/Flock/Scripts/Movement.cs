using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    SphereCollider col;
    AudioSource attackAudio;
    AudioSource lightOnAudio;

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
    bool playingAttackSound = false;
    bool playingLightOnSound = false;

    public Vector3 goalPos;
    Flock flock;

    float startTime;
    float duration = 3.0f;

    float returnDist = 1.0f;

    private Vector3 playerPos;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
        playerPos = Camera.main.transform.position;
        flock = GetComponentInParent<Flock>();
        goalPos = flock.goalPos;
        attackAudio = GetComponents<AudioSource>()[0];
        lightOnAudio = GetComponents<AudioSource>()[1];
    }
	
	// Update is called once per frame
	void Update () {
        goalPos = flock.goalPos;

        if(attacking)
        {
            Vector3 directionOfPlayer = Camera.main.transform.position - transform.position;
            turnTo(directionOfPlayer);
            speed = 0;
        }

        //from time to time apply rules if not attacking
        if(Random.Range(0f, 1f) < applyRulesLikelihood && !attacking)
            ApplyRules();

        float t = (Time.time - startTime / duration);

        transform.Translate(0, 0, Time.deltaTime * speed);

	}


    private void playAttackingSound()
    {
        if (playingAttackSound == false){
            attackAudio.Play(0);
            playingAttackSound = true;
        }
    }

    private void playLightOnSound()
    {
        if (playingLightOnSound == false)
        {
            lightOnAudio.Play(0);
            playingLightOnSound = true;
        }
    }

    void ApplyRules ()
    {
        List<GameObject> gos;
        gos = GetComponentInParent<Flock>().allEnemies;

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

            //speed = gSpeed / groupSize;

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

    public void startAttack()
    {
        Debug.Log("starting Attack Coroutine");
        StartCoroutine("Attack");
    }

    public IEnumerator Attack()
    {
        Renderer eye = transform.Find("eye").gameObject.GetComponent<Renderer>();
        attacking = true;
        eye.material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0.1f));
        eye.material.SetColor("_Color", new Color(1f, 0.5f, 0.1f));

        // Threat
        Vector3 toPlayer = playerPos - transform.position;
        while (toPlayer.magnitude > 3)
        {
            toPlayer = playerPos - transform.position;
            transform.position = transform.position + toPlayer * Time.deltaTime * 0.5f;
            yield return null;
        }
        eye.material.SetColor("_EmissionColor", Color.red);
        eye.material.SetColor("_Color", Color.red);
        playLightOnSound();
        playAttackingSound();
        yield return new WaitForSeconds(1f);
        
        // Attack
        while (toPlayer.magnitude > 0.1)
        {
            transform.position = transform.position + toPlayer * Time.deltaTime * 1;
            yield return null;
        }
    }

    // after hitting the player, tell flockmanager 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) {
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

    public void setApplyRulesLikelihood (float applyRulesLikelihood)
    {
        this.applyRulesLikelihood = applyRulesLikelihood;
    }

    public void setAttackDistance(float attackDistance)
    {
        this.attackDistance = attackDistance;
    }

}
