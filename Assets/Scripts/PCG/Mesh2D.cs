using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "New Mesh2D")]
public class Mesh2D : ScriptableObject
{
    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float u;         //U of UV
    }

    public Vertex[] vertices;
    public int[] lineIndices;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
