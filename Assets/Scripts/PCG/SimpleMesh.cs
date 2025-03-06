using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SimpleMesh : MonoBehaviour
{
    enum GeneratedMesh
    {
        Plane = 0,
        Disc,
        Donut
    }
    
    [SerializeField] GeneratedMesh meshOption = 0;
    [Range(3, 360)] [SerializeField] private int segmentCount;
    [Range(0.1f, 10f)] [SerializeField] private float thickness;
    [Range(0.1f, 10f)] [SerializeField] private float radius;
    [SerializeField] Color color;

    private Mesh GeneratePlane()
    {
        Mesh mesh = new();
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uvPoints = new Vector2[4];

        //init vertices
        vertices[0] = new(0, 0, 0);
        vertices[1] = new(1, 0, 0);
        vertices[2] = new(0, 0, 1);
        vertices[3] = new(1, 0, 1);
        uvPoints[0] = new(0, 0);
        uvPoints[1] = new(1, 0);
        uvPoints[2] = new(0, 1);
        uvPoints[3] = new(1, 1);

        //init triangles
        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        //set vertices
        mesh.vertices = vertices;
        //set triangles
        mesh.triangles = triangles;
        mesh.uv = uvPoints;

        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh GenerateDisc()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvPoints = new();
        float uvScaler = 1 / (radius + thickness);

        vertices.Add(Vector3.zero);
        uvPoints.Add(new Vector2(0.5f, 0.5f));

        float deltaAngle = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = i * deltaAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * (radius + thickness);
            float z = Mathf.Sin(angle) * (radius + thickness);
            // Add the vertex into a suitable data structure (array, list)
            vertices.Add(new Vector3(x, 0, z));

            uvPoints.Add((new Vector2(1, 1) + (new Vector2(x, z) * uvScaler)) / 2f);
        }

        for (int i = 0; i < segmentCount - 1; i++)
        {
            // 1. vertex is always the center (at index 0):
            triangles.Add(0);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
        }
        //"special case" final triangle
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(segmentCount);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvPoints);
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh GenerateDonut()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvPoints = new();
        float uvScaler = 1 / (radius + thickness);

        // vertices.Add(Vector3.zero);

        float deltaAngle = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = i * deltaAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            // Add the vertex into a suitable data structure (array, list)
            vertices.Add(new Vector3(x, 0, z));

            uvPoints.Add((new Vector2(1, 1) + (new Vector2(x, z) * uvScaler)) / 2f);

            angle = i * deltaAngle * Mathf.Deg2Rad;
            x = Mathf.Cos(angle) * (radius + thickness);
            z = Mathf.Sin(angle) * (radius + thickness);
            // Add the vertex into a suitable data structure (array, list)
            vertices.Add(new Vector3(x, 0, z));
            uvPoints.Add((new Vector2(1, 1) + (new Vector2(x, z) * uvScaler)) / 2f);
        }

        for (int i = 0; i < segmentCount * 2 - 2; i += 2)
        {
            triangles.Add(i);
            triangles.Add(i + 3);
            triangles.Add(i + 1);
            triangles.Add(i);
            triangles.Add(i + 2);
            triangles.Add(i + 3);
        }
        //"special case" final triangles
        triangles.Add(segmentCount * 2 - 2);
        triangles.Add(1);
        triangles.Add(segmentCount * 2 - 1);
        triangles.Add(segmentCount * 2 - 2);
        triangles.Add(0);
        triangles.Add(1);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvPoints);
        mesh.RecalculateNormals();

        return mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (meshOption == GeneratedMesh.Plane)
        {
            GetComponent<MeshFilter>().sharedMesh = GeneratePlane();
        }
        else if (meshOption == GeneratedMesh.Disc)
        {
            GetComponent<MeshFilter>().sharedMesh = GenerateDisc();
        }
        else if (meshOption == GeneratedMesh.Donut)
        {
            GetComponent<MeshFilter>().sharedMesh = GenerateDonut();
        }

        GetComponent<MeshRenderer>().material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (meshOption == GeneratedMesh.Plane)
        {
            GetComponent<MeshFilter>().sharedMesh = GeneratePlane();
        }
        else if (meshOption == GeneratedMesh.Disc)
        {
            GetComponent<MeshFilter>().sharedMesh = GenerateDisc();
        }
        else if (meshOption == GeneratedMesh.Donut)
        {
            GetComponent<MeshFilter>().sharedMesh = GenerateDonut();
        }
    }
}
