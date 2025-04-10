using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PosLaser : MonoBehaviour
{
    [SerializeField] private Transform car;
    
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);

        bool didHit = Physics.Raycast(transform.position, transform.right, out RaycastHit hit);
        if (!didHit) return;

        //draw laser
        Handles.DrawLine(transform.position, hit.point, 3f);

        //cals right and forward vectors
        Vector3 rightVec = Vector3.Cross(hit.normal, transform.right).normalized;
        Vector3 forwardVec = Vector3.Cross(rightVec, hit.normal).normalized;

        //draw "gizmo"
        MyDraw.DrawVectorAt(hit.point, 3f* hit.normal, Color.green, 3f);
        MyDraw.DrawVectorAt(hit.point, 3f * rightVec, Color.red, 3f);
        MyDraw.DrawVectorAt(hit.point, 3f * forwardVec, Color.blue, 3f);

        //set position and rotation of car
        car.SetPositionAndRotation(hit.point, quaternion.LookRotation(rightVec, hit.normal));
    }
}