using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    AudioSource attackAudio;
    AudioSource lightOnAudio;

    // movement
    public float speed = 0.7f;
    float minSpeed = 0.2f;
    float maxSpeed = 1.2f;
    float rotationSpeed = 5.0f;

    // Attack
    float pauseTime = 1.0f;

    // flocking
    public float groupDistance = 2.0f;
    public float avoidDistance = 0.3f;

    // predictability
    public float applyRulesLikelihood = 1 / 5f;

    // booleans
    bool attacking = false;
    bool turning = false;
    bool playingAttackSound = false;
    bool playingLightOnSound = false;

    
    private Vector3 goalPos;
    private Vector3 playerPos;
    Flock flock;

    // eye
    public Color warningColor = new Color(1f, 0.5f, 0.1f);
    public Color attackColor = Color.red;

    // Use this for initialization
    void Start () {
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
        }

        //from time to time apply rules if not attacking
        if(Random.Range(0f, 1f) < applyRulesLikelihood && !attacking)
            ApplyRules();

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
        List<GameObject> gos = GetComponentInParent<Flock>().allEnemies; ;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
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
                }
            }
        }

        // when in a group, find average position and position of goal and correct direction and speed accordingly
        if(groupSize > 0)
        {
            vCentre = vCentre / groupSize + (goalPos - this.transform.position);
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
        StartCoroutine("Attack");
    }

    public IEnumerator Attack()
    {
        Renderer eye = transform.Find("eye").gameObject.GetComponent<Renderer>();
        speed = 0;
        attacking = true;
        eye.material.SetColor("_EmissionColor", warningColor);
        eye.material.SetColor("_Color", warningColor);

        // Threat
        Vector3 toPlayer = playerPos - transform.position;
        while (toPlayer.magnitude > 3)
        {
            toPlayer = playerPos - transform.position;
            transform.position = transform.position + toPlayer * Time.deltaTime * 0.5f;
            yield return null;
        }
        eye.material.SetColor("_EmissionColor", attackColor);
        eye.material.SetColor("_Color", attackColor);
        playLightOnSound();
        playAttackingSound();
        yield return new WaitForSeconds(pauseTime);
        
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

    public void setPauseTime(float pauseTime)
    {
        this.pauseTime = pauseTime;
    }

}
