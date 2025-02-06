using UnityEditor;
using UnityEngine;

public class LookAtTrigger : MonoBehaviour
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
        MyDraw.DrawVectorAt(transform.position, lookVector, orange, 3.0f);
        MyDraw.DrawVectorAt(transform.position, lookVectorNormalized, orange, 3.0f);

        Quaternion rotate = Quaternion.Euler(0, threshold, 0);
        Vector3 rangeLeft = rotate * lookVectorNormalized;
        MyDraw.DrawVectorAt(transform.position, rangeLeft, Color.magenta, 3.0f);
        rotate = Quaternion.Euler(0, -threshold, 0);
        Vector3 rangeRight = rotate * lookVectorNormalized;
        MyDraw.DrawVectorAt(transform.position, rangeRight, Color.magenta, 3.0f);

        //check player color
        Color playerVecColor = angle >= threshold ? Color.green : Color.red;

        //vec to player
        MyDraw.DrawVectorAt(vecTriggerPos, vecTargetToTrigger, playerVecColor, 3.0f);
        MyDraw.DrawVectorAt(vecTriggerPos, normalizedTargetToTrigger, playerVecColor, 3.0f);

        //Draw disc
        Handles.color = vecTargetToTrigger.magnitude <= radius ? Color.red : Color.green;
        Handles.DrawWireDisc(vecTriggerPos, Vector3.up, radius, 3.0f);

        //Draw unit circle
        Handles.color = orange;
        Handles.DrawWireDisc(vecTriggerPos, Vector3.up, 1f, 3.0f);
    }
}
