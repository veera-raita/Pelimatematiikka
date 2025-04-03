using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
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
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, transform.right, out hit);

        if (!didHit) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.3f);

        Vector3 reflection = transform.right - 2 * Vector3.Dot(transform.right, hit.normal) * hit.normal;
        // Vector3 reflection2 = Vector3.Reflect(transform.right, hit.normal);
        Handles.DrawLine(hit.point, hit.point + reflection, 3f);

        didHit = Physics.Raycast(hit.point, hit.normal, out hit);
        if (!didHit) return;
        // Gizmos.DrawSphere(hit.point, 0.3f);
        // Handles.DrawLine(hit.point, hit.point + hit.normal * 5f, 3f);
    }
}
