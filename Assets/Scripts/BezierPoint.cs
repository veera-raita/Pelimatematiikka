using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    public Vector3 anchor => transform.position;
    [SerializeField] private Transform controlATransform;
    [SerializeField] private Transform controlBTransform;
    public Vector3 controlA => controlATransform.position;
    public Vector3 controlB => controlBTransform.position;

    [SerializeField] private bool DrawOn = true;

    private void OnDrawGizmos()
    {
        if (!DrawOn) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(anchor, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(controlA, 0.3f);
        Gizmos.DrawSphere(controlB, 0.3f);

        Handles.color = Color.white;
        Handles.DrawLine(anchor, controlA, 2.0f);
        Handles.DrawLine(anchor, controlB, 2.0f);
    }
}