using UnityEditor;
using UnityEngine;

public class RadialTrigger : MonoBehaviour
{
    [Range(0.1f, 20f)] [SerializeField] private float radius = 5.0f;
    [SerializeField] private Transform targetTransform;

    private void OnDrawGizmos()
    {
        Vector3 vecTriggerPos = transform.position;
        Vector3 vecTargetPos = targetTransform.position;
        Vector3 vecTargetToTrigger = vecTargetPos - vecTriggerPos;

        MyDraw.DrawVectorAt(vecTriggerPos, vecTargetToTrigger, Color.magenta, 3.0f);

        //Draw disc
        Handles.color = vecTargetToTrigger.magnitude <= radius ? Color.red : Color.magenta;
        // Handles.color = Color.magenta;
        Handles.DrawWireDisc(vecTriggerPos, Vector3.up, radius, 3.0f);
    }
}
