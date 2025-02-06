using UnityEditor;
using UnityEngine;

public class WedgeTrigger : MonoBehaviour
{
    [Range(0.1f, 20f)] [SerializeField] private float radius = 5.0f;
    [Range(1f, 180f)] [SerializeField] private float threshold = 45f;

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
        if (vecTargetToTrigger.magnitude <= radius && angle <= threshold)
        {
            Handles.color = Color.red;
        }
        else
        {
            Handles.color = Color.white;
        }

        //Draw arc && lines
        Quaternion rotate = Quaternion.Euler(0, threshold, 0);
        Vector3 rangeRight = rotate * lookVectorNormalized;
        Handles.DrawLine(transform.position, transform.position + rangeRight * radius, 3.0f);
        rotate = Quaternion.Euler(0, -threshold, 0);
        Vector3 rangeLeft = rotate * lookVectorNormalized;
        Handles.DrawLine(transform.position, transform.position + rangeLeft * radius, 3.0f);
        Handles.DrawWireArc(vecTriggerPos, Vector3.up, rangeLeft, threshold*2, radius, 3.0f);
    }
}
