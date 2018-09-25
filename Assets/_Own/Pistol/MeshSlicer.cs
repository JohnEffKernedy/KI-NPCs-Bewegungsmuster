using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshSlicer : MonoBehaviour
{
    public int firstTriangle = 0;
    public int numTriangles = 100;
    public int subMesh = 0;

    [HideInInspector]
    int appliedFirstTriangle = 0;
    [HideInInspector]
    int appliedNumTriangles = 0;
    [HideInInspector]
    int[] oldTriangles;

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        oldTriangles = mesh.GetTriangles(subMesh);
    }

    void Update()
    {
        if (appliedFirstTriangle != firstTriangle || appliedNumTriangles != numTriangles)
        {
            firstTriangle = Mathf.Clamp(firstTriangle, 0, oldTriangles.Length / 3 - 1);
            numTriangles = Mathf.Min(numTriangles, oldTriangles.Length / 3 - firstTriangle);

            int[] newTriangles = new int[numTriangles * 3];
            System.Array.Copy(oldTriangles, firstTriangle * 3, newTriangles, 0, newTriangles.Length);
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.SetTriangles(newTriangles, subMesh);
        }
    }
}
