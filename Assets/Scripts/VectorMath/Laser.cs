using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Range(0, 16)][SerializeField] private int reflectionCount = 3;

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Gizmos.color = Color.red;

        //declare variables used for calculating and drawing reflections
        Vector3 prevPoint = transform.position;
        Vector3 prevReflect = transform.right;

        for (int i = 0; i <= reflectionCount; i++)
        {
            //if no hit, break loop early. note inlined hit variable declaration
            if (!Physics.Raycast(prevPoint, prevReflect, out RaycastHit hit)) break;
            Vector3 reflection = Vector3.Reflect(prevReflect, hit.normal);

            //draw reflection and hit point
            Handles.DrawLine(prevPoint, hit.point, 3.0f);
            Gizmos.DrawSphere(hit.point, 0.1f);
            
            //save previous variables, used for calcs and drawing again
            prevPoint = hit.point;
            prevReflect = reflection;
        }
    }
}