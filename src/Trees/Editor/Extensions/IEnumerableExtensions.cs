using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> FilterCast<TResult>(this IEnumerable values)
        where TResult : class
        {
            foreach (var value in values)
            {
                var cast = value as TResult;

                if (cast == null)
                {
                    continue;
                }

                yield return cast;
            }
        }

        public static Vector2 Sum(this IEnumerable<Vector2> vectors)
        {
            var s = Vector2.zero;

            foreach (var vector in vectors)
            {
                s += vector;
            }

            return s;
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            var s = Vector2.zero;
            var c = 0;

            foreach (var vector in vectors)
            {
                s += vector;
                c += 1;
            }

            return s / (c == 0 ? 1 : c);
        }

        public static Vector2 Average(this IList<Vector2> vectors, int start, int end)
        {
            var s = Vector2.zero;

            for(var i = start; i < end; i++)
            {
                s += vectors[i];
            }

            var c = end - start;
            return s / (c == 0 ? 1 : c);
        }
        
        public static Vector3 Sum(this IEnumerable<Vector3> vectors)
        {
            
            var s = Vector3.zero;

            foreach (var vector in vectors)
            {
                s += vector;
            }

            return s;
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            var s = Vector3.zero;
            var c = 0;

            foreach (var vector in vectors)
            {
                s += vector;
                c += 1;
            }

            return s / (c == 0 ? 1 : c);
        }
                
        public static Vector3 Average(this IList<Vector3> vectors, int start, int end)
        {
            var s = Vector3.zero;

            for(var i = start; i < end; i++)
            {
                s += vectors[i];
            }

            var c = end - start;
            return s / (c == 0 ? 1 : c);
        }
        
        public static Vector4 Sum(this IEnumerable<Vector4> vectors)
        {
            
            var s = Vector4.zero;

            foreach (var vector in vectors)
            {
                s += vector;
            }

            return s;
        }

        public static Vector4 Average(this IEnumerable<Vector4> vectors)
        {
            var s = Vector4.zero;
            var c = 0;

            foreach (var vector in vectors)
            {
                s += vector;
                c += 1;
            }

            return s / (c == 0 ? 1 : c);
        }
        
        public static Vector4 Average(this IList<Vector4> vectors, int start, int end)
        {
            var s = Vector4.zero;

            for(var i = start; i < end; i++)
            {
                s += vectors[i];
            }

            var c = end - start;
            return s / (c == 0 ? 1 : c);
        }
    }
}