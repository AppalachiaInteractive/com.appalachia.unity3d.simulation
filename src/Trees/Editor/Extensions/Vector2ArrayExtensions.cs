using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class Vector2ArrayExtensions
    {
        public static Vector2[] ConvexHull(this Vector2[] P)
        {
            if (P.Length > 1)
            {
                int n = P.Length, k = 0;
                Vector2[] H = new Vector2[2 * n];

                Comparison<Vector2> comparison = new Comparison<Vector2>((a, b) =>
                {
                    if (Math.Abs(a.x - b.x) < float.Epsilon)
                        return a.y.CompareTo(b.y);
                    else
                        return a.x.CompareTo(b.x);
                });
                Array.Sort<Vector2>(P, comparison);

                // Build lower hull
                for (int i = 0; i < n; ++i)
                {
                    while ((k >= 2) && (P[i].Cross(H[k - 2], H[k - 1]) <= 0))
                        k--;
                    H[k++] = P[i];
                }

                // Build upper hull
                for (int i = n - 2, t = k + 1; i >= 0; i--)
                {
                    while ((k >= t) && (P[i].Cross(H[k - 2], H[k - 1]) <= 0))
                        k--;
                    H[k++] = P[i];
                }

                if (k > 1)
                    Array.Resize<Vector2>(ref H, k - 1);

                return H;
            }
            else if (P.Length <= 1)
            {
                return P;
            }
            else
            {
                return null;
            }
        }

        public static Vector2[] ScaleAlongNormals(this Vector2[] P, float scaleAmount)
        {
            Vector2[] normals = new Vector2[P.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                int prev = i - 1;
                int next = i + 1;
                if (i == 0)
                    prev = P.Length - 1;
                if (i == (P.Length - 1))
                    next = 0;

                Vector2 ba = P[i] - P[prev];
                Vector2 bc = P[i] - P[next];
                Vector2 normal = (ba.normalized + bc.normalized).normalized;
                normals[i] = normal;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                P[i] = P[i] + (normals[i] * scaleAmount);
            }

            return P;
        }

        static Vector2[] ReduceLeastSignificantVertice(Vector2[] P)
        {
            float currentArea = 0;
            int smallestIndex = 0;
            int replacementIndex = 0;
            Vector2 newPos = Vector2.zero;
            for (int i = 0; i < P.Length; i++)
            {
                int next = i + 1;
                int upNext = i + 2;
                int finalNext = i + 3;
                if (next >= P.Length)
                    next -= P.Length;
                if (upNext >= P.Length)
                    upNext -= P.Length;
                if (finalNext >= P.Length)
                    finalNext -= P.Length;

                Vector2 intersect = GetIntersectionPointCoordinates(P[i], P[next], P[upNext], P[finalNext]);
                if (i == 0)
                {
                    currentArea = intersect.TriangleArea(P[next], P[upNext]);

                    if (OutsideBounds(intersect) > 0)
                        currentArea = currentArea + (OutsideBounds(intersect) * 1);

                    smallestIndex = next;
                    replacementIndex = upNext;
                    newPos = intersect;
                }
                else
                {
                    float newArea = intersect.TriangleArea(P[next], P[upNext]);

                    if (OutsideBounds(intersect) > 0)
                        newArea = newArea + (OutsideBounds(intersect) * 1);

                    if ((newArea < currentArea) && (OutsideBounds(intersect) <= 0))
                    {
                        currentArea = newArea;
                        smallestIndex = next;
                        replacementIndex = upNext;
                        newPos = intersect;
                    }
                }
            }

            P[replacementIndex] = newPos;
            return Array.FindAll<Vector2>(P, x => Array.IndexOf(P, x) != smallestIndex);
        }


        public static Vector2[] ReduceVertices(this Vector2[] P, int maxVertices)
        {
            if (maxVertices == 4)
            {
                // turn into a box
                Rect newBox = new Rect(P[0].x, P[0].y, 0f, 0f);
                for (int i = 0; i < P.Length; i++)
                {
                    newBox.xMin = Mathf.Min(newBox.xMin, P[i].x);
                    newBox.xMax = Mathf.Max(newBox.xMax, P[i].x);
                    newBox.yMin = Mathf.Min(newBox.yMin, P[i].y);
                    newBox.yMax = Mathf.Max(newBox.yMax, P[i].y);
                }

                P = new Vector2[]
                {
                    new Vector2(newBox.xMin, newBox.yMin),
                    new Vector2(newBox.xMax, newBox.yMin),
                    new Vector2(newBox.xMax, newBox.yMax),
                    new Vector2(newBox.xMin, newBox.yMax),
                };
            }
            else
            {
                // remove vertices to target count (naive implementation)
                int reduction = System.Math.Max(0, P.Length - maxVertices);
                for (int k = 0; k < reduction; k++)
                {
                    P = ReduceLeastSignificantVertice(P);
                }
            }

            return P;
        }

        static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
        {
            float tmp = ((B2.x - B1.x) * (A2.y - A1.y)) - ((B2.y - B1.y) * (A2.x - A1.x));

            if (tmp == 0)
            {
                return ((Vector2.Lerp(A2, B1, 0.5f) - (Vector2.one * 0.5f)) * 1000) +
                    (Vector2.one * 500f); //Vector2.positiveInfinity;// Vector2.zero;
            }

            float mu = (((A1.x - B1.x) * (A2.y - A1.y)) - ((A1.y - B1.y) * (A2.x - A1.x))) / tmp;

            return new Vector2(
                B1.x + ((B2.x - B1.x) * mu),
                B1.y + ((B2.y - B1.y) * mu)
            );
        }

        static float OutsideBounds(Vector2 P)
        {
            P = P - (Vector2.one * 0.5f);
            float vert = Mathf.Clamp01(Mathf.Abs(P.y) - 0.5f);
            float hori = Mathf.Clamp01(Mathf.Abs(P.x) - 0.5f);
            return hori + vert;
        }
    }
}