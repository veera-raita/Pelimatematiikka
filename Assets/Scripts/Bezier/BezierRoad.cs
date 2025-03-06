using UnityEditor;
using UnityEngine;

public class BezierRoad : MonoBehaviour
{
    //t values
    [Range(0.0f, 1.0f)][SerializeField] private float t;
    [Range(10, 200)][SerializeField] private int drawnSegmentCount = 50;

    float adjustedT => AdjustTValue(GetSegment());

    [SerializeField] Mesh2D roadCrossSection;

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

    private void OnDrawGizmos()
    {
        if (points.Length < 2) return;

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

        Gizmos.color = Color.red;

        for (int i = 0; i < segments; i++)
        {
            int sectionsPerSegment = drawnSegmentCount / segments;
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
                if (roadCrossSection != null)
                    foreach (Mesh2D.Vertex vertex in roadCrossSection.vertices)
                    {
                        //2D-point x-coodrinate times right-vector + y-coord times realUp-vector
                        Vector3 pointToDraw = vertex.point.x * _right + vertex.point.y * _realUp;
                        pointToDraw += trackCenter;
                        Gizmos.DrawSphere(pointToDraw, 0.2f);
                    }
            }
        }
    }
}