using UnityEditor;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    public Vector3 anchor => transform.position;
    [SerializeField] private Transform controlATransform;
    [SerializeField] private Transform controlBTransform;
    public Vector3 controlA => controlATransform.position;
    public Vector3 controlB => controlBTransform.position;
    [SerializeField] private bool DrawOn = true;


    //these two should probably be generalized into the same method
    private void FixBPosition()
    {
        float bLength = Vector3.Distance(Vector3.zero, controlBTransform.localPosition);
        Vector3 newBPos = controlATransform.localPosition.normalized;
        newBPos = bLength * - newBPos;
        controlBTransform.localPosition = newBPos;
    }

    private void FixAPosition()
    {
        float aLength = Vector3.Distance(Vector3.zero, controlATransform.localPosition);
        Vector3 newAPos = controlBTransform.localPosition.normalized;
        newAPos = aLength * -newAPos;
        controlATransform.localPosition = newAPos;
    }

    private void OnDrawGizmos()
    {
        if (controlATransform.hasChanged)
        {
            FixBPosition();
            controlATransform.hasChanged = false;
        }
        else if (controlBTransform.hasChanged)
        {
            FixAPosition();
            controlBTransform.hasChanged = false;
        }


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