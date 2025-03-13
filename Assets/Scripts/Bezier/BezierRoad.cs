using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierRoad : MonoBehaviour
{
    [Range(0.0f, 1.0f)][SerializeField] private float t;
    [Range(10, 200)][SerializeField] private int drawnSegmentCount = 50;
    [Range(0.1f, 5.0f)][SerializeField] private float roadScaler = 1f;

    float adjustedT => AdjustTValue(GetSegment());

    //road stuff
    [SerializeField] Mesh2D roadCrossSection;
    private Mesh roadMesh;

    //bezier points and t visualizer
    [SerializeField] private BezierPoint[] points;
    [SerializeField] private Transform curvePoint;
    private int segments => closedPath ? points.Length : points.Length - 1;
    [SerializeField] bool closedPath = false;


    private int GetSegment()
    {
        return Mathf.FloorToInt(segments * t) < segments ? Mathf.FloorToInt(segments * t) : segments - 1;
    }

    private float AdjustTValue(int _segment)
    {
        return (t - ((float)_segment / (float)segments)) / (1.0f / (float)segments);
    }

    private Vector3 GetBezierPoint(int _currentSegment, float _t)
    {
        int nextSegment;

        if (!closedPath || _currentSegment < segments - 1)
        {
            nextSegment = _currentSegment + 1;
        }
        else
        {
            nextSegment = 0;
        }

        //Interpolation, step 1
        Vector3 lerp12 = Vector3.Lerp(points[_currentSegment].anchor, points[_currentSegment].controlB, _t);
        Vector3 lerp23 = Vector3.Lerp(points[_currentSegment].controlB, points[nextSegment].controlA, _t);
        Vector3 lerp34 = Vector3.Lerp(points[nextSegment].controlA, points[nextSegment].anchor, _t);

        //Interpolation, step 2
        Vector3 lerp1223 = Vector3.Lerp(lerp12, lerp23, _t);
        Vector3 lerp2334 = Vector3.Lerp(lerp23, lerp34, _t);

        //Interpolation, final step
        return Vector3.Lerp(lerp1223, lerp2334, _t);
    }

    private Vector3 GetBezierForwardVector(int _currentSegment, float _t)
    {
        int nextSegment;

        if (!closedPath || _currentSegment < segments - 1)
        {
            nextSegment = _currentSegment + 1;
        }
        else
        {
            nextSegment = 0;
        }

        //Interpolation, step 1
        Vector3 lerp12 = Vector3.Lerp(points[_currentSegment].anchor, points[_currentSegment].controlB, _t);
        Vector3 lerp23 = Vector3.Lerp(points[_currentSegment].controlB, points[nextSegment].controlA, _t);
        Vector3 lerp34 = Vector3.Lerp(points[nextSegment].controlA, points[nextSegment].anchor, _t);

        //Interpolation, step 2
        Vector3 lerp1223 = Vector3.Lerp(lerp12, lerp23, _t);
        Vector3 lerp2334 = Vector3.Lerp(lerp23, lerp34, _t);

        return (lerp2334 - lerp1223).normalized;
    }

    private Mesh GenerateRoadMesh(Mesh _currentMesh)
    {
        if (roadCrossSection == null) return null;

        Mesh _roadMesh;
        
        if (_currentMesh != null)
        {
            _currentMesh.Clear();
            _roadMesh = _currentMesh;
        }
        else
        {
            _roadMesh = new();
        }

        List<Vector3> vertices = new();
        List<int> triangles = new();

        for (int i = 0; i < segments; i++)
        {
            int sectionsPerSegment = drawnSegmentCount / segments;
            Vector3?[] previousSegment = new Vector3?[roadCrossSection.vertices.Length];
            
            for(int j = 0 ; j < previousSegment.Length; j++)
            {
                previousSegment[j] = null;
            }

            for (int j = 0; j < sectionsPerSegment; j++)
            {
                float _t = (float)j / (float)sectionsPerSegment;
                Vector3 trackCenter = GetBezierPoint(i, _t);

                //get forward vector
                Vector3 _forwardVector = GetBezierForwardVector(i, _t);

                //get "right" vector
                Vector3 _right = Vector3.Cross(Vector3.up, _forwardVector);

                //get "real up" vector
                Vector3 _realUp = Vector3.Cross(_forwardVector, _right);

                //draw points using cross section
                for (int k = 0; k < roadCrossSection.vertices.Length; k++)
                {
                    //these are this road crossection's points
                    Vector3 vert = roadCrossSection.vertices[k].point.x * _right +
                    roadCrossSection.vertices[k].point.y * _realUp;

                    //scale road crosssection
                    vert *= roadScaler;

                    //actual corrected point
                    vert += trackCenter;

                    Gizmos.DrawSphere(vert, 0.2f);

                    vertices.Add(vert);

                    //clumsy way of connecting the first and last pieces without using
                    //an additional Vector3 array to hold the points
                    // if (k + 2 == roadCrossSection.vertices.Length)
                    // {
                    //     vert = roadCrossSection.vertices[^1].point.x * _right +
                    //     roadCrossSection.vertices[^1].point.y * _realUp;
                        
                    //     vert *= roadScaler;
                    //     vert += trackCenter;
                    // }
                    previousSegment[k] = vert;
                }
            }
        }

        //USE MORE FOR LOOPS
        for (int i = 0; i < drawnSegmentCount - 1; i++)
        {
            int indexOffset = i * roadCrossSection.vertices.Length;

            int lowerLeft, lowerRight, upperLeft, upperRight;

            for (int j = 1; j < roadCrossSection.vertices.Length - 1; j += 2)
            {
                lowerLeft = j + indexOffset;
                lowerRight = lowerLeft + 1;
                upperLeft = lowerLeft + roadCrossSection.vertices.Length;
                upperRight = upperLeft + 1;

                //first triangle
                triangles.Add(lowerLeft);
                triangles.Add(upperLeft);
                triangles.Add(upperRight);

                //second triangle
                triangles.Add(lowerLeft);
                triangles.Add(upperRight);
                triangles.Add(lowerRight);
            }

            lowerLeft = indexOffset + roadCrossSection.vertices.Length - 1;
            lowerRight = indexOffset;
            upperLeft = lowerLeft + roadCrossSection.vertices.Length;
            upperRight = lowerRight + roadCrossSection.vertices.Length;

            triangles.Add(lowerLeft);
            triangles.Add(upperLeft);
            triangles.Add(upperRight);

            //second triangle
            triangles.Add(lowerLeft);
            triangles.Add(upperRight);
            triangles.Add(lowerRight);
        }

        _roadMesh.SetVertices(vertices);
        _roadMesh.SetTriangles(triangles, 0);
        _roadMesh.RecalculateNormals();

        return _roadMesh;
        //returning here
    }

    private void OnDrawGizmos()
    {
        if (points.Length < 2) return;

        roadMesh = GenerateRoadMesh(roadMesh);
        roadMesh.name = "generated road mesh";
        GetComponent<MeshFilter>().sharedMesh = roadMesh;

        for (int i = 0; i < segments; i++)
        {
            if (!closedPath)
                Handles.DrawBezier(points[i].anchor, points[i + 1].anchor, points[i].controlB, points[i + 1].controlA, Color.white, null, 3.0f);
            else
            {
                if (i < segments - 1)
                    Handles.DrawBezier(points[i].anchor, points[i + 1].anchor, points[i].controlB, points[i + 1].controlA, Color.white, null, 3.0f);
                else
                    Handles.DrawBezier(points[i].anchor, points[0].anchor, points[i].controlB, points[0].controlA, Color.white, null, 3.0f);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(curvePoint.position, 0.3f);

        curvePoint.position = GetBezierPoint(GetSegment(), adjustedT);

        //get & draw forward vector
        Vector3 forwardVector = GetBezierForwardVector(GetSegment(), adjustedT);
        MyDraw.DrawVectorAt(curvePoint.position, forwardVector * 3.0f, Color.blue, 3.0f);

        //get & draw "right" vector
        Vector3 right = Vector3.Cross(Vector3.up, forwardVector);
        MyDraw.DrawVectorAt(curvePoint.position, right * 3.0f, Color.red, 3.0f);

        //get & draw "real up" vector
        Vector3 realUp = Vector3.Cross(forwardVector, right);
        MyDraw.DrawVectorAt(curvePoint.position, realUp * 3.0f, Color.green, 3.0f);

        Handles.color = Color.yellow;

        
        if (roadCrossSection == null) return;

        for (int i = 0; i < segments; i++)
        {
            int sectionsPerSegment = drawnSegmentCount / segments;
            Vector3?[] previousSegment = new Vector3?[roadCrossSection.vertices.Length];
            
            for(int j = 0 ; j < previousSegment.Length; j++)
            {
                previousSegment[j] = null;
            }

            for (int j = 0; j <= sectionsPerSegment; j++)
            {
                float _t = (float)j / (float)sectionsPerSegment;
                Vector3 trackCenter = GetBezierPoint(i, _t);

                //get forward vector
                Vector3 _forwardVector = GetBezierForwardVector(i, _t);

                //get "right" vector
                Vector3 _right = Vector3.Cross(Vector3.up, _forwardVector);

                //get "real up" vector
                Vector3 _realUp = Vector3.Cross(_forwardVector, _right);

                //draw points using cross section
                for (int k = 0; k < roadCrossSection.vertices.Length - 1; k++)
                {
                    //these are this road crossection's points
                    Vector3 pointToDraw = roadCrossSection.vertices[k].point.x * _right +
                    roadCrossSection.vertices[k].point.y * _realUp;
                    Vector3 nextPointToDraw = roadCrossSection.vertices[k + 1].point.x * _right +
                    roadCrossSection.vertices[k + 1].point.y * _realUp;

                    //scale road crosssection
                    pointToDraw *= roadScaler;
                    nextPointToDraw *= roadScaler;

                    //these are for drawing this segment
                    pointToDraw += trackCenter;
                    nextPointToDraw += trackCenter;

                    //draw lines
                    Handles.DrawLine(pointToDraw, nextPointToDraw, 1.5f);
                    if (previousSegment[k] != null)
                    {
                        Handles.DrawLine(pointToDraw, (Vector3)previousSegment[k], 1.5f);
                    }

                    //clumsy way of connecting the first and last pieces without using
                    //an additional Vector3 array to hold the points
                    if (k + 2 == roadCrossSection.vertices.Length)
                    {
                        pointToDraw = roadCrossSection.vertices[^1].point.x * _right +
                        roadCrossSection.vertices[^1].point.y * _realUp;
                        nextPointToDraw = roadCrossSection.vertices[0].point.x * _right +
                        roadCrossSection.vertices[0].point.y * _realUp;
                        
                        pointToDraw *= roadScaler;
                        nextPointToDraw *= roadScaler;
                        pointToDraw += trackCenter;
                        nextPointToDraw += trackCenter;
                        Handles.DrawLine(pointToDraw, nextPointToDraw, 1.5f);
                    }
                    previousSegment[k] = pointToDraw;
                }
            }
        }
    }
}