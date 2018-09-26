using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public GameObject enemyPrefab;
    public AudioSource explosion;
    
    public static int roomSize = 2;

    public float minSpeed = 0.2f;
    public float maxSpeed = 1.5f;
    public float groupDistance = 4.0f;
    public float avoidDistance = 2.0f;
    public float attackDstance = 5f;

    public float speedChangeLikelihood = 1 / 60f;
    public float applyRulesLikelihood = 1 / 5f;

    static int numEnemies = 7;
    public List <GameObject> allEnemies = new List<GameObject>();

    public Transform pathGoal;

    public Vector3 goalPos;

    bool playerActive = true;
    public float inactivityTimer = 30;

	// Use this for initialization
	void Start () {
        explosion = GetComponent<AudioSource>();
        pathGoal = gameObject.transform.parent;
        for (int i = 0; i < numEnemies; i++)
        {
            allEnemies.Add(createEnemy());
        }
	}

    GameObject createEnemy ()
    {
        Vector3 pos = new Vector3(transform.position.x + Random.Range(-roomSize, roomSize),
                                      transform.position.y+Random.Range(-roomSize, roomSize),
                                      transform.position.z+Random.Range(-roomSize, roomSize));
        GameObject newEnemy = (GameObject)Instantiate(enemyPrefab, pos, Quaternion.identity, this.transform);

        Movement movement = newEnemy.GetComponent<Movement>();
        setUpEnemy(movement);
        return newEnemy;
    }

    void setUpEnemy(Movement movement)
    {
        movement.setSpeed(Random.Range(minSpeed, maxSpeed));
        movement.setSpeedRange(minSpeed, maxSpeed);
        movement.setGroupDistance(groupDistance);
        movement.setAvoidDistance(avoidDistance);
        movement.setApplyRulesLikelihood(applyRulesLikelihood);
        movement.setSpeedChangeLikelihood(speedChangeLikelihood);
    }
	
	// Update is called once per frame
	void Update () {
        goalPos = pathGoal.position;
	}

    public void OnLastClusterReached()
    {
        StartCoroutine("PlayAttackPattern");
    }

    IEnumerator PlayAttackPattern()
    {
        while (allEnemies.Count > 0)
        {
            allEnemies[0].GetComponent<Movement>().startAttack();
            yield return new WaitForSeconds(1f);
        }
    }

    // when an enemy hits the player, destroy enemy and spawn another (for demo only)
    // to do: subtract hitpoints from player, respawn according to AI Director 
    public void OnPlayerHit(GameObject enemy)
    {
        explosion.Play(0);
        allEnemies.Remove(enemy);
        GameObject.Destroy(enemy);
    }
}
