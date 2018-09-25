using UnityEngine;
using System.Collections;

public class DestroyAfterFixedTime : MonoBehaviour {

    public float lifetime = 0.3f;

	void Start () {
        Destroy(gameObject, lifetime);
    }
}
