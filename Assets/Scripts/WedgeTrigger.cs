using UnityEditor;
using UnityEngine;

public class WedgeTrigger : MonoBehaviour
{
    [Range(0.1f, 20f)] [SerializeField] private float radius = 5.0f;
    [Range(1f, 180f)] [SerializeField] private float threshold = 45f;
    [Range(0.1f, 2f)] [SerializeField] private float wedgeHeight = 0.5f;

    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform targetTransform;
    private Color orange = new Color32(255, 80, 0, 255);

    private void OnDrawGizmos()
    {
        Vector3 vecTriggerPos = transform.position;
        Vector3 vecTargetPos = targetTransform.position;
        Vector3 vecTargetToTrigger = vecTargetPos - vecTriggerPos;
        Vector3 normalizedTargetToTrigger = vecTargetToTrigger.normalized;
        Vector3 lookVector = lookTarget.position - transform.position;
        Vector3 lookVectorNormalized = lookVector.normalized;

        float dotProduct = Vector3.Dot(lookVectorNormalized, normalizedTargetToTrigger);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        //look dir
        MyDraw.DrawVectorAt(transform.position, lookVectorNormalized * radius, orange, 3.0f);

        //vec to player
        MyDraw.DrawVectorAt(vecTriggerPos, vecTargetToTrigger, Color.cyan, 3.0f);

        //determine arc color
        if (vecTargetToTrigger.magnitude <= radius && angle <= threshold && Mathf.Abs(vecTargetPos.y) < vecTriggerPos.y + wedgeHeight)
        {
            Handles.color = Color.red;
        }
        else
        {
            Handles.color = Color.white;
        }


        //Draw wedge

        //math for determining arc positioning
        Quaternion rotate = Quaternion.Euler(0, threshold, 0);
        Vector3 rangeRight = rotate * lookVectorNormalized;
        rotate = Quaternion.Euler(0, -threshold, 0);
        Vector3 rangeLeft = rotate * lookVectorNormalized;
        Vector3 offsetTriggerPosDown = new(vecTriggerPos.x, vecTriggerPos.y - wedgeHeight * 0.5f, vecTriggerPos.z);
        Vector3 offsetTriggerPosUp = new(vecTriggerPos.x, vecTriggerPos.y + wedgeHeight * 0.5f, vecTriggerPos.z);

        //actually drawing the wedge: first three lines draw one slice, second three draw second slice higher up
        Handles.DrawLine(offsetTriggerPosDown, offsetTriggerPosDown + rangeRight * radius, 3.0f);
        Handles.DrawLine(offsetTriggerPosDown, offsetTriggerPosDown + rangeLeft * radius, 3.0f);
        Handles.DrawWireArc(offsetTriggerPosDown, Vector3.up, rangeLeft, threshold * 2, radius, 3.0f);
        Handles.DrawLine(offsetTriggerPosUp, offsetTriggerPosUp + rangeRight * radius, 3.0f);
        Handles.DrawLine(offsetTriggerPosUp, offsetTriggerPosUp + rangeLeft * radius, 3.0f);
        Handles.DrawWireArc(offsetTriggerPosUp, Vector3.up, rangeLeft, threshold * 2, radius, 3.0f);

        //connect corners
        Handles.DrawLine(offsetTriggerPosDown + rangeRight * radius, offsetTriggerPosUp + rangeRight * radius, 3.0f);
        Handles.DrawLine(offsetTriggerPosDown + rangeLeft * radius, offsetTriggerPosUp + rangeLeft * radius, 3.0f);
        Handles.DrawLine(offsetTriggerPosDown, offsetTriggerPosUp, 3.0f);
    }
}
