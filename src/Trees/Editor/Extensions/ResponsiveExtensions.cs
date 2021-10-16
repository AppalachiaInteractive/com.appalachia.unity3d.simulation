using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Appalachia.Simulation.Trees.Core.Settings;
using Unity.Profiling;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class ResponsiveExtensions
    {
        [NonSerialized] private static Dictionary<Type, FieldInfo[]> _responsiveFieldsByType;
        [NonSerialized] private static Dictionary<Type, FieldInfo[]> _enumerableFieldsByType;
        [NonSerialized] private static Dictionary<FieldInfo, MethodInfo> _responsiveMethodsByField;

        
        private const string _PRF_PFX = nameof(ResponsiveExtensions) + ".";

        private static readonly ProfilerMarker _PRF_HandleResponsiveUpdate = new ProfilerMarker(_PRF_PFX + nameof(HandleResponsiveUpdate));
        public static void HandleResponsiveUpdate(this IResponsive responsive, ResponsiveSettingsType t)
        {
            using (_PRF_HandleResponsiveUpdate.Auto())
            {
                var type = responsive.GetType();
                var respType = typeof(IResponsive);

                var responsiveFields = GetTypeFieldsAssignableTo<IResponsive>(type, ref _responsiveFieldsByType);
                var enumerableFields = GetTypeFieldsAssignableTo<IEnumerable>(type, ref _enumerableFieldsByType);
                
                for (var index = 0; index < responsiveFields.Length; index++)
                {
                    var field = responsiveFields[index];
                    var method = field.FieldType.GetMethod("UpdateSettingsType", BindingFlags.Public | BindingFlags.Instance);

                    var instance = field.GetValue(responsive);

                    if (instance != null)
                    {
                        method?.Invoke(instance, new object[] {t});
                    }
                }

                for (var index = 0; index < enumerableFields.Length; index++)
                {
                    var field = enumerableFields[index];
                    
                    Type elementType = null;
                    
                    var listType = field.FieldType;
                    if (listType.HasElementType)
                    {
                        elementType = listType.GetElementType();
                    }
                    else
                    {
                        var gen = listType.GetGenericArguments();

                        if (gen.Length > 0)
                        {
                            elementType = gen[0];
                        }
                    }

                    if (elementType != null)
                    {
                        if (respType.IsAssignableFrom(elementType))
                        {
                            var listInstance = field.GetValue(responsive) as IEnumerable;

                            foreach (var instance in listInstance.FilterCast<IResponsive>())
                            {
                                instance.UpdateSettingsType(t);
                            }
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker _PRF_GetTypeFieldsAssignableTo = new ProfilerMarker(_PRF_PFX + nameof(GetTypeFieldsAssignableTo));
        private static FieldInfo[] GetTypeFieldsAssignableTo<T>(Type type, ref Dictionary<Type, FieldInfo[]> lookup)
        {
            using (_PRF_GetTypeFieldsAssignableTo.Auto())
            {
                FieldInfo[] fields;

                if (lookup == null)
                {
                    lookup = new Dictionary<Type, FieldInfo[]>();
                }
            
                var assignType = typeof(T);
                
                if (!lookup.ContainsKey(type))
                {
                    var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    fields = fieldInfos.Where(f => assignType.IsAssignableFrom(f.FieldType)).ToArray();

                    lookup.Add(type, fields);
                }
                else
                {
                    fields = lookup[type];
                }

                return fields;
            }
        }
    }
}
