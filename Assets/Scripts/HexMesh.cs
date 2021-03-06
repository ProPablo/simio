using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;

    List<int> triangles;
    MeshCollider meshCollider;
    public float meshHeight = 0.5f;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        meshCollider = gameObject.GetComponent<MeshCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.enabled = false;
        meshCollider.sharedMesh = hexMesh;
        meshCollider.enabled = true;
    }

    private void Triangulate(HexCell cell)
    {
        Vector3 center = cell.transform.localPosition + Vector3.up * meshHeight;
        for (int i = 0; i < 6; i++)
        {
            var nextIndex = i + 1 < 6 ? i + 1 : 0;
            AddTriangle(center,
                center + HexGrid.Corners[i],
                center + HexGrid.Corners[nextIndex]);
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

}