using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameObject eye;
    public AudioSource explosion;
    public AudioSource flyAudio;
    
    public static int roomSize = 2;

    // change from AI Director to influence flock behaviour for new spawns
    public float minSpeed = 0.2f;
    public float maxSpeed = 1.5f;
    public float groupDistance = 4.0f;
    public float avoidDistance = 2.0f;
    public float applyRulesLikelihood = 1 / 5f;

    // change from AI Director to increase/decrease pressure
    public int numEnemies = 7;

    public List <GameObject> allEnemies = new List<GameObject>();

    public Transform pathGoal;
    public Vector3 goalPos;

	// Use this for initialization
	void Start () {
        explosion = GetComponents<AudioSource>()[0];
        flyAudio = GetComponents<AudioSource>()[1];
        flyAudio.pitch *= Random.Range(0.9f, 1.1f);
        
        pathGoal = gameObject.transform.parent;
        for (int i = 0; i < numEnemies; i++)
        {
            allEnemies.Add(createEnemy());
        }
	}

    // should later be called from AI Manager if variables need to be changed before creating enemies
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

    // setting up the flock enemy happens here to change behaviour of new spawns
    void setUpEnemy(Movement movement)
    {
        movement.setSpeed(Random.Range(minSpeed, maxSpeed));
        movement.setSpeedRange(minSpeed, maxSpeed);
        movement.setGroupDistance(groupDistance);
        movement.setAvoidDistance(avoidDistance);
        movement.setApplyRulesLikelihood(applyRulesLikelihood);
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
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    // when an enemy from the flock hits the player it is destroyed here
    // to do: subtract hitpoints from player, respawn according to AI Director 
    public void OnPlayerHit(GameObject enemy)
    {
        explosion.Play(0);
        allEnemies.Remove(enemy);
        GameObject.Destroy(enemy);
    }
}
