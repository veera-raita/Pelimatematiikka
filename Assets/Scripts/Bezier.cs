using UnityEditor;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    [SerializeField] private Transform point1Transform;
    [SerializeField] private Transform point2Transform;
    [SerializeField] private Transform point3Transform;
    [SerializeField] private Transform point4Transform;
    private Vector3 point1 => point1Transform.position;
    private Vector3 point2 => point2Transform.position;
    private Vector3 point3 => point3Transform.position;
    private Vector3 point4 => point4Transform.position;

    private Vector3 lerp12;
    private Vector3 lerp23;
    private Vector3 lerp34;
    
    private Vector3 lerp1223;
    private Vector3 lerp2334;

    [Range(0.0f, 1.0f)] [SerializeField] float t;

    [SerializeField] private Transform finalPoint;


    private void OnDrawGizmos()
    {
        //Interpolation, step 1
        lerp12 = Vector3.Lerp(point1, point2, t);
        lerp23 = Vector3.Lerp(point2, point3, t);
        lerp34 = Vector3.Lerp(point3, point4, t);

        //Interpolation, step 2
        lerp1223 = Vector3.Lerp(lerp12, lerp23, t);
        lerp2334 = Vector3.Lerp(lerp23, lerp34, t);

        //Interpolation, final step
        finalPoint.position = Vector3.Lerp(lerp1223, lerp2334, t);

        //Draw initial anchor & control points
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(point1, 0.3f);
        Gizmos.DrawSphere(point2, 0.3f);
        Gizmos.DrawSphere(point3, 0.3f);
        Gizmos.DrawSphere(point4, 0.3f);

        //Draw lines between anchors
        // Handles.color = Color.black;
        Handles.DrawLine(point1, point2, 2f);
        Handles.DrawLine(point3, point4, 2f);
        /*
        Handles.DrawLine(point2, point3, 2f);

        //Draw first step interpolation points
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lerp12, 0.3f);
        Gizmos.DrawSphere(lerp23, 0.3f);
        Gizmos.DrawSphere(lerp34, 0.3f);

        //Draw lines between first step interpolations
        Handles.color = Color.cyan;
        Handles.DrawLine(lerp12, lerp23, 2.0f);
        Handles.DrawLine(lerp23, lerp34, 2.0f);

        //Draw second step interpolation points
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lerp1223, 0.3f);
        Gizmos.DrawSphere(lerp2334, 0.3f);

        //Draw lines between second step interpolations
        Handles.color = Color.white;
        Handles.DrawLine(lerp1223, lerp2334, 2.0f);
        */

        //Draw final interpolated point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(finalPoint.position, 0.3f);

        Handles.DrawBezier(point1, point4, point2, point3, Color.white, null, 3.0f);
    }
}
