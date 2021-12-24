using System;
using System.Collections.Generic;
using Appalachia.Utility.Strings;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class EnumExtensions
    {
        private static Dictionary<Type, Array> _enumLookup = new Dictionary<Type,Array>();
        
        public static T Next<T>(this T src) where T : Enum
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException(ZString.Format("Argument {0} is not an Enum", type.FullName));
            }

            if (_enumLookup == null)
            {
                _enumLookup = new Dictionary<Type, Array>();
            }

            if (!_enumLookup.ContainsKey(type))
            {
                var array = Enum.GetValues(src.GetType());

                _enumLookup.Add(type, array);  
            }

            var result = _enumLookup[type] as T[];
            
            var j = Array.IndexOf<T>(result, src) + 1;
                
            return (result.Length==j) ? result[0] : result[j];   
        }
    }
}
