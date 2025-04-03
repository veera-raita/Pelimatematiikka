using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PosLaser : MonoBehaviour
{
    [SerializeField] private Transform car;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawLine(transform.position, transform.position + transform.right * 5f, 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, transform.right, out hit);

        if (!didHit) return;

        car.position = hit.point;
        car.up = hit.normal;

        MyDraw.DrawVectorAt(hit.point, 3f* hit.normal, Color.green, 3f);
        // MyDraw.DrawVectorAt(hit.point, 3f* transform.right, Color.magenta, 3f);

        Vector3 rightVec = Vector3.Cross(hit.normal, transform.right).normalized;
        Vector3 forwardVec = Vector3.Cross(rightVec, hit.normal).normalized;
        MyDraw.DrawVectorAt(hit.point, 3f* rightVec, Color.red, 3f);
        MyDraw.DrawVectorAt(hit.point, 3f* forwardVec, Color.blue, 3f);

        car.position = hit.point;
        car.up = hit.normal;
        car.right = forwardVec;
        car.forward = rightVec;

        didHit = Physics.Raycast(hit.point, hit.normal, out hit);
        if (!didHit) return;
        // Gizmos.DrawSphere(hit.point, 0.3f);
        // Handles.DrawLine(hit.point, hit.point + hit.normal * 5f, 3f);
    }
}
