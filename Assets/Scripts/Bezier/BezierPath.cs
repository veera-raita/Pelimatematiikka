using UnityEditor;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    //t values
    [Range(0.0f, 1.0f)] [SerializeField] private float t;
    float adjustedT => AdjustTValue(GetSegment());

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

    private Vector3 GetBezierPoint(int _currentSegment)
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
        Vector3 lerp12 = Vector3.Lerp(points[_currentSegment].anchor, points[_currentSegment].controlB, adjustedT);
        Vector3 lerp23 = Vector3.Lerp(points[_currentSegment].controlB, points[nextSegment].controlA, adjustedT);
        Vector3 lerp34 = Vector3.Lerp(points[nextSegment].controlA, points[nextSegment].anchor, adjustedT);

        //Interpolation, step 2
        Vector3 lerp1223 = Vector3.Lerp(lerp12, lerp23, adjustedT);
        Vector3 lerp2334 = Vector3.Lerp(lerp23, lerp34, adjustedT);

        //Interpolation, final step
        return Vector3.Lerp(lerp1223, lerp2334, adjustedT);
    }

    private void OnDrawGizmos()
    {
        if (points.Length < 2) return;

        for (int i = 0; i < segments; i++)
        {
            if (!closedPath)
                Handles.DrawBezier(points[i].anchor, points[i+1].anchor, points[i].controlB, points[i+1].controlA, Color.white, null, 3.0f);
            else
            {
                if (i < segments - 1)
                    Handles.DrawBezier(points[i].anchor, points[i+1].anchor, points[i].controlB, points[i+1].controlA, Color.white, null, 3.0f);
                else
                    Handles.DrawBezier(points[i].anchor, points[0].anchor, points[i].controlB, points[0].controlA, Color.white, null, 3.0f);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(curvePoint.position, 0.3f);

        curvePoint.position = GetBezierPoint(GetSegment());
    }
}