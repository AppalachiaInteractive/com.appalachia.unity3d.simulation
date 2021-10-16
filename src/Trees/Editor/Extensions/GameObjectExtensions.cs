using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class GameObjectExtensions
    {
        public static T[] GetComponentsInChildren<T>(this GameObject c)
            where T : Component
        {
            return c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>()).Distinct().ToArray();
        }

        public static T GetComponentHereOrInChildren<T>(this GameObject c)
            where T : Component
        {
            return c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>()).First();
        }
    }
}