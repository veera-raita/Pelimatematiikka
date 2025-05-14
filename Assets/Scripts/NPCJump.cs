using System.Collections;
using UnityEditor;
using UnityEngine;

public class NPCJump : MonoBehaviour
{
    [Range(0.1f, 20f)] [SerializeField] private float radius = 5.0f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float jumpCooldownTime = 2f;
    private Rigidbody rb;

    private bool jumpOnCooldown = false;
    Vector3 vecTriggerPos => transform.position;
    Vector3 vecTargetPos => targetTransform.position;
    Vector3 vecTargetToTrigger => vecTargetPos - vecTriggerPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (vecTargetToTrigger.magnitude <= radius && !jumpOnCooldown)
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        jumpOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldownTime);
        jumpOnCooldown = false;
    }

    private void OnDrawGizmos()
    {
        //Draw disc
        Handles.color = vecTargetToTrigger.magnitude <= radius ? Color.red : Color.magenta;
        // Handles.color = Color.magenta;
        Handles.DrawWireDisc(vecTriggerPos, Vector3.up, radius, 3.0f);
        Handles.DrawWireDisc(vecTriggerPos, Vector3.left, radius, 3.0f);
        Handles.DrawWireDisc(vecTriggerPos, Vector3.forward, radius, 3.0f);
    }
}
