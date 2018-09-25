using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DroneBehavior : MonoBehaviour
{

    public GameObject explosionPrefab;

    public BoxCollider selectedFiringPoint;
    public Vector3 currentTarget = Vector3.positiveInfinity;

    public float maxSpeed = 1;
    public MovementDerivativesTracker movementDerivativesTracker;

    [HideInInspector]
    private float initialDistanceToCurrentTarget = 10000;
    [HideInInspector]
    private float myRadius;

    private float distanceForNewTarget;

    public enum MovementMode { MoveTowardsFiringPoint, RotateTowardsTarget, HoverInPosition, MoveTowardsPlayer};
    public MovementMode movementMode = MovementMode.MoveTowardsFiringPoint;


    // avoiding peers
    GameObject[] spawnersGO;
    Spawner[] spawners;
    private float avoidDistance = 0.8f;

    // hovering
    Vector3 hoverPoint;
    
    float timeSinceHover;
    int randSign = 0;
    Quaternion hoverRotation;
    float horizontalHoverSpeed;
    float verticalHoverSpeed;

    // Attack
    float attackLikelihood = 0.1f;



    List<GameObject> drones;

    // Use this for initialization
    void Start()
    {
        verticalHoverSpeed = Random.Range(0.1f, 0.4f);
        horizontalHoverSpeed = Random.Range(0.1f, 0.4f);
        myRadius = GetComponentInChildren<SphereCollider>().radius;
        distanceForNewTarget = myRadius / 2;
        Debug.Assert(movementDerivativesTracker != null);

        spawnersGO = GameObject.FindGameObjectsWithTag("Spawner");
        spawners = new Spawner[spawnersGO.Length]; 
        for(int i = 0; i < spawners.Length; i++)
        {
            spawners[i] = spawnersGO[i].GetComponent<Spawner>();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
        switch (movementMode)
        {
            case MovementMode.MoveTowardsFiringPoint:
                {
                    if (selectedFiringPoint == null)
                    {
                        SelectRandomFiringPointAndCurrentTarget();
                    }

                    MovePositionTowardsCurrentTarget();

                    if ((currentTarget - transform.position).magnitude < distanceForNewTarget)
                    {
                        // avoid rotation when moving sideways 
                        this.GetComponentInChildren<RotateTowardsMovement>().enabled = false;
                        // search free spot to hover (still buggy)
                        Vector3 vAvoid = AvoidVector();
                        if (vAvoid != Vector3.zero)
                        {
                            transform.position += vAvoid * Time.deltaTime;
                        }
                        else
                        {
                            // save position and time when starting to hover to calculate offsets
                            selectedFiringPoint = null;
                            SetHoverPoint();
                            InvokeRepeating("DecideRandomAttack", 3f, 3f);
                            movementMode = MovementMode.HoverInPosition;
                        }
                    }
                    break;
                }

            case MovementMode.RotateTowardsTarget:
                {
                    break;
                }

            case MovementMode.HoverInPosition:
                {
                    RotateTowardsPlayer();
                    Hover();
                    break;
                }
            case MovementMode.MoveTowardsPlayer:
                {
                    MovePositionTowardsCurrentTarget();
                    if ((currentTarget - transform.position).magnitude < 3f)
                    {
                        SetHoverPoint();
                        movementMode = MovementMode.HoverInPosition;
                    }
                    break;
                }
        }

        if (selectedFiringPoint != null)
        {
            Debug.DrawLine(transform.position, currentTarget, Color.gray);
        }
    }

    private void SetHoverPoint()
    {
        hoverPoint = transform.position;
        timeSinceHover = Time.time;
    }

    private void DecideRandomAttack()
    {
        if(Random.Range(0f, 1.0f) < attackLikelihood)
        {
            currentTarget = Camera.main.transform.position;
            currentTarget.y = transform.position.y;
            CancelInvoke();
            movementMode = MovementMode.MoveTowardsPlayer;
        }
    }

    private void RotateTowardsPlayer()
    {
        float step = 2 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.GetChild(0).forward, Camera.main.transform.position - transform.position, step, 0.0f);
        transform.GetChild(0).rotation = Quaternion.LookRotation(newDir);
    }

    private void Hover()
    {
        float amplitude = 0.2f;
        if (randSign == 0)
        {
            randSign = Random.Range(0f, 1f) < 0.5 ? -1 : 1;
        }
        Vector3 tempPos = transform.position;
        tempPos = hoverPoint + (transform.GetChild(0).right * (randSign * amplitude * Mathf.Sin(horizontalHoverSpeed * (Time.time - timeSinceHover))));

        tempPos.y = hoverPoint.y + (randSign * amplitude * Mathf.Sin(verticalHoverSpeed * (Time.time - timeSinceHover)));
        transform.position = tempPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidDistance);
    }


    private void SelectRandomFiringPointAndCurrentTarget()
    {
        // look up all firing points, sort them near to far
        var firingPointsNearToFar = FindObjectsOfType<DroneFiringPoint>()
            .OrderBy(droneFiringPoint => (droneFiringPoint.transform.position - transform.position).magnitude);

        // select one randomly, nearest with highest probability
        int i = 0;
        while (Random.value > 0.5f)
        {
            i = (i + 1) % firingPointsNearToFar.Count();
        }

        // every firing point must have a box collider
        selectedFiringPoint = firingPointsNearToFar.ElementAt(i).GetComponent<BoxCollider>();

        // pick random point within firing point's box collider as currentTarget
        currentTarget = selectedFiringPoint.transform.TransformPoint(new Vector3(
            selectedFiringPoint.center.x + (Random.value - 0.5f) * selectedFiringPoint.size.x,
            selectedFiringPoint.center.y + (Random.value - 0.5f) * selectedFiringPoint.size.y,
            selectedFiringPoint.center.z + (Random.value - 0.5f) * selectedFiringPoint.size.z
            ));
    }

    private Vector3 AvoidVector()
    {
        Vector3 vAvoid = Vector3.zero;
        for (int i = 0; i < spawners.Length; i++)
        {
            drones = spawners[i].childList;
            foreach (GameObject drone in drones)
            {
                if (drone != gameObject && (drone.transform.position - transform.position).magnitude < avoidDistance)
                {
                    vAvoid += (transform.position - drone.transform.position);
                }
            }

        }
        if(vAvoid.magnitude != 0)
        {
            Debug.DrawRay(transform.position, (vAvoid).normalized, Color.magenta);
        }
        return vAvoid.normalized;
    }

    private void MovePositionTowardsCurrentTarget()
    {
        float currentdistanceToCurrentTarget = (transform.position - currentTarget).magnitude;
        float speedFactor = 1;
        speedFactor = Mathf.Min(speedFactor, currentdistanceToCurrentTarget);
        speedFactor = Mathf.Min(speedFactor, initialDistanceToCurrentTarget - currentdistanceToCurrentTarget);
        speedFactor = Mathf.Max(speedFactor, 0.1f);
        float speed = speedFactor * maxSpeed;
        var direction = (currentTarget - transform.position).normalized;
        var droneRepellingforce = QueryDroneRepellingForce();
        if (droneRepellingforce.magnitude > 0.01f)
        {
            direction = (direction + AvoidVector()).normalized;
            direction = Vector3.Slerp(direction, droneRepellingforce.normalized, droneRepellingforce.magnitude);
            Debug.DrawRay(transform.position, droneRepellingforce, Color.red);
        }
        transform.position += direction * Time.deltaTime * speed;
    }

    private Vector3 QueryDroneRepellingForce()
    {
        Vector3 totalForce = Vector3.zero;
        foreach (DroneRepellingSphere droneRepellingSphere in DroneRepellingSphere.instances)
        {
            totalForce += droneRepellingSphere.QueryNormalizedRepellingVector(transform.position);
        }
        return totalForce;
    }

    private void PickNewRandomTarget()
    {
        //Debug.Log("PickNewRandomTarget");
        RaycastHit raycastHit;
        if (Physics.SphereCast(transform.position, myRadius * 2, Random.onUnitSphere, out raycastHit))
        {
            currentTarget = raycastHit.point;
            initialDistanceToCurrentTarget = (transform.position - currentTarget).magnitude;
            //Debug.Log(currentTarget);
        }
    }

    private bool applicationIsQuitting = false;

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    private void OnDestroy()
    {
        if (!applicationIsQuitting && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
    }
}
