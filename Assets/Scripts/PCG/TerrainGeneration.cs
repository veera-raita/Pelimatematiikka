using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private NoiseSettings[] noiseSettings;

    [Range(1f, 10000f)] public float size = 1000.0f;
    [Range(-100f, 100f)] public float xOffset = 0f;
    [Range(-100f, 100f)] public float yOffset = 0f;

    [Range(10, 255)] public int segments = 100;

    [Range (0.1f, 100f)] public float heightScaler = 10.0f;

    [Range(0.001f, 1f)] public float frequencyScaler = 1.0f;

    public bool useThresholding = false;

    private Mesh mesh;

    private void OnDrawGizmos()
    {
        GenerateMesh();

        float delta = size / (segments - 1);

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                float x = i * delta;
                float y = j * delta;

                Gizmos.color = Color.green;
                //Gizmos.DrawSphere(new Vector3(x, 0, y), .5f);
            }
        }
    }

    void GenerateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Terrain Mesh";
        }
        else
        {
            mesh.Clear();
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> uvs = new List<Vector3>();
        List<int> tris = new List<int>();

        float delta = size / (segments - 1);

        // Generate vertices
        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                float x = i * delta;
                float y = j * delta;

                // Use Perlin Noise
                float noiseValue = 0f;
                for (int k = 0; k < noiseSettings.Length; k++)
                {
                    noiseValue += noiseSettings[k].amplitude * (Mathf.PerlinNoise(noiseSettings[k].frequency * (x + xOffset),
                                                                                  noiseSettings[k].frequency * (y + yOffset)) - 0.5f);
                }
                //Debug.Log(noisevalue);
                if (useThresholding)
                {
                    if (noiseValue < 0.0f)
                    {
                        noiseValue = 0.0f;
                    }
                }
                float height = heightScaler * noiseValue;
                // Add the vertex
                vertices.Add(new Vector3(x,height, y));
                uvs.Add(new Vector2(x + xOffset, y + yOffset) / size);
            }
        }

        // Generate triangles
        for (int i = 0; i < segments-1; i++)
        {
            for (int j = 0;j < segments-1; j++)
            {
                // "Upper left"
                int ul = j * segments + i;

                // "Upper right"
                int ur = ul + 1;

                // "Lower left"
                int ll = ul + segments;

                // "Lower right"
                int lr = ll + 1;

                // Triangles:
                tris.Add(ll);
                tris.Add(ul);
                tris.Add(ur);

                tris.Add(ll);
                tris.Add(ur);
                tris.Add(lr);
            }
        }


        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
