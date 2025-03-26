using System.Collections.Generic;
using System.Linq;
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

        if (nextSegment > segments) nextSegment = segments;

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

        if (nextSegment > segments) nextSegment = segments;

        //Interpolation, step 1
        Vector3 lerp12 = Vector3.Lerp(points[_currentSegment].anchor, points[_currentSegment].controlB, _t);
        Vector3 lerp23 = Vector3.Lerp(points[_currentSegment].controlB, points[nextSegment].controlA, _t);
        Vector3 lerp34 = Vector3.Lerp(points[nextSegment].controlA, points[nextSegment].anchor, _t);

        //Interpolation, step 2
        Vector3 lerp1223 = Vector3.Lerp(lerp12, lerp23, _t);
        Vector3 lerp2334 = Vector3.Lerp(lerp23, lerp34, _t);

        return (lerp2334 - lerp1223).normalized;
    }

    private void GenerateRoadMesh()
    {
        if (roadCrossSection == null) return;

        if (roadMesh != null)
        {
            roadMesh.Clear();
        }
        else
        {
            roadMesh = new();
        }

        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> uvs = new();
        List<int> triangles = new();

        for (int i = 0; i <= drawnSegmentCount; i++)
        {
            // int sectionsPerSegment = drawnSegmentCount / segments;
            // if (!closedPath) sectionsPerSegment = drawnSegmentCount / segments + 1;
            int iterator;
            if (closedPath) iterator = i < drawnSegmentCount ? i : 0;
            else iterator = i;

            float _t = iterator / (float)drawnSegmentCount;
            float uvT = i / (float)drawnSegmentCount;
            int currentSegment = Mathf.FloorToInt(_t * segments);
            if (currentSegment > segments) currentSegment = segments;
            float adjustedT = (_t - ((float)currentSegment / (float)segments)) / (1.0f / (float)segments);
            float v = (uvT - ((float)currentSegment / (float)segments)) / (1.0f / (float)segments);
            if (v > 1f) v = 1f;
            Debug.Log($"i {i} adjustedT {adjustedT} v {v}");
            Vector3 trackCenter = GetBezierPoint(currentSegment, adjustedT);

            //get forward vector
            Vector3 _forwardVector = GetBezierForwardVector(currentSegment, adjustedT);

            //get "right" vector
            Vector3 _right = Vector3.Cross(Vector3.up, _forwardVector);

            //get "real up" vector
            Vector3 _realUp = Vector3.Cross(_forwardVector, _right);

            //draw points using cross section
            for (int j = 0; j < roadCrossSection.vertices.Length; j++)
            {
                //these are this road crossection's points
                Vector3 vert = roadCrossSection.vertices[j].point.x * _right +
                roadCrossSection.vertices[j].point.y * _realUp;

                //scale road crosssection
                vert *= roadScaler;

                //actual corrected point
                vert += trackCenter - transform.position;

                Gizmos.DrawSphere(vert + transform.position, 0.2f);

                vertices.Add(vert);
                uvs.Add(new Vector2(roadCrossSection.vertices[j].u, v));
                normals.Add((Vector3)roadCrossSection.vertices[j].normal * roadScaler + trackCenter);
            }
        }

        int baseIndex;
        int lowerLeft, lowerRight, upperLeft, upperRight;

        for (int i = 0; i < drawnSegmentCount - 1; i++)
        {
            baseIndex = i * roadCrossSection.vertices.Length;

            for (int j = 1; j < roadCrossSection.vertices.Length - 1; j += 2)
            {
                lowerLeft = j + baseIndex;
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

            lowerLeft = baseIndex + roadCrossSection.vertices.Length - 1;
            lowerRight = baseIndex;
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

        //final set of triangles
        ///////////////////////////////////////////////////////////////////
        baseIndex = (drawnSegmentCount - 1) * roadCrossSection.vertices.Length;

        for (int j = 1; j < roadCrossSection.vertices.Length - 1; j += 2)
        {
            lowerLeft = j + baseIndex;
            lowerRight = lowerLeft + 1;
            upperLeft = closedPath ? j : lowerLeft + roadCrossSection.vertices.Length;
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

        lowerLeft = baseIndex + roadCrossSection.vertices.Length - 1;
        lowerRight = baseIndex;
        upperLeft = lowerLeft + roadCrossSection.vertices.Length;
        upperRight = lowerRight + roadCrossSection.vertices.Length;

        triangles.Add(lowerLeft);
        triangles.Add(upperLeft);
        triangles.Add(upperRight);

        //second triangle
        triangles.Add(lowerLeft);
        triangles.Add(upperRight);
        triangles.Add(lowerRight);
        ////////////////////////////////////////////////////////////////////

        roadMesh.SetVertices(vertices);
        roadMesh.SetNormals(normals);
        roadMesh.SetUVs(0, uvs);
        roadMesh.SetTriangles(triangles, 0);
        roadMesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (points.Length < 2) return;

        GenerateRoadMesh();
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
        
        Vector3?[] previousSegment = new Vector3?[roadCrossSection.vertices.Length];

        for (int i = 0; i < previousSegment.Length; i++)
        {
            previousSegment[i] = null;
        }

        for (int i = 0; i <= drawnSegmentCount; i++)
        {
            // int sectionsPerSegment = drawnSegmentCount / segments;
            // if (!closedPath) sectionsPerSegment = drawnSegmentCount / segments + 1;

            int iterator = i < drawnSegmentCount ? i : 0;
            if (!closedPath && i == drawnSegmentCount) iterator = drawnSegmentCount;

            float _t = iterator / (float)drawnSegmentCount;
            int currentSegment = Mathf.FloorToInt(_t * segments);
            if (currentSegment > segments) currentSegment = segments;
            float adjustedT = (_t - ((float)currentSegment / (float)segments)) / (1.0f / (float)segments);
            Vector3 trackCenter = GetBezierPoint(currentSegment, adjustedT);

            //get forward vector
            Vector3 _forwardVector = GetBezierForwardVector(currentSegment, adjustedT);

            //get "right" vector
            Vector3 _right = Vector3.Cross(Vector3.up, _forwardVector);

            //get "real up" vector
            Vector3 _realUp = Vector3.Cross(_forwardVector, _right);
            

            //draw points using cross section
            for (int j = 0; j < roadCrossSection.vertices.Length - 1; j++)
            {
                //these are this road crossection's points
                Vector3 pointToDraw = roadCrossSection.vertices[j].point.x * _right +
                roadCrossSection.vertices[j].point.y * _realUp;
                Vector3 nextPointToDraw = roadCrossSection.vertices[j + 1].point.x * _right +
                roadCrossSection.vertices[j + 1].point.y * _realUp;

                //scale road crosssection
                pointToDraw *= roadScaler;
                nextPointToDraw *= roadScaler;

                //these are for drawing this segment
                pointToDraw += trackCenter;
                nextPointToDraw += trackCenter;

                //draw lines
                Handles.DrawLine(pointToDraw, nextPointToDraw, 1.5f);
                if (previousSegment[j] != null)
                {
                    Handles.DrawLine(pointToDraw, (Vector3)previousSegment[j], 1.5f);
                }

                //clumsy way of connecting the first and last pieces without using
                //an additional Vector3 array to hold the points
                if (j + 2 == roadCrossSection.vertices.Length)
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
                previousSegment[j] = pointToDraw;
            }
        }

        // for (int i = 0; i < segments; i++)
        // {
        //     int sectionsPerSegment = drawnSegmentCount / segments;
        //     Vector3?[] previousSegment = new Vector3?[roadCrossSection.vertices.Length];

        //     for (int j = 0; j < previousSegment.Length; j++)
        //     {
        //         previousSegment[j] = null;
        //     }

        //     for (int j = 0; j <= sectionsPerSegment; j++)
        //     {
        //         float _t = (float)j / (float)sectionsPerSegment;
        //         Vector3 trackCenter = GetBezierPoint(i, _t);

        //         //get forward vector
        //         Vector3 _forwardVector = GetBezierForwardVector(i, _t);

        //         //get "right" vector
        //         Vector3 _right = Vector3.Cross(Vector3.up, _forwardVector);

        //         //get "real up" vector
        //         Vector3 _realUp = Vector3.Cross(_forwardVector, _right);

        //         //draw points using cross section
        //         for (int k = 0; k < roadCrossSection.vertices.Length - 1; k++)
        //         {
        //             //these are this road crossection's points
        //             Vector3 pointToDraw = roadCrossSection.vertices[k].point.x * _right +
        //             roadCrossSection.vertices[k].point.y * _realUp;
        //             Vector3 nextPointToDraw = roadCrossSection.vertices[k + 1].point.x * _right +
        //             roadCrossSection.vertices[k + 1].point.y * _realUp;

        //             //scale road crosssection
        //             pointToDraw *= roadScaler;
        //             nextPointToDraw *= roadScaler;

        //             //these are for drawing this segment
        //             pointToDraw += trackCenter;
        //             nextPointToDraw += trackCenter;

        //             //draw lines
        //             Handles.DrawLine(pointToDraw, nextPointToDraw, 1.5f);
        //             if (previousSegment[k] != null)
        //             {
        //                 Handles.DrawLine(pointToDraw, (Vector3)previousSegment[k], 1.5f);
        //             }

        //             //clumsy way of connecting the first and last pieces without using
        //             //an additional Vector3 array to hold the points
        //             if (k + 2 == roadCrossSection.vertices.Length)
        //             {
        //                 pointToDraw = roadCrossSection.vertices[^1].point.x * _right +
        //                 roadCrossSection.vertices[^1].point.y * _realUp;
        //                 nextPointToDraw = roadCrossSection.vertices[0].point.x * _right +
        //                 roadCrossSection.vertices[0].point.y * _realUp;

        //                 pointToDraw *= roadScaler;
        //                 nextPointToDraw *= roadScaler;
        //                 pointToDraw += trackCenter;
        //                 nextPointToDraw += trackCenter;
        //                 Handles.DrawLine(pointToDraw, nextPointToDraw, 1.5f);
        //             }
        //             previousSegment[k] = pointToDraw;
        //         }
        //     }
        // }
    }
}