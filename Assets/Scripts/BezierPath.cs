using UnityEngine;

public class BezierPath : MonoBehaviour
{
    [SerializeField] private BezierPoint[] points;
    [Range(0.0f, 1.0f)] [SerializeField] private float t;

    private void OnDrawGizmos()
    {
        if (points.Length < 2) return;


    }
}