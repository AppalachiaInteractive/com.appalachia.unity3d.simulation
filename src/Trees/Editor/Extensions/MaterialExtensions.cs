using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class MaterialExtensions
    {
        static MaterialExtensions()
        {
            successfulHits = new Dictionary<Shader, string>();            
        }
        
        private static string[] allowances = new[]
        {
            "albe","diff","basec"
        };

        private static string[] primaryExclusions = new[]
        {
            "detail", "_maintex", "normal", "bump", "metal", "surface", "occ", "ambie", "base", "alt","vari"
        };

        private static string[] secondaryExclusions = new[]
        {
            "detail", "_maintex", "normal", "bump", "metal", "surface", "occ", "ambie"
        };

        private static Dictionary<Shader, string> successfulHits;

        public static Texture2D primaryTexture(this Material material)
        {
            if (successfulHits.ContainsKey(material.shader))
            {
                var h = successfulHits[material.shader];

                if (material.HasProperty(h))
                {
                    var r = material.GetTexture(h) as Texture2D;

                    return r ? r : material.mainTexture as Texture2D;
                }
                else
                {
                    successfulHits.Remove(material.shader);
                }
            }
            
            var props = material.GetTexturePropertyNames();
            var hit = props.FirstOrDefault(p =>
            {
                var pL = p.ToLowerInvariant();

                foreach (var allowance in allowances)
                {
                    if (pL.Contains(allowance))
                    {
                        foreach (var exclusion in primaryExclusions)
                        {
                            if (pL.Contains(exclusion))
                            {
                                return false;
                            }
                        }
                        
                        return true;
                    }
                }

                return false;
            });

            if (string.IsNullOrWhiteSpace(hit))
            {
                return material.mainTexture as Texture2D;
            }
            
            hit = props.FirstOrDefault(p =>
            {
                var pL = p.ToLowerInvariant();

                foreach (var allowance in allowances)
                {
                    if (pL.Contains(allowance))
                    {
                        foreach (var exclusion in secondaryExclusions)
                        {
                            if (pL.Contains(exclusion))
                            {
                                return false;
                            }
                        }
                        
                        return true;
                    }
                }

                return false;
            });
            
            if (string.IsNullOrWhiteSpace(hit))
            {
                return material.mainTexture as Texture2D;
            }

            var result = material.GetTexture(hit) as Texture2D;

            successfulHits.Add(material.shader, hit);

            return result ? result : material.mainTexture as Texture2D;
        }

        public static void SetFloatIfNecessary(
            this Material material,
            string nameID,
            float value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetFloat(nameID, value);
            }
        }

        public static void SetFloatArrayIfNecessary(
            this Material material,
            string nameID,
            float[] value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetFloatArray(nameID, value);
            }
        }

        public static void SetIntIfNecessary(
            this Material material,
            string nameID,
            int value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetInt(nameID, value);
            }
        }

        public static void SetVectorIfNecessary(
            this Material material,
            string nameID,
            Vector4 value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetVector(nameID, value);
            }
        }

        public static void SetVectorArrayIfNecessary(
            this Material material,
            string nameID,
            Vector4[] value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetVectorArray(nameID, value);
            }
        }

        public static void SetColorIfNecessary(
            this Material material,
            string nameID,
            Color value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetColor(nameID, value);
            }
        }

        public static void SetColorArrayIfNecessary(
            this Material material,
            string nameID,
            Color[] value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetColorArray(nameID, value);
            }
        }

        public static void SetMatrixIfNecessary(
            this Material material,
            string nameID,
            Matrix4x4 value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetMatrix(nameID, value);
            }
        }

        public static void SetMatrixArrayIfNecessary(
            this Material material,
            string nameID,
            Matrix4x4[] value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetMatrixArray(nameID, value);
            }
        }

        public static void SetTextureIfNecessary(
            this Material material,
            string nameID,
            Texture2D value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetTexture(nameID, value);
            }
        }

        public static void SetTextureArrayIfNecessary(
            this Material material,
            string nameID,
            Texture2DArray value)
        {
            if (material.HasProperty(nameID))
            {
                material.SetTexture(nameID, value);
            }
        }
    }
}
