using UnityEditor;
using UnityEngine;

public static class MyDraw
{
    public static void DrawVectorAt(Vector3 _pos, Vector3 _vec, Color _color, float _thickness = 1.0f)
    {
        float handleCapSize = 0.5f;
        Color orig = Handles.color;
        Handles.color = _color;
        Handles.ConeHandleCap(0, _pos + _vec -_vec.normalized * handleCapSize * 0.7f, Quaternion.LookRotation(_vec), handleCapSize, EventType.Repaint);
        Handles.DrawLine(_pos, _pos + _vec, _thickness);
        Handles.color = orig;
    }
}
