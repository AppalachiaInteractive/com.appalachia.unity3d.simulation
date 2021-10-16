#region

using System;
using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    [Serializable]
    public struct TreeVertex : /*TreePoolObject<TreeVertex>,*/ IEquatable<TreeVertex>
    {
        public TreeVertex(bool initialize = true)
        {
            initialized = true;
            visible = true;
            billboard = false;
            uvScaleUpdated = false;
            tangent = new Vector4(1, 0, 0, 1);
            visible = false;
            billboard = false;
            uvScaleUpdated = false;
            tangent = Vector4.zero;
            ambientOcclusion = 0;
            variation = 0;
            noise = 0;
            hierarchyID = 0;
            shapeID = 0;
            parentOffset = 0;
            heightOffset = 0;
            matrix = Matrix4x4.zero;
            normal = Vector3.forward;
            position = Vector3.zero;
            weldTo = -1;
            splineBreak = false;
            type = 0;
            raw_uv0 = Vector2.zero;
            rect_uv0 = Vector2.zero;
            uvScale = Vector2.one;
            uvOffset = Vector2.one;
            uvRect = Rect.zero;
            uvScale = Vector2.one;
            uvOffset = Vector2.one;
            uvRect = Rect.zero;
            rawWind = default;
            wind = default;
            log = default;
            billboardData = default;
        }

        public bool initialized;
        public bool visible;
        public bool billboard;
        public bool uvScaleUpdated;
        public Vector4 tangent;
        public float ambientOcclusion;
        public float variation;
        public float noise;
        public int hierarchyID;
        public int shapeID;
        public float parentOffset;
        public float heightOffset;
        public Matrix4x4 matrix;
        public Vector3 normal;
        public Vector3 position;
        public int weldTo;
        public bool splineBreak;
        public TreeComponentType type;
        public Vector2 raw_uv0;
        public Vector2 rect_uv0;
        public Vector2 uvScale;
        public Vector2 uvOffset;
        public Rect uvRect;
        public WindVector rawWind;
        public WindVector wind;
        public LogVector log;
        public Vector4 billboardData;
        
        public Vector4 UV0
        {
            get
            {
                var ao = ambientOcclusion;
                
                if (noise > 0f)
                {
                    ao = ((ambientOcclusion) + (1 - Mathf.Clamp01(noise))) / 2.0f;
                }

                if (wind.tertiaryRoll > 0f)
                {
                    ao = (ao + heightOffset) / 1.5f;
                }
                
                if (uvRect == Rect.zero)
                {
                    return new Vector4(
                        uvOffset.x + (uvScale.x * rect_uv0.x),
                        uvOffset.y + (uvScale.y * rect_uv0.y),
                        Mathf.Clamp01(ao),
                        Mathf.Clamp01(variation)
                    );
                }

                var uv = rect_uv0;
                uv.x = uvRect.x + (rect_uv0.x * uvRect.width);
                uv.y = uvRect.y + (rect_uv0.y * uvRect.height);

                return new Vector4(
                    uv.x,
                    uv.y,
                    Mathf.Clamp01(ao),
                Mathf.Clamp01(variation));
            }
        }

        

        public Color TreeVertexColor =>
            new Color(
                Mathf.Clamp01(wind.primaryRoll),
                Mathf.Clamp01(wind.secondaryRoll),
                Mathf.Clamp01(wind.tertiaryRoll),
                Mathf.Clamp01(wind.phase)
            );

        public Color TreeVertexColorUnclamped =>
            new Color(
                (wind.primaryRoll),
                (wind.secondaryRoll),
                (wind.tertiaryRoll),
                (wind.phase)
            );
        
        public Color LogColor =>
            new Color(
                Mathf.Clamp01(log.bark),
                Mathf.Clamp01(log.woodDarkening),
                Mathf.Clamp01(log.noise1),
                Mathf.Clamp01(log.noise2)
            );
        
        
                
        /*
        PrimaryRoll = 10,           // VERTEX R
        PrimaryPivot = 10,          // UV2 XYZ
        PrimaryBend = 22,           // UV2 W
        
        SecondaryRoll = 20,         // VERTEX G
        SecondaryPivot = 21,        // UV4 XYZ
        SecondaryBend = 22,         // UV4 W
        SecondaryDirection = 23,    // UV3 XYZ
        
        Tertiary = 30,              // VERTEX B
        
        TypeDesignator = 40,  // UV3 W
        

        GustStrength = 90,
        GustDirectionality = 91,
        

        AmbientOcclusion = 97,       // UV0 Z
        Phase = 98,                  // UV0 W
        Variation = 99,              // VERTEX A
        */

        public Vector4 UV2 =>
            new Vector4(
                wind.primaryPivot.x,
                wind.primaryPivot.y,
                wind.primaryPivot.z,
                wind.primaryBend
            );
        
        
        public Vector4 UV3 =>
            new Vector4(
                wind.secondaryForward.x,
                wind.secondaryForward.y,
                wind.secondaryForward.z,
                (splineBreak ? 10 : (int)type) 
            );
        
        
        public Vector4 UV4 =>
            new Vector4(
                wind.secondaryPivot.x,
                wind.secondaryPivot.y,
                wind.secondaryPivot.z,
                wind.secondaryBend
            );
        
        
        public Vector4 UV5 =>
            new Color(
                billboardData.x,
                billboardData.y,
                billboardData.z,
                billboard ? -2 : 2
            );


        public bool Equals(TreeVertex other)
        {
            return (initialized == other.initialized) && (visible == other.visible) && (billboard == other.billboard) && (uvScaleUpdated == other.uvScaleUpdated) && tangent.Equals(other.tangent) && ambientOcclusion.Equals(other.ambientOcclusion) && variation.Equals(other.variation) && noise.Equals(other.noise) && (hierarchyID == other.hierarchyID) && (shapeID == other.shapeID) && parentOffset.Equals(other.parentOffset) && heightOffset.Equals(other.heightOffset) && matrix.Equals(other.matrix) && normal.Equals(other.normal) && position.Equals(other.position) && (weldTo == other.weldTo) && (splineBreak == other.splineBreak) && (type == other.type) && raw_uv0.Equals(other.raw_uv0) && rect_uv0.Equals(other.rect_uv0) && uvScale.Equals(other.uvScale) && uvOffset.Equals(other.uvOffset) && uvRect.Equals(other.uvRect) && rawWind.Equals(other.rawWind) && wind.Equals(other.wind) && log.Equals(other.log) && billboardData.Equals(other.billboardData);
        }

        public void LerpVerticesByFactor(TreeVertex[] tv, Vector2 factor)
        {
            position = Vector3.Lerp(
                Vector3.Lerp(tv[1].position, tv[2].position, factor.x),
                Vector3.Lerp(tv[0].position, tv[3].position, factor.x),
                factor.y
            );
            normal = Vector3.Lerp(
                    Vector3.Lerp(tv[1].normal, tv[2].normal, factor.x),
                    Vector3.Lerp(tv[0].normal, tv[3].normal, factor.x),
                    factor.y
                )
                .normalized;

            tangent = Vector4.Lerp(
                Vector4.Lerp(tv[1].tangent, tv[2].tangent, factor.x),
                Vector4.Lerp(tv[0].tangent, tv[3].tangent, factor.x),
                factor.y
            );
            var tangentNormalized = new Vector3(tangent.x, tangent.y, tangent.z);

            tangentNormalized.Normalize();
            
            tangent.x = tangentNormalized.x;
            tangent.y = tangentNormalized.y;
            tangent.z = tangentNormalized.z;
        }

        public void SetAmbientOcclusion(float ao)
        {
            ambientOcclusion = ao;
        }

        public void SetDynamicVariationValue(float v)
        {
            variation = v;
        }

        public void Set(ShapeData shape, Vector3 pos, Vector3 nor, float offset)
        {
            position = pos;
            normal = nor;
            Set(shape, offset);
        }

        public void Set(ShapeData shape, float offset)
        {
            visible = shape.exportGeometry;
            hierarchyID = shape.hierarchyID;
            shapeID = shape.shapeID;
            parentOffset = shape.offset;
            heightOffset = offset;
            matrix = shape.effectiveMatrix;
            type = shape.type;
        }

#region IEquatable

        public override bool Equals(object obj)
        {
            return obj is TreeVertex other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = initialized.GetHashCode();
                hashCode = (hashCode * 397) ^ visible.GetHashCode();
                hashCode = (hashCode * 397) ^ billboard.GetHashCode();
                hashCode = (hashCode * 397) ^ uvScaleUpdated.GetHashCode();
                hashCode = (hashCode * 397) ^ tangent.GetHashCode();
                hashCode = (hashCode * 397) ^ ambientOcclusion.GetHashCode();
                hashCode = (hashCode * 397) ^ variation.GetHashCode();
                hashCode = (hashCode * 397) ^ noise.GetHashCode();
                hashCode = (hashCode * 397) ^ hierarchyID;
                hashCode = (hashCode * 397) ^ shapeID;
                hashCode = (hashCode * 397) ^ parentOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ heightOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ matrix.GetHashCode();
                hashCode = (hashCode * 397) ^ normal.GetHashCode();
                hashCode = (hashCode * 397) ^ position.GetHashCode();
                hashCode = (hashCode * 397) ^ weldTo;
                hashCode = (hashCode * 397) ^ splineBreak.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) type;
                hashCode = (hashCode * 397) ^ raw_uv0.GetHashCode();
                hashCode = (hashCode * 397) ^ rect_uv0.GetHashCode();
                hashCode = (hashCode * 397) ^ uvScale.GetHashCode();
                hashCode = (hashCode * 397) ^ uvOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ uvRect.GetHashCode();
                hashCode = (hashCode * 397) ^ rawWind.GetHashCode();
                hashCode = (hashCode * 397) ^ wind.GetHashCode();
                hashCode = (hashCode * 397) ^ log.GetHashCode();
                hashCode = (hashCode * 397) ^ billboardData.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TreeVertex left, TreeVertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TreeVertex left, TreeVertex right)
        {
            return !left.Equals(right);
        }

#endregion

        public TreeVertex Clone()
        {
            var clone = new TreeVertex();

            clone.visible = visible;
            clone.billboard = billboard;
            clone.uvScaleUpdated = uvScaleUpdated;
            clone.tangent = tangent;
            clone.ambientOcclusion = ambientOcclusion;
            clone.variation = variation;
            clone.noise = noise;
            clone.hierarchyID = hierarchyID;
            clone.shapeID = shapeID;
            clone.parentOffset = parentOffset;
            clone.heightOffset = heightOffset;
            clone.matrix = matrix;
            clone.normal = normal;
            clone.position = position;
            clone.type = type;
            clone.raw_uv0 = raw_uv0;
            clone.rect_uv0 = rect_uv0;
            clone.uvScale = uvScale;
            clone.uvRect = uvRect;
            clone.rawWind = rawWind;
            clone.wind = wind;
            clone.log = log;
            clone.splineBreak = splineBreak;
            
            return clone;
        }
    }
}
