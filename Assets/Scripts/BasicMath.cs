using UnityEditor;
using UnityEngine;

public class BasicMath : MonoBehaviour
{
    [Range(0.1f, 10f)][SerializeField] private float axisLength = 3f;
    [Range(0.1f, 10f)][SerializeField] private float originAxisLength = 3f;
    [Range(0.1f, 10f)][SerializeField] private float radius = 1f;
    [SerializeField] private Vector3 rectStart = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 rectSize = new Vector3(0, 0, 0);
    [SerializeField] private Transform trackedTransform;
    private Vector3 trackedPoint => trackedTransform.position;


    [Range(0.01f, 1f)][SerializeField] private float popupWidthPercent = 0.6f;
    [Range(0.01f, 1f)][SerializeField] private float popupHeightPercent = 0.35f;
    [SerializeField] private Vector2 screenSize;
    [SerializeField] private Vector2 screenPos;

    private void DrawVector(Vector3 _start, Vector3 _vec, Color _color, float _thickness)
    {
        float handleCapSize = 0.5f;
        Handles.color = _color;
        Handles.ConeHandleCap(0, _start + _vec -_vec.normalized * handleCapSize * 0.7f, Quaternion.LookRotation(_vec), handleCapSize, EventType.Repaint);
        Handles.DrawLine(_start, _start + _vec, _thickness);
    }

    private void DrawRect(Vector3 _rectStart, Vector3 _rectSize, Color _color)
    {
        Handles.color = _color;
        Handles.DrawLine(_rectStart, new Vector3(_rectStart.x + _rectSize.x, _rectStart.y), 3.0f);
        Handles.DrawLine(new Vector3(_rectStart.x + _rectSize.x, _rectStart.y), new Vector3(_rectStart.x + _rectSize.x, _rectStart.y + _rectSize.y), 3.0f);
        Handles.DrawLine(_rectStart, new Vector3(_rectStart.x, _rectStart.y + _rectSize.y), 3.0f);
        Handles.DrawLine(new Vector3(_rectStart.x, _rectStart.y + _rectSize.y), new Vector3(_rectStart.x + _rectSize.x, _rectStart.y + _rectSize.y), 3.0f);
    }

    private void DrawAxes(Vector3 _start, float _axisLength)
    {
        //draw y-axis
        DrawVector(_start, Vector3.up * _axisLength, Color.green, 3.0f);

        //draw x-axis
        DrawVector(_start, Vector3.right * _axisLength, Color.red, 3.0f);
    }

    private void OnDrawGizmos()
    {
        //draw axes to origin
        DrawAxes(Vector3.zero, originAxisLength);

        //draw circle
        Handles.color = Color.white;
        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);

        //draw tracking line
        DrawVector(Vector3.zero, trackedPoint, Color.cyan, 3.0f);
        DrawAxes(trackedPoint, axisLength);

        //draw rectangle
        DrawRect(rectStart, rectSize, Color.black);
        DrawAxes(rectStart, axisLength);

        //moikka, voisitko laittaa ens kerralla tehtävänantoonkin, että tää oli tarkotus
        //tehä "kehyksen" sisään, eikä oikean näytön koon mukaan... melkeen kuolin oikeesti :d
        //draw fake "screen" frame
        Vector2 screenDrawSize = screenSize * 0.01f;
        DrawRect(screenPos, screenDrawSize, Color.black);

        //draw popup
        Vector2 popupSize = new(screenDrawSize.x * popupWidthPercent, screenDrawSize.y * popupHeightPercent);
        Vector2 popupStart = new(screenPos.x + screenDrawSize.x * 0.5f - popupSize.x * 0.5f, screenPos.y + screenDrawSize.y * 0.5f - popupSize.y * 0.5f);
        DrawRect(popupStart, popupSize, Color.red);
    }
}