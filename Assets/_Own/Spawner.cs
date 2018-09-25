using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject prefab;

    public int maxInstances = 5;

    public GameObject children;
    public List<GameObject> childList;

	// Use this for initialization
	void Start () {
        children = new GameObject("Spawned Instances");
        children.transform.parent = transform;
        children.transform.position = Vector3.zero;
        children.transform.rotation = Quaternion.identity;

        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndSpawn()
    {
        while (true)
        {
            if (children.transform.childCount < maxInstances)
            {
                SpawnInstance();
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void SpawnInstance()
    {
        GameObject newSpawn = Instantiate(prefab, transform.position, transform.rotation, children.transform);
        childList.Add(newSpawn);
    }
}
