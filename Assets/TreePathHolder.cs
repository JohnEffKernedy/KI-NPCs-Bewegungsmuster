using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePathHolder : MonoBehaviour
{

    // path code from https://youtu.be/1aBjTa3xQzE

    public Color rayColor = Color.blue;
    public List<Transform> path_objs = new List<Transform>();
    Transform[] theArray;

    private void Start()
    {
        path_objs.Reverse();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        theArray = GetComponentsInChildren<Transform>();

        path_objs.Clear();

        foreach (Transform path_obj in theArray)
        {
            if (path_obj != this.transform)
            {
                path_objs.Add(path_obj);
            }
        }


        for (int i = 0; i < path_objs.Count; i++)
        {
            if(path_objs[i].parent != null)
            {
                Gizmos.DrawLine(path_objs[i].parent.position, path_objs[i].position);
            }
        }

    }
}
