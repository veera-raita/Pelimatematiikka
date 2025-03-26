using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/*
namespace teemun_koodi
{
    public class BezierRoad : MonoBehaviour
    {

        public GameObject[] points;

        // 2D cross-section of the road
        public Mesh2D CrossSection;


        [Range(0f, 1f)]
        public float t = 0.0f;

        [Range(10, 1000)]
        public int RoadSegments = 100;

        [Range(0.1f, 10f)]
        public float RoadScaler = 1.0f;

        public bool DrawCrossSectionLines = true;
        public bool DrawVertices = true;
        public bool DrawConnectingLines = true;

        public bool ClosedLoop = false;

        // Our mesh
        private Mesh mesh;

        private int GetBezSegment(float t)
        {
            // Number of bezier segments
            int bez_segments = points.Length - 1;

            float seg = t * bez_segments;

            //Debug.Log(Mathf.FloorToInt(seg));
            return Mathf.FloorToInt(seg);
        }

        private float AdjustTValue(float t, int bez_segment)
        {
            // Number of bezier segments
            int bez_segments = points.Length - 1;

            return (t - ((float)bez_segment / (float)bez_segments)) / (1.0f / (float)bez_segments);
        }

        private Vector3 getBezierPoint(int bezier_segment, float t)
        {
            // Get the points for THIS SEGMENT!
            Vector3 A = points[bezier_segment].GetComponent<BezierPoint>().getAnchor();
            Vector3 B = points[bezier_segment].GetComponent<BezierPoint>().getControl2();
            Vector3 C = points[bezier_segment + 1].GetComponent<BezierPoint>().getControl1();
            Vector3 D = points[bezier_segment + 1].GetComponent<BezierPoint>().getAnchor();

            // Interpolation, 1st stage
            Vector3 X = (1 - t) * A + t * B;
            Vector3 Y = (1 - t) * B + t * C;
            Vector3 Z = (1 - t) * C + t * D;


            // Interpolation, 2nd stage
            Vector3 P = (1 - t) * X + t * Y;
            Vector3 Q = (1 - t) * Y + t * Z;

            return (1 - t) * P + t * Q;
        }


        private Vector3 getBezierPoint(Vector3 A, Vector3 B,
                                       Vector3 C, Vector3 D,
                                       float t)
        {
            // Interpolation, 1st stage
            Vector3 X = (1 - t) * A + t * B;
            Vector3 Y = (1 - t) * B + t * C;
            Vector3 Z = (1 - t) * C + t * D;


            // Interpolation, 2nd stage
            Vector3 P = (1 - t) * X + t * Y;
            Vector3 Q = (1 - t) * Y + t * Z;

            return (1 - t) * P + t * Q;
        }

        Vector3 getBezierForwardVector(int bezier_segment,
                                       float t)
        {
            // Get the points for THIS SEGMENT!
            Vector3 A = points[bezier_segment].GetComponent<BezierPoint>().getAnchor();
            Vector3 B = points[bezier_segment].GetComponent<BezierPoint>().getControl2();
            Vector3 C = points[bezier_segment + 1].GetComponent<BezierPoint>().getControl1();
            Vector3 D = points[bezier_segment + 1].GetComponent<BezierPoint>().getAnchor();

            // Interpolation, 1st stage
            Vector3 X = (1 - t) * A + t * B;
            Vector3 Y = (1 - t) * B + t * C;
            Vector3 Z = (1 - t) * C + t * D;


            // Interpolation, 2nd stage
            Vector3 P = (1 - t) * X + t * Y;
            Vector3 Q = (1 - t) * Y + t * Z;

            return (Q - P).normalized;
        }

        Vector3 getBezierForwardVector(Vector3 A, Vector3 B,
                                   Vector3 C, Vector3 D,
                                   float t)
        {
            // Interpolation, 1st stage
            Vector3 X = (1 - t) * A + t * B;
            Vector3 Y = (1 - t) * B + t * C;
            Vector3 Z = (1 - t) * C + t * D;


            // Interpolation, 2nd stage
            Vector3 P = (1 - t) * X + t * Y;
            Vector3 Q = (1 - t) * Y + t * Z;

            return (Q - P).normalized;
        }

        void GenerateRoadMesh()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "Road Mesh";
            }
            else
            {
                mesh.Clear();
            }

            // Vertices
            List<Vector3> vertices = new List<Vector3>();

            // Triangles
            List<int> triangles = new List<int>();

            // ---------------------------------------------------
            // Go through the whole road and generate the vertices
            // ---------------------------------------------------

            // Number of bezier segments
            int bez_segments = points.Length - 1;

            // 
            // Current T-value for the road segment
            //
            for (int road_seg = 0; road_seg < RoadSegments; road_seg++)
            {
                // Compute the T-value for current segment and next segment
                float t_for_curr_seg = road_seg / (float)RoadSegments;
                // Correct bezier segment
                int bez_segment = GetBezSegment(t_for_curr_seg);
                // Double check this...
                if (bez_segment == bez_segments) // t = 1.0 --> segment should be segment-1
                {
                    Debug.Log("Adjusted segment!");
                    bez_segment--;
                }

                // The value of t at current segment
                float myTValue = AdjustTValue(t_for_curr_seg, bez_segment);

                // Current point on the Bezier curve
                Vector3 BezPoint = getBezierPoint(bez_segment, myTValue);

                // Get the forward vector current bezier point
                Vector3 forw = getBezierForwardVector(bez_segment, myTValue);
                // Use vector3.up to get the "right" vector
                Vector3 right = Vector3.Cross(Vector3.up, forw);
                // Use the forward vector and "right" vector to get the correct "up"-vector
                Vector3 up = Vector3.Cross(forw, right);

                // Compute the vertices for the cross section at this point
                for (int i = 0; i < CrossSection.vertices.Length; i++)
                {
                    // 2D-point x-coord times right-vector + y-coord times up-vector
                    Vector3 point = CrossSection.vertices[i].point.x * right +
                        CrossSection.vertices[i].point.y * up;

                    // Scale the "2D" point 
                    point *= RoadScaler;

                    // Add the Bezier point to get the vertex
                    point += BezPoint;  // We might have to subtract the transform.position from this...

                    // Add the vertex to our list
                    vertices.Add(point);
                }
            }

            // ---------------------------------------------------
            // Go through the vertices and generate triangles
            // ---------------------------------------------------
            for (int road_seg = 0; road_seg < RoadSegments - 1; road_seg++)
            {
                int baseIndex = road_seg * CrossSection.vertices.Length;
                int LowerLeft, LowerRight, UpperLeft, UpperRight;

                // Loop through every other vertex
                for (int i = 1; i < CrossSection.vertices.Length - 1; i += 2)
                {
                    LowerLeft = i + baseIndex;
                    LowerRight = LowerLeft + 1;

                    UpperLeft = LowerLeft + CrossSection.vertices.Length;
                    UpperRight = UpperLeft + 1;

                    // 1st triangle
                    triangles.Add(LowerLeft);
                    triangles.Add(UpperLeft);
                    triangles.Add(UpperRight);

                    // 2nd triangle
                    triangles.Add(LowerLeft);
                    triangles.Add(UpperRight);
                    triangles.Add(LowerRight);
                }
                // Handle the last bit of the cross-sections here
                LowerLeft = baseIndex + CrossSection.vertices.Length - 1;
                LowerRight = baseIndex;

                UpperLeft = LowerLeft + CrossSection.vertices.Length;
                UpperRight = LowerRight + CrossSection.vertices.Length;
                // 1st triangle
                triangles.Add(LowerLeft);
                triangles.Add(UpperLeft);
                triangles.Add(UpperRight);

                // 2nd triangle
                triangles.Add(LowerLeft);
                triangles.Add(UpperRight);
                triangles.Add(LowerRight);

            }


            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

        }

        private void OnDrawGizmos()
        {
            GenerateRoadMesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;

            // Number of bezier segments
            int bez_segments = points.Length - 1;

            // Draw the bezier segments
            for (int i = 0; i < bez_segments; i++)
            {
                Vector3 A = points[i].GetComponent<BezierPoint>().getAnchor();
                Vector3 B = points[i].GetComponent<BezierPoint>().getControl2();
                Vector3 C = points[i + 1].GetComponent<BezierPoint>().getControl1();
                Vector3 D = points[i + 1].GetComponent<BezierPoint>().getAnchor();

                Handles.DrawBezier(A, D, B, C, Color.white, null, 3f);
            }


            // 
            // Current T-value for the road segment
            //
            for (int road_seg = 0; road_seg < RoadSegments; road_seg++)
            {
                // Compute the T-value for current segment and next segment
                float t_for_curr_seg = road_seg / (float)RoadSegments;
                float t_for_next_seg = (road_seg + 1) / (float)RoadSegments;

                int bez_segment = GetBezSegment(t_for_curr_seg);
                int bez_next_segment = GetBezSegment(t_for_next_seg);

                if (bez_segment == bez_segments) // t = 1.0 --> segment should be segment-1
                {
                    Debug.Log("Adjusted segment! Previous value:" + bez_segment);
                    bez_segment--;
                }
                if (bez_next_segment == bez_segments) // t = 1.0 --> segment should be segment-1
                {
                    Debug.Log("Adjusted next segment! Previous value:" + bez_next_segment);
                    bez_next_segment--;
                }

                // The value of t at current segment and next segments
                float myTValue = AdjustTValue(t_for_curr_seg, bez_segment);
                float myTValueNext = AdjustTValue(t_for_next_seg, bez_next_segment);

                // Current point on the Bezier curve
                Vector3 BezPoint = getBezierPoint(bez_segment, myTValue);
                // Next point on the Bezier curve
                Vector3 BezNextPoint = getBezierPoint(bez_next_segment, myTValueNext);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(BezPoint, 0.3f);

                // Get the forward vector current bezier point
                Vector3 forw = getBezierForwardVector(bez_segment, myTValue);
                // Get the forward vector next bezier point
                Vector3 forw_next = getBezierForwardVector(bez_next_segment, myTValueNext);

                // Use vector3.up to get the "right" vector
                Vector3 right = Vector3.Cross(Vector3.up, forw);
                // Use vector3.up to get the "right" vector
                Vector3 right_next = Vector3.Cross(Vector3.up, forw_next);

                // Use the forward vector and "right" vector to get the correct "up"-vector
                Vector3 up = Vector3.Cross(forw, right);
                // Use the forward vector and "right" vector to get the correct "up"-vector
                Vector3 up_next = Vector3.Cross(forw, right_next);

                // ------------------------------------
                // Draw the points using the cross section of the road
                // ------------------------------------
                for (int i = 0; i < CrossSection.vertices.Length - 1; i++)
                {
                    // 2D-point x-coord times right-vector + y-coord times up-vector
                    Vector3 point = CrossSection.vertices[i].point.x * right +
                        CrossSection.vertices[i].point.y * up;

                    // Scale the "2D" point 
                    point *= RoadScaler;

                    // Add the bezier point to the above
                    point += BezPoint;

                    if (DrawConnectingLines)
                    {
                        // 2D-point x-coord times right-vector + y-coord times up-vector
                        Vector3 pointNext = CrossSection.vertices[i].point.x * right_next +
                            CrossSection.vertices[i].point.y * up_next;

                        // Scale the "2D" point 
                        pointNext *= RoadScaler;

                        // Add the bezier point to the above
                        pointNext += BezNextPoint;

                        Gizmos.DrawLine(point, pointNext);
                    }


                    // Compute the next point, too
                    Vector3 nextpoint = CrossSection.vertices[i + 1].point.x * right +
                        CrossSection.vertices[i + 1].point.y * up;

                    // Scale the "2D" point 
                    nextpoint *= RoadScaler;

                    // Add the bezier point to the above
                    nextpoint += BezPoint;


                    // Draw the point
                    Gizmos.color = Color.white;
                    if (DrawVertices)
                        Gizmos.DrawSphere(point, 0.4f);

                    // Draw the cross section lines
                    if (DrawCrossSectionLines)
                        Gizmos.DrawLine(point, nextpoint);

                    // If we are at the second to last cross section vertex, also
                    // draw the next point
                    if (i == CrossSection.vertices.Length - 2)
                    {
                        if (DrawVertices)
                            Gizmos.DrawSphere(nextpoint, 0.4f);

                        if (DrawCrossSectionLines)
                        {
                            point = CrossSection.vertices[0].point.x * right +
                                CrossSection.vertices[0].point.y * up;

                            // Scale the "2D" point 
                            point *= RoadScaler;

                            // Add the bezier point to the above
                            point += BezPoint;

                            // Draw the line from 0th to the last (??)
                            Gizmos.DrawLine(point, nextpoint);

                        }


                    }
                }

            }
            // The final segment???
            // Current point on the Bezier curve
            Vector3 FinalPoint = getBezierPoint(bez_segments - 1, 1.0f);
            Vector3 FinalForward = getBezierForwardVector(bez_segments - 1, 1.0f);
            // Use vector3.up to get the "right" vector
            Vector3 FinalRight = Vector3.Cross(Vector3.up, FinalForward);
            // Use the forward vector and "right" vector to get the correct "up"-vector
            Vector3 FinalUp = Vector3.Cross(FinalForward, FinalRight);

            for (int i = 0; i < CrossSection.vertices.Length - 1; i++)
            {
                // 2D-point x-coord times right-vector + y-coord times up-vector
                Vector3 point = CrossSection.vertices[i].point.x * FinalRight +
                    CrossSection.vertices[i].point.y * FinalUp;

                // Scale the "2D" point 
                point *= RoadScaler;

                // Add the bezier point to the above
                point += FinalPoint;


                // Compute the next point, too
                Vector3 nextpoint = CrossSection.vertices[i + 1].point.x * FinalRight +
                    CrossSection.vertices[i + 1].point.y * FinalUp;

                // Scale the "2D" point 
                nextpoint *= RoadScaler;

                // Add the bezier point to the above
                nextpoint += FinalPoint;


                // Draw the point
                Gizmos.color = Color.white;
                if (DrawVertices)
                    Gizmos.DrawSphere(point, 0.4f);

                // Draw the cross section lines
                if (DrawCrossSectionLines)
                    Gizmos.DrawLine(point, nextpoint);

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GenerateRoadMesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;

        }
    }
}*/